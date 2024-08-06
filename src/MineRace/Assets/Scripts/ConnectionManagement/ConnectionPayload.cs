using System;

namespace Assets.Scripts.ConnectionManagement
{
    [Serializable]
    internal sealed class ConnectionPayload
    {
        public string PlayerId;
        public string PlayerName;

        public ConnectionPayload(string playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
        }
    }
}
