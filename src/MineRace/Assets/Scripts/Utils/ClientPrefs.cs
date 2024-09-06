using UnityEngine;

namespace MineRace.Utils
{
    public static class ClientPrefs
    {
        private const string PlayerNameKey = "player_name";

        public static string GetPlayerName() => PlayerPrefs.GetString(PlayerNameKey);

        public static void SetPlayerName(string playerName) => PlayerPrefs.SetString(PlayerNameKey, playerName);

        public static void ClearPlayerName() => PlayerPrefs.SetString(PlayerNameKey, null);
    }
}
