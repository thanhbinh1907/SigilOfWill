using UnityEngine;

namespace SG
{
	public class ProjectileDamageCollider : DamageCollider
	{
		public float light_attack_Modifier;
		public float heavy_attack_Modifier;

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
