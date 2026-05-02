using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
	public class CharacterSoundFXManager : MonoBehaviour
	{
		private AudioSource audioSource;

		protected virtual void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
		{
			audioSource.PlayOneShot(soundFX, volume);
			// RESET PITCH
			audioSource.pitch = 1;

			// RANDOMIZE PITCH BECAUSE WE DONT WANT THE SOUND TO BE EXACTLY THE SAME EVERY TIME IT PLAYS, IT CAN GET ANNOYING
			if (randomizePitch)
			{
				audioSource.pitch += UnityEngine.Random.Range(-pitchRandom, pitchRandom);
			}
		}

		public void PlayRollSoundFX()
		{
			audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
		}
	}
}