using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerNameplate : NetworkBehaviour
{
    private Player player;
    private Text usernameText;

    private DisposableGroup subscriptions;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        usernameText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(player.NetworkPlayerState.State.Subscribe(OnPlayerStateChanged));

        usernameText.text = player.NetworkPlayerState.Username.Value.ToString();
    }

    public override void OnNetworkDespawn()
    {
        subscriptions?.Dispose();
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        bool isPlaying = state == PlayerState.Playing;
        gameObject.SetActive(isPlaying);
    }
}
