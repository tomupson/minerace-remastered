using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text text;
    private Color originalColour;

    [SerializeField] private Color highlightColour = Color.grey;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        originalColour = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = highlightColour;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = originalColour;
    }
}
