using TMPro;
using UnityEngine;

public class ChatItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private TextMeshProUGUI messageText;

    public void Setup(string sender, string message, Color colour)
    {
        senderText.text = $"[{sender}]";
        messageText.text = message;
        messageText.color = colour;
    }
}
