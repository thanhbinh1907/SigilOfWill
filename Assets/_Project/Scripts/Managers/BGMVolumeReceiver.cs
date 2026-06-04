using UnityEngine;

namespace SG
{
    [RequireComponent(typeof(AudioSource))]
    public class BGMVolumeReceiver : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            UpdateVolume(WorldSoundFXManager.instance != null ? WorldSoundFXManager.instance.GetBGMVolume() : 0.5f);
            if (WorldSoundFXManager.instance != null)
            {
                WorldSoundFXManager.instance.OnBGMVolumeChanged += UpdateVolume;
            }
        }

        private void OnDestroy()
        {
            if (WorldSoundFXManager.instance != null)
            {
                WorldSoundFXManager.instance.OnBGMVolumeChanged -= UpdateVolume;
            }
        }

        private void UpdateVolume(float volume)
        {
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }
    }
}
