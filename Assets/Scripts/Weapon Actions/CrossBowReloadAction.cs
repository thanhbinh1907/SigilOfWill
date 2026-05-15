using SG;
using UnityEngine;

namespace SG
{
	[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Crossbow/Crossbow Reload Action")]
	public class CrossBowReloadAction : WeaponItemAction
	{
		[SerializeField] string reloadAnimation = "Crossbow_Reload_01";

		public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
		{
			base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
			
			if (playerPerformingAction.currentStamina <= 0)
				return;

			if (!playerPerformingAction.isGrounded)
				return;

			if (playerPerformingAction.characterCombatManager.isCrossbowLoaded)
				return;

			PerformReload(playerPerformingAction, weaponPerformingAction);
		}

		private void PerformReload(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
		{
			playerPerformingAction.playerAnimatorManager.PlayTargetAnimation(reloadAnimation, true);
			playerPerformingAction.characterCombatManager.isCrossbowLoaded = true;
		}
	}
}