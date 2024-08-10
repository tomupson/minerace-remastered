using System;
using MineRace.Infrastructure;
using Unity.Netcode;

namespace MineRace.Utils.Netcode
{
    internal sealed class NetworkVariableSubscriber<T> : ISubscriber<T>, ISubscriber<NetworkVariableChangedEvent<T>>
    {
        private readonly NetworkVariable<T> networkVariable;
        private readonly bool emitOnSubscribe;

        private Action<NetworkVariableChangedEvent<T>> handler;

        public NetworkVariableSubscriber(NetworkVariable<T> networkVariable, bool emitOnSubscribe = false)
        {
            this.networkVariable = networkVariable;
            this.emitOnSubscribe = emitOnSubscribe;
        }

        public IDisposable Subscribe(Action<T> handler)
        {
            this.handler = @event => handler(@event.newValue);
            networkVariable.OnValueChanged += OnValueChanged;

            if (emitOnSubscribe)
            {
                handler(networkVariable.Value);
            }

            return new DisposableSubscription<T>(this, handler);
        }

        public IDisposable Subscribe(Action<NetworkVariableChangedEvent<T>> handler)
        {
            this.handler = handler;
            networkVariable.OnValueChanged += OnValueChanged;
            return new DisposableSubscription<NetworkVariableChangedEvent<T>>(this, handler);
        }

        public void Unsubscribe(Action<T> handler) => Unsubscribe();

        public void Unsubscribe(Action<NetworkVariableChangedEvent<T>> handler) => Unsubscribe();

        private void Unsubscribe()
        {
            networkVariable.OnValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(T previousValue, T newValue)
        {
            handler.Invoke(new NetworkVariableChangedEvent<T>(previousValue, newValue));
        }
    }

    public static class NetworkVariableExtensions
    {
        public static IDisposable Subscribe<T>(this NetworkVariable<T> networkVariable, Action<T> handler)
        {
            NetworkVariableSubscriber<T> subscriber = new NetworkVariableSubscriber<T>(networkVariable, emitOnSubscribe: true);
            return subscriber.Subscribe(handler);
        }

        public static IDisposable Subscribe<T>(this NetworkVariable<T> networkVariable, Action<NetworkVariableChangedEvent<T>> handler)
        {
            NetworkVariableSubscriber<T> subscriber = new NetworkVariableSubscriber<T>(networkVariable, emitOnSubscribe: true);
            return subscriber.Subscribe(handler);
        }
    }
}
