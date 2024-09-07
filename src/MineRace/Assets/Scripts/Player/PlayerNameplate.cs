using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using Unity.Netcode;

public class PlayerNameplate : NetworkBehaviour
{
    private Player player;
    private TextMeshProUGUI usernameText;

    private DisposableGroup subscriptions;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        usernameText = GetComponentInChildren<TextMeshProUGUI>();

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
