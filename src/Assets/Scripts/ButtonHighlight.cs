using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows a button's text component to change color when hovered
/// </summary>
public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = Color.grey;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = Color.white;
    }
}
