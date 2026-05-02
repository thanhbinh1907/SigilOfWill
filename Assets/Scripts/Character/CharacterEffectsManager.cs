using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class CharacterEffectsManager : MonoBehaviour
    {
		// PROCESS INSTANT EFFECTS (TAKE DAMAGE, HEAL)

		// PROCESS TIMED EFFECTS (POISON, BUILD UP)

		// PROCESS STATIC EFFECTS (BUFFS, DEBUFFS)
		CharacterManager character;

		[Header("VFX")]
		[SerializeField] GameObject bloodSplatterVFX;

		protected virtual void Awake()
		{
			character = GetComponent<CharacterManager>();
		}

		public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
		{
			effect.ProcessEffect(character);
		}

		public void PlayeBloodSplatterVFX(Vector3 contactPoint)
		{
			if (bloodSplatterVFX != null)
			{
				GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
			}
			else
			{
				GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
			}
		}
	}
}