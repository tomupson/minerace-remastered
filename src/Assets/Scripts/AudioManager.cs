using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles the playing of pre-defined sounds
/// </summary>
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
            GameObject soundObject = new GameObject($"Sound_{i}_{sounds[i]}");
            soundObject.transform.SetParent(transform);

            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.clip = sounds[i].clip;

            soundData.Add(new SoundData(sounds[i], source));
        }
    }

    public void PlaySound(string name)
    {
        SoundData data = soundData.FirstOrDefault(s => s.sound.soundName.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (data == null)
        {
            Debug.LogWarning($"No sound found with name '{name}'");
            return;
        }

        data.source.volume = data.sound.volume * (1 + UnityEngine.Random.Range(-data.sound.volumeVariance / 2f, data.sound.volumeVariance / 2f));
        data.source.pitch = data.sound.pitch * (1 + UnityEngine.Random.Range(-data.sound.pitchVariance / 2f, data.sound.pitchVariance / 2f));
        data.source.Play();
    }
}
