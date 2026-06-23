using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace SG
{
	[CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
	public class CombatStanceState : AIState
	{
		[Header("Attack")]
		public List<AICharacterAttackAction> aiCharacterAttacks;
		protected List<AICharacterAttackAction> potentialAttacks;
		[SerializeField] protected AICharacterAttackAction choosenAttack;
		[SerializeField] protected AICharacterAttackAction previousAttack;
		protected bool hasAttack = false;

		[Header("Combo")]
		[SerializeField] protected bool canPerformCombo = false;
		[SerializeField] protected int chanceToPerformCombo = 25;
		//[SerializeField] bool hasRollForComboChance = false;

		[Header("Engagement Distance")]
		[SerializeField] public float maximumEngagementDistance = 5;

		public override AIState Tick(AICharacterManager aiCharacter)
		{
			if (aiCharacter.isPerformingAction)
				return this;

			if (aiCharacter.navMeshAgent != null)
			{
				if (!aiCharacter.navMeshAgent.enabled || !aiCharacter.navMeshAgent.isOnNavMesh)
				{
					NavMeshHit hit;
					if (NavMesh.SamplePosition(aiCharacter.transform.position, out hit, 10.0f, NavMesh.AllAreas))
					{
						Debug.Log($">> [COMBAT STANCE] {aiCharacter.name} snapping off-mesh Agent to {hit.position}");
						aiCharacter.navMeshAgent.enabled = false;
						aiCharacter.transform.position = hit.position;
						aiCharacter.navMeshAgent.enabled = true;
					}
					else
					{
						Debug.LogWarning($">> [COMBAT STANCE] {aiCharacter.name} could not snap Agent near 10m! Disabling agent.");
						aiCharacter.navMeshAgent.enabled = false;
					}
				}
			}

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
			else
			{
				if (!aiCharacter.isMoving)
				{
					aiCharacter.aiCharacterCombatManager.RotateTowardsTarget(aiCharacter);
				}
			}

			aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

			if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
				return SwitchState(aiCharacter, aiCharacter.idle);

			if (!hasAttack)
			{
				GetNewAttack(aiCharacter);
			}
			else
			{
				aiCharacter.attack.currentAttack = choosenAttack;

				return SwitchState(aiCharacter, aiCharacter.attack);
			}
			if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
				return SwitchState(aiCharacter, aiCharacter.pursueTarget);

			if (aiCharacter.navMeshAgent != null && aiCharacter.navMeshAgent.isActiveAndEnabled && aiCharacter.navMeshAgent.isOnNavMesh)
			{
				Debug.Log($">> [COMBAT STANCE] {aiCharacter.name} calculating path to target: {aiCharacter.aiCharacterCombatManager.currentTarget.transform.position}");
				NavMeshPath path = new NavMeshPath();
				aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
				aiCharacter.navMeshAgent.SetPath(path);
			}

			return this;
		}

		protected virtual void GetNewAttack(AICharacterManager aiCharacter)
		{
			potentialAttacks = new List<AICharacterAttackAction>();

			foreach (var potentialAttack in aiCharacterAttacks)
			{
				if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget
					|| potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
				{
					continue;
				}

				if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle
					|| potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
				{
					continue;
				}
				potentialAttacks.Add(potentialAttack);
			}

			if (potentialAttacks.Count <= 0)
				return;

			var totalWeight = 0;

			foreach (var attack in potentialAttacks)
			{
				totalWeight += attack.attackWeight;
			}

			var randomValue = Random.Range(1, totalWeight + 1);
			var progressWeight = 0;

			foreach (var attack in potentialAttacks)
			{
				progressWeight += attack.attackWeight;

				if (randomValue <= progressWeight)
				{
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
			//hasRollForComboChance = false;
		}
	}
}