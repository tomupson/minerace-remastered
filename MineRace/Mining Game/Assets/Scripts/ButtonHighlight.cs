using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ButtonHighlight.cs allows a button's text component to change color when 
/// </summary>

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // When the mouse is hovering over the object this script is on...
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = Color.grey; // Set its text color to grey.
    }

    // When the mouse leaves the object this script is on...
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = Color.white; // Set its text color to white.
    }
}
