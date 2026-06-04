using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace SG
{
    public class CharacterStatsManager : MonoBehaviour
	{
		CharacterManager character;

		[Header("Stamina Regeneration")]
		[SerializeField] float staminaRegenerationAmount = 2;
		private float staminaRegenerationTimer = 0;
		[SerializeField] float staminaRegenerationDelay = 2;

		[Header("Mana Regeneration")]
		[SerializeField] float manaRegenerationAmount = 10;
		private float manaRegenerationTimer = 0;
		[SerializeField] float manaRegenerationDelay = 2;

		protected virtual void Awake()
		{
			character = GetComponent<CharacterManager>();
		}

		protected virtual void Start()
		{

		}

		public int CalculateHealthBasedOnVitalityLevel(int vitality)
		{
			float health = 0;

			// Create an equation for how you want health to be calculated

			health = vitality * 15;

			return Mathf.RoundToInt(health);
		}

		public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
			float stamina = 0;

			// Create an equation for how you want stamina to be calculated

			stamina = endurance * 10;

			return Mathf.RoundToInt(stamina);
		}

		public int CalculateManaBasedOnIntelligenceLevel(int intelligence)
		{
			float mana = 0;
			
			mana = intelligence * 10;
			return Mathf.RoundToInt(mana);
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
					character.currentStamina += staminaRegenerationAmount * Time.deltaTime;
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

		public virtual void RegenerateMana()
		{
			if (character.isPerformingAction)
				return;
			manaRegenerationTimer += Time.deltaTime;

			if (manaRegenerationTimer >= manaRegenerationDelay)
			{
				if (character.currentMana < character.maxMana)
				{
					character.currentMana += manaRegenerationAmount * Time.deltaTime;
				}
			}
		}

		public virtual void ResetManaRegenTimer(float previousManaAmount, float currentManaAmount)
		{
			if (currentManaAmount < previousManaAmount)
			{
				manaRegenerationTimer = 0;
			}
		}

	}
}
