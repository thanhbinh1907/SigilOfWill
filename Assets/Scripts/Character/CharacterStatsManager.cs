using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class CharacterStatsManager : MonoBehaviour
	{
		CharacterManager character;

		[Header("Stamina Regeneration")]
		[SerializeField] float staminaRegenerationAmount = 2;
		private float staminaRegenerationTimer = 0;
		[SerializeField] float staminaRegenerationDelay = 2;

		protected virtual void Awake()
		{
			character = GetComponent<CharacterManager>();
		}

		public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
			float stamina = 0;

			// Create an equation for how you want stamina to be calculated

			stamina = endurance * 10;

			return Mathf.RoundToInt(stamina);
		}

		public virtual void RegenerateStamina()
		{
			if (character.isSprinting)
				return;

			if (character.isPerformingAction)
				return;

			staminaRegenerationTimer += Time.deltaTime;

			if (staminaRegenerationTimer >= staminaRegenerationDelay)
			{
				if (character.currentStamina < character.maxStamina)
				{
					staminaRegenerationTimer += Time.deltaTime;

					if (staminaRegenerationTimer >= staminaRegenerationDelay)
					{
						character.currentStamina += staminaRegenerationAmount * Time.deltaTime;
					}
				}
			}
		}

		public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
		{

			if (currentStaminaAmount < previousStaminaAmount)
			{
				staminaRegenerationTimer = 0;
			}
		}
	}
}
