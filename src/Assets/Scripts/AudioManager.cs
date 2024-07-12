using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class Sound
{
    private AudioSource source;

    public string soundName;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.5f, 1.5f)]
    public float pitch;
    [Range(0f, 0.5f)]
    public float volumeVariance = 0.1f;
    [Range(0f, 0.5f)]
    public float pitchVariance = 0.1f;

    public void SetSource(AudioSource source)
    {
        this.source = source;
        source.clip = clip;
    }

    public void PlayAudio()
    {
        source.volume = volume * (1 + UnityEngine.Random.Range(-volumeVariance / 2f, volumeVariance / 2f));
        source.pitch = pitch * (1 + UnityEngine.Random.Range(-pitchVariance / 2f, pitchVariance / 2f));
        source.Play();
    }
}

/// <summary>
/// Handles the playing of pre-defined sounds
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        // TODO: Look at removing this once Login scene is properly used
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject soundObject = new GameObject($"Sound_{i}_{sounds[i]}");
            soundObject.transform.SetParent(transform);
            sounds[i].SetSource(soundObject.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string name)
    {
        Sound sound = sounds.FirstOrDefault(s => s.soundName.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (sound == null)
        {
            Debug.LogWarning($"No Sound found with name '{name}'");
            return;
        }

        sound.PlayAudio();
    }
}
