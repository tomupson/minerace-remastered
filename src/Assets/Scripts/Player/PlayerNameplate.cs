using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
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
        player.State.OnValueChanged += HandlePlayerStateChanged;
        player.Username.OnValueChanged += HandleUsernameChanged;
        SetUsername(player.Username.Value.ToString());
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Playing:
                gameObject.SetActive(true);
                // TODO: Required?
                gameObject.GetComponent<RectTransform>().position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, 0);
                break;
            case PlayerState.Completed:
                gameObject.SetActive(false);
                break;
        }
    }

    private void HandleUsernameChanged(FixedString64Bytes previousUsername, FixedString64Bytes newUsername)
    {
        SetUsername(newUsername.ToString());
    }

    private void SetUsername(string username)
    {
        usernameText.text = username;
    }
}
