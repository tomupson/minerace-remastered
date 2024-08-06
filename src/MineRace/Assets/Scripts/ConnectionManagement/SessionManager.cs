using System.Collections.Generic;
using UnityEngine;

namespace MineRace.ConnectionManagement
{
    public sealed class SessionManager
    {
        private static SessionManager instance;

        private readonly Dictionary<string, SessionPlayerData> clientData = new Dictionary<string, SessionPlayerData>();
        private readonly Dictionary<ulong, string> clientIdToPlayerId = new Dictionary<ulong, string>();
        private bool hasGameStarted;

        public static SessionManager Instance => instance ??= new SessionManager();

        public void DisconnectClient(ulong clientId)
        {
            if (hasGameStarted)
            {
                // Mark client as disconnected, but keep their data so they can reconnect.
                if (clientIdToPlayerId.TryGetValue(clientId, out string playerId))
                {
                    SessionPlayerData? playerData = GetPlayerData(playerId);
                    if (playerData.HasValue && playerData.Value.ClientId == clientId)
                    {
                        SessionPlayerData clientData = playerData.Value;
                        clientData.IsConnected = false;
                        this.clientData[playerId] = clientData;
                    }
                }
            }
            else
            {
                // Game has not started, no need to keep their data 
                if (clientIdToPlayerId.TryGetValue(clientId, out string playerId))
                {
                    clientIdToPlayerId.Remove(clientId);
                    SessionPlayerData? playerData = GetPlayerData(playerId);
                    if (playerData.HasValue && playerData.Value.ClientId == clientId)
                    {
                        clientData.Remove(playerId);
                    }
                }
            }
        }

        public void SetupConnectingPlayerSessionData(ulong clientId, string playerId, SessionPlayerData sessionPlayerData)
        {
            bool isReconnecting = false;

            if (IsDuplicateConnection(playerId))
            {
                Debug.Log($"Player id '{playerId}' already exists. This is a duplicate connection. Rejecting this session data.");
                return;
            }

            if (clientData.ContainsKey(playerId))
            {
                if (!clientData[playerId].IsConnected)
                {
                    isReconnecting = true;
                }
            }

            if (isReconnecting)
            {
                sessionPlayerData = clientData[playerId];
                sessionPlayerData.ClientId = clientId;
                sessionPlayerData.IsConnected = true;
            }

            clientIdToPlayerId[clientId] = playerId;
            clientData[playerId] = sessionPlayerData;
        }

        public string GetPlayerId(ulong clientId)
        {
            if (clientIdToPlayerId.TryGetValue(clientId, out string playerId))
            {
                return playerId;
            }

            Debug.Log($"No client player id found for client id '{clientId}'");
            return null;
        }

        public SessionPlayerData? GetPlayerData(ulong clientId)
        {
            string playerId = GetPlayerId(clientId);
            if (playerId != null)
            {
                return GetPlayerData(playerId);
            }

            return null;
        }

        public SessionPlayerData? GetPlayerData(string playerId)
        {
            if (clientData.TryGetValue(playerId, out SessionPlayerData data))
            {
                return data;
            }

            Debug.Log($"No player data found for player id '{playerId}'");
            return null;
        }

        public void SetPlayerData(ulong clientId, SessionPlayerData sessionPlayerData)
        {
            if (!clientIdToPlayerId.TryGetValue(clientId, out string playerId))
            {
                Debug.LogError($"No client player id found for client id '{clientId}'");
                return;
            }

            clientData[playerId] = sessionPlayerData;
        }

        public void OnGameStarted()
        {
            hasGameStarted = true;
        }

        public void OnGameEnded()
        {
            ClearDisconnectedPlayersData();
            ReinitializePlayersData();
            hasGameStarted = false;
        }

        public void OnServerEnded()
        {
            clientData.Clear();
            clientIdToPlayerId.Clear();
            hasGameStarted = false;
        }

        private bool IsDuplicateConnection(string playerId)
        {
            return clientData.ContainsKey(playerId) && clientData[playerId].IsConnected;
        }

        private void ReinitializePlayersData()
        {
            foreach (ulong id in clientIdToPlayerId.Keys)
            {
                string playerId = clientIdToPlayerId[id];
                SessionPlayerData sessionPlayerData = clientData[playerId];
                sessionPlayerData.Reinitialize();
                clientData[playerId] = sessionPlayerData;
            }
        }

        private void ClearDisconnectedPlayersData()
        {
            List<ulong> idsToClear = new List<ulong>();
            foreach (ulong id in clientIdToPlayerId.Keys)
            {
                SessionPlayerData? data = GetPlayerData(id);
                if (data is { IsConnected: false })
                {
                    idsToClear.Add(id);
                }
            }

            foreach (ulong id in idsToClear)
            {
                string playerId = clientIdToPlayerId[id];
                SessionPlayerData? playerData = GetPlayerData(playerId);
                if (playerData.HasValue && playerData.Value.ClientId == id)
                {
                    clientData.Remove(playerId);
                }

                clientIdToPlayerId.Remove(id);
            }
        }
    }
}
