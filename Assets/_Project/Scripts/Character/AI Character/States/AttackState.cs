using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	[CreateAssetMenu(menuName = "A.I/States/Attack")]
	public class AttackState : AIState
    {
        [HideInInspector] public AICharacterAttackAction currentAttack;
        [HideInInspector] public bool willPerformCombo = false;

        [Header("State Flags")]
        protected bool hasPerformAttack = false;
        protected bool hasPerformedCombo = false;

        [Header("Pivot After Attack")]
        [SerializeField] protected bool pivotAfterAttack = false;

		public override AIState Tick(AICharacterManager aiCharacter)
		{
			if (aiCharacter.characterCombatManager.currentTarget == null) 
                return SwitchState(aiCharacter, aiCharacter.idle);

            if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead)
                return SwitchState(aiCharacter, aiCharacter.idle);


			aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhilistAttacking(aiCharacter);

			aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);

			if (willPerformCombo && !hasPerformedCombo)
            {
                if (currentAttack.comboAction != null)
                {
                    // IF CAN COMBO 
                    //hasPerformedCombo = true;
                    //currentAttack.comboAction.AttempToPerformAction(aiCharacter);
				}
			}

			if (aiCharacter.isPerformingAction)
				return this;

			if (!hasPerformAttack)
            {
				if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
                    return this;

                PerformAttack(aiCharacter);

                return this;
			}

            if (pivotAfterAttack)
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);

            return SwitchState(aiCharacter, aiCharacter.combatStance);
		}

        protected void PerformAttack(AICharacterManager aiCharacter)
        {
			hasPerformAttack = true;

			currentAttack.AttempToPerformAction(aiCharacter);
            aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
		}

		protected override void ResetStateFlags(AICharacterManager aiCharacter)
		{
			base.ResetStateFlags(aiCharacter);

            hasPerformAttack = false;
            hasPerformedCombo = false;
		}
	}
}