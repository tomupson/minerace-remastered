using System.Collections;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using UnityEngine;
using VContainer;

public class WaitingForPlayersUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private Coroutine waitForPlayersCoroutine;

    [SerializeField] private TextMeshProUGUI waitingForPlayersText;

    private void Start()
    {
        gameObject.SetActive(true);

        subscriptions = new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnGameStateChanged(GameState state)
    {
        bool isWaitingForPlayers = state == GameState.WaitingForPlayers;
        gameObject.SetActive(isWaitingForPlayers);

        if (!isWaitingForPlayers && waitForPlayersCoroutine != null)
        {
            StopCoroutine(waitForPlayersCoroutine);
            waitForPlayersCoroutine = null;
        }
        else if (isWaitingForPlayers && waitForPlayersCoroutine == null)
        {
            waitForPlayersCoroutine = StartCoroutine(WaitForPlayers());
        }
    }

    private IEnumerator WaitForPlayers()
    {
        int index = 0;
        while (true)
        {
            yield return new WaitForSeconds(1);
            waitingForPlayersText.text = $"WAITING FOR PLAYERS{new string('.', index)}";
            index++;
            index %= 4;
        }
    }
}
