using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
	public class LightAttackWeaponAction : WeaponItemAction
    {
		[SerializeField] string light_attack_01 = "Main_Light_Attack_01";

		public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
		{
			base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

			// CHECK FOR STOP

			if (playerPerformingAction.currentStamina <= 0)
				return;

			if (!playerPerformingAction.isGrounded)
				return; 

			PerformLightAttack(playerPerformingAction, weaponPerformingAction);
		}

		private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
		{
			if (playerPerformingAction.isUsingRightHand)
			{
				playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(light_attack_01, true);
			}
			if (playerPerformingAction.isUsingLeftHand)
			{

			}
		}
    }
}