using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Allows a button's text component to scale when hovered
/// </summary>
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text text;
    private Vector3 originalScale;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        originalScale = text.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.transform.localScale = originalScale * 1.05f;
        //AudioManager.Instance.PlaySound("button_hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.transform.localScale = originalScale;
    }
}
