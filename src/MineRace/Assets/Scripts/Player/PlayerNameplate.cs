using Unity.Netcode;
using UnityEngine.UI;

public class PlayerNameplate : NetworkBehaviour
{
    private Player player;
    private Text usernameText;

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
        player.NetworkPlayerState.State.OnValueChanged += HandlePlayerStateChanged;
        SetUsername(player.NetworkPlayerState.Username.Value.ToString());
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        bool isPlaying = newState == PlayerState.Playing;
        gameObject.SetActive(isPlaying);
    }

    private void SetUsername(string username)
    {
        usernameText.text = username;
    }
}
