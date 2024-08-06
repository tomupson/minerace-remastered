using UnityEngine;

public struct SessionPlayerData
{
    public string PlayerName { get; set; }
    public Vector3 PlayerPosition { get; set; }
    public bool HasCharacterSpawned { get; set; }
    public bool IsConnected { get; set; }
    public ulong ClientId { get; set; }

    public SessionPlayerData(ulong clientId, string name, bool isConnected = false, bool hasCharacterSpawned = false)
    {
        ClientId = clientId;
        PlayerName = name;
        PlayerPosition = Vector3.zero;
        IsConnected = isConnected;
        HasCharacterSpawned = hasCharacterSpawned;
    }

    public void Reinitialize()
    {
        HasCharacterSpawned = false;
    }
}