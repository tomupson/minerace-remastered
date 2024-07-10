using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    private Player player;

    [Header("Username Overhead UI")]
    [SerializeField] private GameObject usernameCanvas;
    [SerializeField] private Text usernameText;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void Start()
    {
        usernameCanvas.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        player.Username.OnValueChanged += HandleUsernameChanged;
        player.State.OnValueChanged += HandlePlayerStateChanged;
    }

    public override void OnNetworkDespawn()
    {
        player.Username.OnValueChanged -= HandleUsernameChanged;
        player.State.OnValueChanged -= HandlePlayerStateChanged;
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.WaitingForPlayers:
                usernameCanvas.SetActive(false);
                break;
            case PlayerState.InGame:
                if (!usernameCanvas.activeSelf)
                {
                    usernameCanvas.SetActive(true);
                }

                usernameCanvas.GetComponent<RectTransform>().position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, 0);
                break;
        }
    }

    private void HandleUsernameChanged(FixedString64Bytes previousUsername, FixedString64Bytes newUsername)
    {
        usernameText.text = newUsername.ToString();
    }
}
