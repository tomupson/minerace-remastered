using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Allows a button's text component to scale when hovered
/// </summary>
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        AudioManager.Instance.PlaySound("button_hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().transform.localScale = new Vector3(1, 1, 1);
    }
}
