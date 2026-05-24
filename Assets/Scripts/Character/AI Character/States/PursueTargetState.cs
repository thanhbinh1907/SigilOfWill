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
			// CHECK IF WE ARE PERFORMING AN ACTION (IF SO DO NOTHING UNTIL ACTION IS COMPLETE)
			if (aiCharacter.isPerformingAction)
				return this;

			// CHECK IF OUR TARGET IS NULL, IF WE DO NOT HAVE A TARGET, RETURN IDLE STATE 
			if (aiCharacter.characterCombatManager.currentTarget == null)
				return SwitchState(aiCharacter, aiCharacter.idle);

			// MAKE SURE OUR NAVMESH AGENT IS ACTIVE, IF IT NOT ENABLE IT
			if (!aiCharacter.navMeshAgent.enabled)
				aiCharacter.navMeshAgent.enabled = true;

			// IF OUR TARGET GOES OUTSIDE OF THE CHARACTER FOV, PIVOT TO FACE THEM
			if (aiCharacter.aiCharacterCombatManager.enablePivot)
			{
				if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimunFOV
					|| aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
					aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
			}

			// ROTATE THE AI CHARACTER TOWARDS THE TARGET
			aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);


			// IF WE ARE WITHIN COMBAT RANGE OF A TARGET, SWITCH STATE TO COMBAT STANCE STATE
			// OPTION 1 (Sử dụng Khoảng cách giao tranh tối đa của Combat Stance)
			if (aiCharacter.combatStance != null && aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.combatStance.maximumEngagementDistance)
				return SwitchState(aiCharacter, aiCharacter.combatStance);
			
			// OPTION 2 (Sử dụng stoppingDistance của NavMeshAgent)
			// if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
			// 	return SwitchState(aiCharacter, aiCharacter.combatStance);


			// IF THE TARGET IS NOT REACHABLE,  AN THEY FAR AWAY, RETURN HOME


			// PURSUE THE TARGET


			// OPTION 1
			//aiCharacter.navMeshAgent.SetDestination(aiCharacter.characterCombatManager.currentTarget.transform.position);

			// OPTION 2
			NavMeshPath path = new NavMeshPath();
			aiCharacter.navMeshAgent.CalculatePath(aiCharacter.characterCombatManager.currentTarget.transform.position, path);
			aiCharacter.navMeshAgent.SetPath(path);

			return this;
		}
	}
}