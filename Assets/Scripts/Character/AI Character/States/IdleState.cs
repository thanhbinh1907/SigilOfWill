using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Idle")]
	public class IdleState : AIState
    {
		public override AIState Tick(AICharacterManager aiCharacter)
		{
			if (aiCharacter.characterCombatManager.currentTarget != null)
			{
				// RETURN THE PURSUE TARGET STATE
				return SwitchState(aiCharacter, aiCharacter.pursueTarget);
			}
			else
			{
				// RETURN THIS STATE, TO CONTINUALLY SEARCH FOR A TARGET
				aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);
				return this;
			}
		}
    }
}