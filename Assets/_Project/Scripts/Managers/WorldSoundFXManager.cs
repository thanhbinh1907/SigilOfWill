using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager instance;

        [Header("Damage Sound")]
        public AudioClip[] slashSFX;

		[Header("Action Sounds")]
		public AudioClip rollSFX;
		public AudioClip pickupItemSFX;

		[Header("Boss Music Players")]
		[SerializeField] AudioSource bossIntroPlayer;
		[SerializeField] AudioSource bossLoopPlayer;
		[SerializeField] float musicVolume = 0.5f;
		[Header("Global Volume Settings")]
		public float sfxVolume = 0.5f;

		public System.Action<float> OnBGMVolumeChanged;
		public System.Action<float> OnSFXVolumeChanged;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
				return;
            }

			// Load saved settings
			musicVolume = PlayerPrefs.GetFloat("BGM_Volume", 0.5f);
			sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 0.5f);


			if (bossIntroPlayer == null)
			{
				bossIntroPlayer = gameObject.AddComponent<AudioSource>();
				bossIntroPlayer.loop = false;
				bossIntroPlayer.playOnAwake = false;
			}
			if (bossLoopPlayer == null)
			{
				bossLoopPlayer = gameObject.AddComponent<AudioSource>();
				bossLoopPlayer.loop = true;
				bossLoopPlayer.playOnAwake = false;
			}

			if (bossIntroPlayer != null)
			{
				bossIntroPlayer.spatialBlend = 0f;
				bossIntroPlayer.volume = musicVolume;
			}
			if (bossLoopPlayer != null)
			{
				bossLoopPlayer.spatialBlend = 0f;
				bossLoopPlayer.volume = musicVolume;
			}
		}

		public void SetBGMVolume(float volume)
		{
			musicVolume = volume;
			PlayerPrefs.SetFloat("BGM_Volume", musicVolume);
			PlayerPrefs.Save();

			if (bossIntroPlayer != null) bossIntroPlayer.volume = musicVolume;
			if (bossLoopPlayer != null) bossLoopPlayer.volume = musicVolume;

			OnBGMVolumeChanged?.Invoke(musicVolume);
		}

		public void SetSFXVolume(float volume)
		{
			sfxVolume = volume;
			PlayerPrefs.SetFloat("SFX_Volume", sfxVolume);
			PlayerPrefs.Save();
			OnSFXVolumeChanged?.Invoke(sfxVolume);
		}

		public float GetBGMVolume()
		{
			return musicVolume;
		}

		public float GetSFXVolume()
		{
			return sfxVolume;
		}

		public void PlayBossTrack(AudioClip introTrack, AudioClip loopTrack)
		{
			if (bossIntroPlayer == null || bossLoopPlayer == null)
				return;

			bossIntroPlayer.Stop();
			bossLoopPlayer.Stop();

			bossIntroPlayer.volume = musicVolume;
			bossLoopPlayer.volume = musicVolume;

			bossIntroPlayer.clip = introTrack;
			bossLoopPlayer.clip = loopTrack;

			if (introTrack != null)
			{
				bossIntroPlayer.Play();
				if (loopTrack != null)
				{
					bossLoopPlayer.PlayDelayed(introTrack.length);
				}
			}
			else if (loopTrack != null)
			{
				bossLoopPlayer.Play();
			}
		}

		public void StopBossMusic()
		{
			StartCoroutine(FadeOutBossMusic(2f));
		}

		public void StopBossMusicImmediate()
		{
			StopAllCoroutines();
			if (bossIntroPlayer != null)
			{
				bossIntroPlayer.Stop();
				bossIntroPlayer.volume = musicVolume;
			}
			if (bossLoopPlayer != null)
			{
				bossLoopPlayer.Stop();
				bossLoopPlayer.volume = musicVolume;
			}
		}

		private void OnEnable()
		{
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDisable()
		{
			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
		{
			StopBossMusicImmediate();
		}

		private IEnumerator FadeOutBossMusic(float duration)
		{
			if (bossIntroPlayer == null || bossLoopPlayer == null)
				yield break;

			float startIntroVol = bossIntroPlayer.volume;
			float startLoopVol = bossLoopPlayer.volume;
			float timer = 0f;

			while (timer < duration)
			{
				timer += Time.deltaTime;
				float ratio = 1f - (timer / duration);
				if (bossIntroPlayer != null) bossIntroPlayer.volume = startIntroVol * ratio;
				if (bossLoopPlayer != null) bossLoopPlayer.volume = startLoopVol * ratio;
				yield return null;
			}

			if (bossIntroPlayer != null)
			{
				bossIntroPlayer.Stop();
				bossIntroPlayer.volume = musicVolume;
			}
			if (bossLoopPlayer != null)
			{
				bossLoopPlayer.Stop();
				bossLoopPlayer.volume = musicVolume;
			}
		}

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
		}

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

		public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
        {
            int index = Random.Range(0, array.Length);
            return array[index];
		}

        /*
        public AudioClip ChooseRandomFootStepSoundBasedOnGround(GameObject steppedOnObject, CharacterManager character)
        {
            if (steppedOnObject.tag == "Dirt")
            {
                return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsDirt);
			}
            else if (steppedOnObject.tag == "Stone")
			{
                return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsStone);
			}

            return null;
        }
        */
	}
}