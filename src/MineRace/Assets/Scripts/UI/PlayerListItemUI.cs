using UnityEngine;
using UnityEngine.UI;

public class PlayerListItemUI : MonoBehaviour
{
    [SerializeField] private Text playerNameText;

    public void Setup(string playerName)
    {
        playerNameText.text = playerName;
    }
}
