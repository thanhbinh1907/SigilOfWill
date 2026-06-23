using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

namespace SG
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {

		[Header("Weapon Attack Modifiers")]
		public float light_Attack_01_Modifier;
		public float heavy_Attack_01_Modifier;

		protected override void Awake()
		{
			base.Awake();

			if (damageCollider == null)
			{
				damageCollider = GetComponent<Collider>();
			}

			damageCollider.enabled = false;
		}

		protected override void OnTriggerEnter(Collider other)
		{
			CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

			if (damageTarget != null)
			{
				if (damageTarget == characterCausingDamage)
					return;

				try
				{
					contactPoint = other.ClosestPointOnBounds(transform.position);
				}
				catch
				{
					contactPoint = other.bounds.ClosestPoint(transform.position);
				}

				if (characterCausingDamage != null && WorldUtilityManager.instance != null)
				{
					if (!WorldUtilityManager.instance.CanIDamageThisTarget(characterCausingDamage.characterGroup, damageTarget.characterGroup))
						return;
				}

				if (damageTarget.isInvulnerable)
					return;

				DamageTarget(damageTarget);
			}
		}

		protected override void DamageTarget(CharacterManager damageTarget)
		{
			if (charactersDamaged.Contains(damageTarget))
				return;

			if (characterCausingDamage == null)
			{
				Debug.LogWarning("characterCausingDamage is null on MeleeWeaponDamageCollider: " + name);
				return;
			}

			if (WorldCharacterEffectsManager.instance == null || WorldCharacterEffectsManager.instance.takeDamageEffect == null)
			{
				Debug.LogError("WorldCharacterEffectsManager or takeDamageEffect is null on MeleeWeaponDamageCollider: " + name);
				return;
			}

			charactersDamaged.Add(damageTarget);

			TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

			damageEffect.characterCausingDamage = characterCausingDamage;

			damageEffect.physicalDamage = physicalDamage;
			damageEffect.fireDamage = fireDamage;
			damageEffect.magicDamage = magicDamage;
			damageEffect.lightningDamage = lightningDamage;
			damageEffect.windDamage = windDamage;
			damageEffect.holyDamage = holyDamage;
			damageEffect.contactPoint = contactPoint;
			damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

			if (characterCausingDamage.characterCombatManager != null)
			{
				switch (characterCausingDamage.characterCombatManager.currentAttackType)
				{
					case AttackType.LightAttack01:
						ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
						break;
					case AttackType.HeavyAttack01:
						ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
						break;
					default:
						break;
				}
			}

			if (damageTarget.characterEffectsManager != null)
			{
				damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
			}
		}

		private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
		{
			damage.physicalDamage *= modifier;
			damage.fireDamage *= modifier;
			damage.magicDamage *= modifier;
			damage.lightningDamage *= modifier;
			damage.windDamage *= modifier;
			damage.holyDamage *= modifier;
			damage.poiseDamage *= modifier;
		}
	}
}