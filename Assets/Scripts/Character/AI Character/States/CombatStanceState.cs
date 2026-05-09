using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	[CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
	public class CombatStanceState : AIState
	{
		// 1. Select an attack for the attack state, depending on the distance and the angle to the target.
		// 2. Process any combat logic here whilst waiting to attack (blocking, strafing, dodging ...)
		// 3. If target is out of range, switch to pursue state
		// 4. If target is no longer present, switch to idle state

		[Header("Attack")]
		public List<AICharacterAttackAction> aiCHaracterAttacks;			// A list of all possible attacks this character can do
		protected List<AICharacterAttackAction> potentialAttacks;           // A list that is created duing this state, all attacks possible in this situation (base on angle, distance, etc)

		[Header("Combo")]
		[SerializeField] protected bool canPerformCombo = false;            // If character can perform a combo attack, after the initial attack
		[SerializeField] protected int chanceToPerformCombo = 25;           // The chance (in percentage) of the character to perform a combo on the next attack
		[SerializeField] bool hasRollForComboChance = false;				// If we have already rolled for the chance during this state 

		[Header("Engagement Distance")]
		[SerializeField] protected float maximumEngagementDistance = 5;     // The distance we have to be away from the target before we enter the pursue state

		public override AIState Tick(AICharacterManager aiCharacter)
		{
			return this;
		}

		protected virtual void GetNewAttack(AICharacterManager aiCharacter)
		{
			// 1. Sort through all possible attacks
			// 2. Remove attacks that cant be used in this situation (based on distance, angle, etc)
			// 3. Place remaining attacks into a list
			// 4. Pick one of the remaining attacks randomly, base on weight 
			// 5. Select this attack and pass it to attack state


		}

		protected virtual bool RollForOutcomeChance(int outcomeChance)
		{
			bool outcomeWillBePerform = false;

			int randomPercentage = Random.Range(0, 100);

			if (randomPercentage < outcomeChance)
			{
				outcomeWillBePerform = true;
			}

			return outcomeWillBePerform;
		}

		protected override void ResetStateFlags(AICharacterManager aiCharacter)
		{
			base.ResetStateFlags(aiCharacter);

			hasRollForComboChance = false;
		}
	}
}