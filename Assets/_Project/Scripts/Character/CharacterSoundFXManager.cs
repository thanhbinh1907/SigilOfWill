using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
	public class CharacterSoundFXManager : MonoBehaviour
	{
		private AudioSource audioSource;

		[Header("Damage Grunts")]
		[SerializeField] protected AudioClip[] damageGrunts;

		[Header("Attack Grunts")]
		[SerializeField] protected AudioClip[] attackGrunts;

		[Header("Foot Steps")]
		[SerializeField] protected AudioClip[] footSteps;
		protected virtual void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
		{
			float finalVolume = volume * (WorldSoundFXManager.instance != null ? WorldSoundFXManager.instance.sfxVolume : 1f);
			audioSource.PlayOneShot(soundFX, finalVolume);
			audioSource.pitch = 1;

			if (randomizePitch)
			{
				audioSource.pitch += UnityEngine.Random.Range(-pitchRandom, pitchRandom);
			}
		}

		public void PlayRollSoundFX()
		{
			float finalVolume = WorldSoundFXManager.instance != null ? WorldSoundFXManager.instance.sfxVolume : 1f;
			audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX, finalVolume);
		}

		public virtual void PlayDamageGruntSFX()
		{
			if (damageGrunts.Length > 0)
				PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
		}

		public virtual void PlayAttackGruntSFX()
		{
			if (attackGrunts.Length > 0)
				PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
		}

		public virtual void PlayFootStepSFX()
		{
			if (footSteps.Length > 0)
				PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footSteps));
		}
	}
}