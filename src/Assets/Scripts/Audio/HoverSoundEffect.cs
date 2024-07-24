using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSoundEffect : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Sound sound;

    public void OnPointerEnter(PointerEventData eventData) =>
        AudioManager.Instance.PlayOneShot(sound);
}
