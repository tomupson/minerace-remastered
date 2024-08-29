using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MineRace.Utils.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonHoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private TextMeshProUGUI text;
        private Color originalColour;

        [SerializeField] private Color highlightColour = Color.grey;

        private void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
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
}
