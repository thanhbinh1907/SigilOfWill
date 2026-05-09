using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class AICharacterAttackAction : MonoBehaviour
    {
		[Header("Attack")]
		[SerializeField] private string attackAnimation;

		[Header("Combo Action")]
		public AICharacterAttackAction comboAction;         // The combo action of this attack action

		[Header("Action Value")]
		public int attackWeight = 50;
		// [SerializeField] AttackType attackType;
		// ATTACK CAN BE  REPEATED
		public float actionRecoveryTime = 2;                // The time before the character can make another attack after performing this one
		public float minimumAttackAngle = -35;                
		public float maximumAttackAngle = 35;
		public float minimumAtackDistance = 0;
		public float maximumAttackDistance = 2;

		public void AttempToPerformAction(AICharacterManager aiCharacter)
        {
			//aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackAnimation, true);
		}
	}
}