using UnityEngine;

namespace SG
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXVolumeReceiver : MonoBehaviour
    {
        private AudioSource audioSource;
        private float baseVolume = 1f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            baseVolume = audioSource.volume;
        }

        private void Start()
        {
            UpdateVolume(WorldSoundFXManager.instance != null ? WorldSoundFXManager.instance.sfxVolume : 0.5f);
            if (WorldSoundFXManager.instance != null)
            {
                WorldSoundFXManager.instance.OnSFXVolumeChanged += UpdateVolume;
            }
        }

        private void OnDestroy()
        {
            if (WorldSoundFXManager.instance != null)
            {
                WorldSoundFXManager.instance.OnSFXVolumeChanged -= UpdateVolume;
            }
        }

        private void UpdateVolume(float volume)
        {
            if (audioSource != null)
            {
                audioSource.volume = baseVolume * volume;
            }
        }
    }
}
