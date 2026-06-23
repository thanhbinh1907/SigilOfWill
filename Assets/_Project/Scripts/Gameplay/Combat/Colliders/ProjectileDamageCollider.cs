using UnityEngine;

namespace SG
{
	public class ProjectileDamageCollider : DamageCollider
	{
		public float light_attack_Modifier;
		public float heavy_attack_Modifier;

		[Header("Projectile Collision Settings")]
		public GameObject explodeVFX;
		public float explodeVFXDestroyTime = 2f;

		protected override void OnTriggerEnter(Collider other)
		{
			CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
			if (damageTarget != null && damageTarget == characterCausingDamage)
			{
				return;
			}

			if (damageTarget != null && characterCausingDamage != null)
			{
				if (!WorldUtilityManager.instance.CanIDamageThisTarget(characterCausingDamage.characterGroup, damageTarget.characterGroup))
				{
					return;
				}
			}

			if (other.isTrigger)
			{
				return;
			}

			contactPoint = other.ClosestPointOnBounds(transform.position);

			if (damageTarget != null)
			{
				if (damageTarget.isInvulnerable)
				{
					return;
				}

				DamageTarget(damageTarget);
			}

			if (explodeVFX != null)
			{
				GameObject explosion = Instantiate(explodeVFX, contactPoint, Quaternion.identity);
				Destroy(explosion, explodeVFXDestroyTime);
			}

			Destroy(transform.root.gameObject);
		}

		protected override void DamageTarget(CharacterManager damageTarget)
		{
			if (charactersDamaged.Contains(damageTarget))
				return;

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

			switch (characterCausingDamage.characterCombatManager.currentAttackType)
			{
				case AttackType.LightAttack01:
					ApplyAttackDamageModifiers(light_attack_Modifier, damageEffect);
					break;
				case AttackType.HeavyAttack01:
					ApplyAttackDamageModifiers(heavy_attack_Modifier, damageEffect);
					break;
				default:
					break;
			}

			damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
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
