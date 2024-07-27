using UnityEngine;

namespace MineRace.Audio
{
    public static class AudioManager
    {
        public static void PlayOneShot(Sound sound)
        {
            PlayOneShot(sound, Vector3.zero);
        }

        public static void PlayOneShot(Sound sound, Vector3 position)
        {
            GameObject gameObject = new GameObject($"Audio-{sound.name}");
            gameObject.transform.position = position;

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = sound.clip;
            audioSource.spatialBlend = 1f;

            Configure(audioSource, sound);

            audioSource.Play();

            float pitchCorrectedLength = sound.clip.length / sound.pitch;
            Object.Destroy(gameObject, pitchCorrectedLength * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
        }

        private static void Configure(AudioSource source, Sound sound)
        {
            source.volume = sound.volume * (1 + Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
            source.pitch = sound.pitch * (1 + Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));
        }
    }
}
