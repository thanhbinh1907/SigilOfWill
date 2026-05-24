using UnityEngine;

namespace SG
{
	public class AIFrostGiantSoundFXManager : CharacterSoundFXManager
	{
		[Header("Hand Whooses")]
		public AudioClip[] handSlamWhooshes;

		[Header("Giant Impact Sounds")]
		public AudioClip[] groundSlam01Impacts;
		public AudioClip[] groundSlam02Impacts;
		public AudioClip[] jumpSlamImpacts;

		public void PlayGroundSlam01ImpactSFX()
		{
			if (groundSlam01Impacts.Length > 0)
				PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(groundSlam01Impacts));
		}

		public void PlayGroundSlam02ImpactSFX()
		{
			if (groundSlam02Impacts.Length > 0)
				PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(groundSlam02Impacts));
		}

		public void PlayJumpSlamImpactSFX()
		{
			if (jumpSlamImpacts.Length > 0)
				PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(jumpSlamImpacts));
		}
	}
}