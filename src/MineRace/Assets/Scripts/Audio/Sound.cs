using UnityEngine;

namespace MineRace.Audio
{
    [CreateAssetMenu(fileName = "Sound", menuName = "MineRace/Sound")]
    public class Sound : ScriptableObject
    {
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Range(0.5f, 1.5f)]
        public float pitch;
        [Range(0f, 0.5f)]
        public float volumeVariance = 0.1f;
        [Range(0f, 0.5f)]
        public float pitchVariance = 0.1f;
    }
}
