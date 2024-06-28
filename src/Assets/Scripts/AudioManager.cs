using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName; // Name of the sound
    public AudioClip clip; // The actual sound clip

    private AudioSource source; // Where it is being played from.

    [Range(0f, 1f)]
    public float volume; // Volume slider between 0-1
    [Range(0.5f, 1.5f)]
    public float pitch; // Pitch slider between 0.5-1.5

    [Range(0f, 0.5f)]
    public float volumeVariance = 0.1f; // Volume variance
    [Range(0f, 0.5f)]
    public float pitchVariance = 0.1f; // Pitch Variance e.g if 0.1, and pitch is 0.7, actual pitch is between 0.65-0.75

    public void SetSource(AudioSource source)
    {
        this.source = source; // Set our source
        source.clip = clip; // And assign the clip to it.
    }

    public void PlayAudio() 
    {
        source.volume = volume * (1 + Random.Range(-volumeVariance / 2f, volumeVariance / 2f)); // Calculate the volume
        source.pitch = pitch * (1 + Random.Range(-pitchVariance / 2f, pitchVariance / 2f)); // Calculate the pitch
        source.Play(); // Play the sound
    }
}

/// <summary>
/// AudioManager.cs handles the playing of pre-defined sounds.
/// </summary>

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] Sound[] sounds; // List of all sounds.

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1) // If one already exists
        {
            Destroy(gameObject); // Remove it
        }

        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        for (var i = 0; i < sounds.Length; i++) // For every sound
        {
            GameObject go = new GameObject("SOUND_" + i + "_" + sounds[i]); // Create a new gameobject with the name of the sound
            go.transform.SetParent(transform); // Parent it to the gameobject of the AudioManager
            sounds[i].SetSource(go.AddComponent<AudioSource>()); // And set that sound's Audio Source to the new gameobject.
        }
    }

    public void PlaySound(string name) // When you want to play a sound
    {
        for (var i = 0; i < sounds.Length; i++) // Loop through all sounds
        {
            if (sounds[i].soundName == name) // until you find a sound with the requested name
            {
                sounds[i].PlayAudio(); // In which case you play that sound
                return; // and return so you don't play multiple sounds if there are more with the same name
            }
        }

        Debug.LogWarning(string.Format("No Sound found with name '{0}'", name)); // If we're here this means no sound was found so we print this to the debug console.
    }
}
