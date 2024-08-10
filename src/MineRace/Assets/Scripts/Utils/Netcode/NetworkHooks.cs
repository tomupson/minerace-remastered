using System;
using Unity.Netcode;

namespace MineRace.Utils.Netcode
{
    public class NetworkHooks : NetworkBehaviour
    {
        public event Action OnNetworkSpawnHook;
        public event Action OnNetworkDespawnHook;

        public override void OnNetworkSpawn() => OnNetworkSpawnHook?.Invoke();

        public override void OnNetworkDespawn() => OnNetworkDespawnHook?.Invoke();
    }
}
