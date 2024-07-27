using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MineRace.Utils.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Text text;
        private Vector3 originalScale;

        [SerializeField] private float scaleMultiplier = 1.05f;

        private void Awake()
        {
            text = GetComponentInChildren<Text>();
            originalScale = text.transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            text.transform.localScale = originalScale * scaleMultiplier;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            text.transform.localScale = originalScale;
        }
    }
}
