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

		protected virtual void Awake()
		{
			character = GetComponent<CharacterManager>();
		}

		public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
		{
			effect.ProcessEffect(character);
		}
	}
}