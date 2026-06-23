using UnityEngine;

namespace SG
{
	public class AIMeleeCombatManager : AICharacterCombatManager
	{
		[Header("Damage Colliders")]
		[SerializeField] MeleeWeaponDamageCollider meleeCollider;

		[Header("Damage Settings")]
		[SerializeField] int baseDamage = 30;
		[SerializeField] float lightAttackDamageModifier = 1.0f;
		[SerializeField] float heavyAttackDamageModifier = 1.5f;

		protected override void Awake()
		{
			base.Awake();
			if (meleeCollider == null)
			{
				meleeCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
			}
		}

		public void SetAttackDamage()
		{
			meleeCollider.characterCausingDamage = aiCharacter;
			meleeCollider.physicalDamage = baseDamage;

			meleeCollider.light_Attack_01_Modifier = lightAttackDamageModifier;
			meleeCollider.heavy_Attack_01_Modifier = heavyAttackDamageModifier;
		}

		public void OpenMeleeDamageCollider()
		{
			aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
			meleeCollider.EnableDamageCollider();
		}

		public void CloseMeleeDamageCollider()
		{
			meleeCollider.DisableDamageCollider();
		}
	}
}