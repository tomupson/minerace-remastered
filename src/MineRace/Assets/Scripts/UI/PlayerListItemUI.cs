using TMPro;
using UnityEngine;

public class PlayerListItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;

    public void Setup(string playerName)
    {
        playerNameText.text = playerName;
    }
}
