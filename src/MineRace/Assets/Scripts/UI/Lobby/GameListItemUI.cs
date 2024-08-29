using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameListItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI matchInfoText;
    [SerializeField] private TextMeshProUGUI matchTypeText;
    [SerializeField] private Button joinButton;

    public void Setup(Lobby lobby, UnityAction<Lobby> onJoin)
    {
        matchInfoText.text = $"{lobby.Name} ({lobby.AvailableSlots}/{lobby.MaxPlayers})";
        if (lobby.HasPassword)
        {
            matchTypeText.color = new Color32(255, 69, 69, 255);
            matchTypeText.text = "PASSWORDED";
        }
        else
        {
            matchTypeText.color = new Color32(149, 255, 69, 255);
            matchTypeText.text = "OPEN";
        }

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() =>
        {
            onJoin(lobby);
        });
    }
}
