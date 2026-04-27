using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
	public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
		public float staminaDamage;

		public override void ProcessEffect(CharacterManager character)
		{
			CalculateStaminaDamage(character);
		}

		private void CalculateStaminaDamage(CharacterManager character)
		{ 
			// COMPARED THE BASE STAMINA DAMAGE AGAINST OTHER PLAYER EFFECTS/MODIFIERS
			// CHANGE THE VALUE BEFORE SUBTRACTING IT FROM THE PLAYER'S STAMINA 
			// PLAY SFX OF VFX DURING EFFECT 

			character.currentStamina -= staminaDamage;
		}

	}
}