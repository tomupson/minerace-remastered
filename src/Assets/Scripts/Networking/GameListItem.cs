// TODO: NETWORKING
using UnityEngine;
//using UnityEngine.Networking.Match;
//using UnityEngine.UI;

/// <summary>
/// GameListItem.cs is a class attached to each of the Room List Rooms on the lobby screen.
/// It shows the game name, whether or not it's passworded, and adds a changeable callback for what to do when the join button is pressed.
/// </summary>

public class GameListItem : MonoBehaviour
{
    //public delegate void JoinRoomDelegate(MatchInfoSnapshot _match); // Delegate for handing the callback.
    //private JoinRoomDelegate joinRoomCallback; // The callback.

    //[Header("Match List Item Text Items")]
    //[SerializeField] private Text matchInfoText;
    //[SerializeField] private Text matchTypeText;

    //private MatchInfoSnapshot match; // Information about the match that the list item is holding.

    //public void Setup(MatchInfoSnapshot match, JoinRoomDelegate joinRoomCallback)
    //{
    //    this.match = match;
    //    this.joinRoomCallback = joinRoomCallback;
    //    matchInfoText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")"; // Sets the match info text to the match name + the amount of connected players.
    //    if (match.isPrivate) // If the match is private, show the user it's passworded in a specific color (red)
    //    {
    //        matchTypeText.color = new Color32(255, 69, 69, 255); 
    //        matchTypeText.text = "PASSWORDED";
    //    }
    //    else // Otherwise show that it's an open game in another color (green).
    //    {
    //        matchTypeText.color = new Color32(149, 255, 69, 255);
    //        matchTypeText.text = "OPEN";
    //    }
    //}

    //public void JoinGame()
    //{
    //    joinRoomCallback.Invoke(match); // Invoke (call) the callback when the "Join" button is pressed.
    //}
}
