using UnityEngine;

namespace MineRace.ConnectionManagement
{
    public struct SessionPlayerData
    {
        public ulong ClientId { get; set; }
        public string PlayerName { get; set; }
        public Vector3 PlayerPosition { get; set; }
        public bool HasCharacterSpawned { get; set; }
        public bool IsConnected { get; set; }

        public SessionPlayerData(ulong clientId, string name, bool hasCharacterSpawned = false, bool isConnected = false)
        {
            ClientId = clientId;
            PlayerName = name;
            PlayerPosition = Vector3.zero;
            HasCharacterSpawned = hasCharacterSpawned;
            IsConnected = isConnected;
        }

        public void Reinitialize()
        {
            HasCharacterSpawned = false;
        }
    }
}
