using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace MineRace.Infrastructure
{
    public sealed class NetworkedMessageChannel<T> : MessageChannel<T> where T : unmanaged, INetworkSerializeByMemcpy
    {
        private readonly string name;
        private NetworkManager networkManager;

        public NetworkedMessageChannel()
        {
            name = $"{typeof(T).FullName}NetworkMessageChannel";
        }

        [Inject, UsedImplicitly]
        private void InjectDependencies(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.networkManager.OnClientConnectedCallback += OnClientConnected;
            if (this.networkManager.IsListening)
            {
                RegisterHandler();
            }
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                if (networkManager != null && networkManager.CustomMessagingManager != null)
                {
                    networkManager.CustomMessagingManager.UnregisterNamedMessageHandler(name);
                }
            }

            base.Dispose();
        }

        public override void Publish(T message)
        {
            if (!networkManager.IsServer)
            {
                Debug.LogError("Only a server can publish in a NetworkedMessageChannel");
                return;
            }

            SendMessageThroughNetwork(message);
            base.Publish(message);
        }

        private void OnClientConnected(ulong clientId)
        {
            RegisterHandler();
        }

        private void RegisterHandler()
        {
            if (!networkManager.IsServer)
            {
                networkManager.CustomMessagingManager.RegisterNamedMessageHandler(name, ReceiveMessageThroughNetwork);
            }
        }

        private void SendMessageThroughNetwork(T message)
        {
            if (networkManager == null || networkManager.CustomMessagingManager == null)
            {
                return;
            }

            FastBufferWriter writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
            writer.WriteValueSafe(message);
            networkManager.CustomMessagingManager.SendNamedMessageToAll(name, writer);
        }

        private void ReceiveMessageThroughNetwork(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out T message);
            base.Publish(message);
        }
    }
}
