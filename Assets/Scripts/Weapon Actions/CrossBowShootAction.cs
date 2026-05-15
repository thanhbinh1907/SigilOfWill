using UnityEngine;

namespace SG
{
	[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Crossbow/Crossbow Shoot Action")]
	public class CrossBowShootAction : WeaponItemAction
    {
        [Header("Shoot Type")]
        public bool isHeavyAttack = false;

        [Header("Animations")]
        [SerializeField] string light_shoot_animation = "Crossbow_Shoot_Light_01";
        [SerializeField] string heavy_shoot_animation = "Crossbow_Shoot_Heavy_01";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            if (playerPerformingAction.currentStamina <= 0)
                return;

            if (!playerPerformingAction.isGrounded)
                return;

            if (!playerPerformingAction.characterCombatManager.isCrossbowLoaded)
                return;

            if (!playerPerformingAction.characterCombatManager.isCrossbowLoaded)
                return;

			PerformShoot(playerPerformingAction, weaponPerformingAction);   
		}

        private void PerformShoot(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (isHeavyAttack)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAnimation(heavy_shoot_animation, true);
            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAnimation(light_shoot_animation, true);
            }

            playerPerformingAction.characterCombatManager.isCrossbowLoaded = false;
		}
	}
}