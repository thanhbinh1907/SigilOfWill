using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace SG
{
	[CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
	public class PursueTargetState : AIState
    {
		public override AIState Tick(AICharacterManager aiCharacter)
        {
			if (aiCharacter.isPerformingAction)
				return this;

			if (aiCharacter.characterCombatManager.currentTarget == null)
				return SwitchState(aiCharacter, aiCharacter.idle);

			if (aiCharacter.navMeshAgent != null)
			{
				if (!aiCharacter.navMeshAgent.enabled || !aiCharacter.navMeshAgent.isOnNavMesh)
				{
					NavMeshHit hit;
					if (NavMesh.SamplePosition(aiCharacter.transform.position, out hit, 10.0f, NavMesh.AllAreas))
					{
						Debug.Log($">> [PURSUE] {aiCharacter.name} snapping off-mesh Agent to {hit.position}");
						aiCharacter.navMeshAgent.enabled = false;
						aiCharacter.transform.position = hit.position;
						aiCharacter.navMeshAgent.enabled = true;
					}
					else
					{
						Debug.LogWarning($">> [PURSUE] {aiCharacter.name} could not snap Agent near 10m! Disabling agent.");
						aiCharacter.navMeshAgent.enabled = false;
					}
				}
			}

			if (aiCharacter.aiCharacterCombatManager.enablePivot)
			{
				if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimunFOV
					|| aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
					aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
			}

			aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

			if (aiCharacter.combatStance != null && aiCharacter.aiCharacterCombatManager.distanceFromTarget <=
				aiCharacter.combatStance.maximumEngagementDistance)
				return SwitchState(aiCharacter, aiCharacter.combatStance);

			if (aiCharacter.navMeshAgent != null && aiCharacter.navMeshAgent.isActiveAndEnabled && aiCharacter.navMeshAgent.isOnNavMesh)
			{
				Debug.Log($">> [PURSUE] {aiCharacter.name} calculating path to target: {aiCharacter.characterCombatManager.currentTarget.transform.position}");
				NavMeshPath path = new NavMeshPath();
				aiCharacter.navMeshAgent.CalculatePath(aiCharacter.characterCombatManager.currentTarget.transform.position, path);
				aiCharacter.navMeshAgent.SetPath(path);
			}

			return this;
		}
	}
}