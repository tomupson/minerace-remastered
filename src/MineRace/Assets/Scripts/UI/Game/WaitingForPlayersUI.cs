using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class WaitingForPlayersUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    [SerializeField] private Text waitingForPlayersText;

    private void Start()
    {
        networkGameState.State.OnValueChanged += HandleGameStateChanged;

        gameObject.SetActive(true);

        StartCoroutine(WaitForPlayers());
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        bool isWaitingForPlayers = newState == GameState.WaitingForPlayers;
        gameObject.SetActive(isWaitingForPlayers);
    }

    private IEnumerator WaitForPlayers()
    {
        int index = 0;
        while (NetworkManager.Singleton.ConnectedClientsIds.Count < 2)
        {
            yield return new WaitForSeconds(1);
            waitingForPlayersText.text = $"WAITING FOR PLAYERS{new string('.', index)}";
            index++;
            index %= 4;
        }
    }
}
