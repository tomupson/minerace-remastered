using UnityEngine;
using UnityEngine.UI;

public class ChatItemUI : MonoBehaviour
{
    [SerializeField] private Text senderText;
    [SerializeField] private Text messageText;

    public void Setup(string sender, string message, Color colour)
    {
        senderText.text = $"[{sender}]";
        messageText.text = message;
        messageText.color = colour;
    }
}
