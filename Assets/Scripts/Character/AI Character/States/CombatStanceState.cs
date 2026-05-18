using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

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
		public List<AICharacterAttackAction> aiCharacterAttacks;			// A list of all possible attacks this character can do
		protected List<AICharacterAttackAction> potentialAttacks;           // A list that is created duing this state, all attacks possible in this situation (base on angle, distance, etc)
		[SerializeField] protected AICharacterAttackAction choosenAttack; 
		[SerializeField] protected AICharacterAttackAction previousAttack;
		protected bool hasAttack = false;                                              	

		[Header("Combo")]
		[SerializeField] protected bool canPerformCombo = false;            // If character can perform a combo attack, after the initial attack
		[SerializeField] protected int chanceToPerformCombo = 25;           // The chance (in percentage) of the character to perform a combo on the next attack
		[SerializeField] bool hasRollForComboChance = false;                // If we have already rolled for the chance during this state 

		[Header("Engagement Distance")]
		[SerializeField] public float maximumEngagementDistance = 5;     // The distance we have to be away from the target before we enter the pursue state

		public override AIState Tick(AICharacterManager aiCharacter)
		{
			if (aiCharacter.isPerformingAction)
				return this;

			if (!aiCharacter.navMeshAgent.enabled)
				aiCharacter.navMeshAgent.enabled = true;

			// IF YOU WANT THE A.I CHARACTER TO FACE AND TURN TOWARDS ITS TARGET WHEN ITS OUTSIDE IT'S FOV INCLUDE THIS

			if (aiCharacter.aiCharacterCombatManager.enablePivot)
			{
				if (!aiCharacter.isMoving)
				{
					if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
					{
						aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
					}
				}
			}

			aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);


			// IF OUR TARGET IS NO LONGER PRESENT, SWITCH TO IDLE STATE
			if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
				return SwitchState(aiCharacter, aiCharacter.idle);

			if (!hasAttack)
			{
				GetNewAttack(aiCharacter);
			}
			else
			{
				aiCharacter.attack.currentAttack = choosenAttack;

				// ROLL FOR COMBO CHANCE

				return SwitchState(aiCharacter, aiCharacter.attack);
			}
			// IF WE ARE OUTSIDE OF THE COMBAT ENGAGEMENT DISTANCE, SWITCH TO PURSUE TARGET STATE
			if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
				return SwitchState(aiCharacter, aiCharacter.pursueTarget);

			NavMeshPath path = new NavMeshPath();
			aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
			aiCharacter.navMeshAgent.SetPath(path);

			return this;
		}

		protected virtual void GetNewAttack(AICharacterManager aiCharacter)
		{
			// 1. Sort through all possible attacks
			potentialAttacks = new List<AICharacterAttackAction>();

			// 2. Remove attacks that cant be used in this situation (based on distance, angle, etc)
			foreach (var potentialAttack in aiCharacterAttacks)
			{
				// IF THE ATTACK IS NOT IN RANGE, CONTINUE TO THE NEXT ONE
				if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget 
					|| potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
				{
					continue;
				}

				// IF ThE ATTACK IS NOT IN VIEWABLE ANGLE, CONTINUE TO THE NEXT ONE
				if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle
					|| potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
				{
					continue;
				}

				// 3. Place remaining attacks into a list
				potentialAttacks.Add(potentialAttack);
			}

			if (potentialAttacks.Count <= 0)
				return;

			var totalWeight = 0;

			foreach (var attack in potentialAttacks)
			{
				totalWeight += attack.attackWeight;
			}

			// 4. Pick one of the remaining attacks randomly, base on weight 
			var randomValue = Random.Range(1, totalWeight + 1);
			var progressWeight = 0;

			foreach (var attack in potentialAttacks)
			{
				progressWeight += attack.attackWeight;

				if (randomValue <= progressWeight)
				{

					// 5. Select this attack and pass it to attack state
					choosenAttack = attack;
					previousAttack = choosenAttack;
					hasAttack = true;
					return;
				}
			}
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

			hasAttack = false;
			hasRollForComboChance = false;
		}
	}
}