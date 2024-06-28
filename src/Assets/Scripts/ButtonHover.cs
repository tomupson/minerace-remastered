using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // When the mouse is hovering over the object this script is attached to...
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().transform.localScale = new Vector3(1.05f, 1.05f, 1.05f); // Shrink it by a little
        AudioManager.Instance.PlaySound("button_hover"); // And play the button hover sound effect.
    }

    // When the mouse exits the object this script is attached to...
    public void OnPointerExit(PointerEventData eventData)
    {
        // Grow it back to its default size.
        GetComponentInChildren<Text>().transform.localScale = new Vector3(1, 1, 1);
    }
}
