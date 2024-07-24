using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private class SoundData
    {
        public Sound sound;
        public AudioSource source;

        public SoundData(Sound sound, AudioSource source)
        {
            this.sound = sound;
            this.source = source;
        }
    }

    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] sounds;

    private readonly List<SoundData> soundData = new List<SoundData>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject soundObject = new GameObject($"Sound_{i}_{sounds[i].name}");
            soundObject.transform.SetParent(transform);

            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.clip = sounds[i].clip;

            soundData.Add(new SoundData(sounds[i], source));
        }
    }

    public void PlayOneShot(Sound sound)
    {
        PlayOneShot(sound, Vector3.zero);
    }

    public void PlayOneShot(Sound sound, Vector3 position)
    {
        GameObject gameObject = new GameObject($"Audio-{sound.name}");
        gameObject.transform.position = position;

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sound.clip;
        audioSource.spatialBlend = 1f;

        Configure(audioSource, sound);

        audioSource.Play();

        Destroy(gameObject, sound.clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

    public void PlaySound(string name)
    {
        SoundData data = soundData.FirstOrDefault(s => s.sound.soundName.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (data == null)
        {
            Debug.LogWarning($"No sound found with name '{name}'");
            return;
        }

        Configure(data.source, data.sound);
        data.source.Play();
    }

    private void Configure(AudioSource source, Sound sound)
    {
        source.volume = sound.volume * (1 + UnityEngine.Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
        source.pitch = sound.pitch * (1 + UnityEngine.Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));
    }
}
