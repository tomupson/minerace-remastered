using UnityEngine;
using UnityEngine.EventSystems;

namespace MineRace.Audio
{
    public class HoverSoundEffect : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private Sound sound;

        public void OnPointerEnter(PointerEventData eventData) =>
            AudioManager.PlayOneShot(sound);
    }
}
