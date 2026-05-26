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

		[Header("Boss Music Players")]
		[SerializeField] AudioSource bossIntroPlayer;
		[SerializeField] AudioSource bossLoopPlayer;
		[SerializeField] float musicVolume = 0.5f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

			// Tự động khởi tạo AudioSource nếu chưa được gán trong Inspector
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

			// Đảm bảo âm thanh phẳng 2D tuyệt đối (không bị giảm theo khoảng cách)
			if (bossIntroPlayer != null) bossIntroPlayer.spatialBlend = 0f;
			if (bossLoopPlayer != null) bossLoopPlayer.spatialBlend = 0f;
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