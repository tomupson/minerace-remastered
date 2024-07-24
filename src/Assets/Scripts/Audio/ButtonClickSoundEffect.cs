using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSoundEffect : MonoBehaviour
{
    [SerializeField] private Sound sound;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickEffect);
    }

    private void PlayClickEffect() => AudioManager.Instance.PlayOneShot(sound);
}
