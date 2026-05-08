using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	[CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
	public class CombatStanceState : AIState
	{
		public override AIState Tick(AICharacterManager aiCharacter)
		{
			if (aiCharacter.isPerformingAction)
				return this;

			if (aiCharacter.characterCombatManager.currentTarget == null)
				return SwitchState(aiCharacter, aiCharacter.idle);

			float distanceToTarget = Vector3.Distance(aiCharacter.transform.position, aiCharacter.characterCombatManager.currentTarget.transform.position);

			if (distanceToTarget > aiCharacter.navMeshAgent.stoppingDistance)
			{
				return SwitchState(aiCharacter, aiCharacter.pursueTarget);
			}

			return this;
		}
    }
}