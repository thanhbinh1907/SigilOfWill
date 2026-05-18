using UnityEngine;

namespace SG
{
	public class FrostGiantDamageCollider : DamageCollider
	{
		[SerializeField] AIBossCharacterManager bossCharacter;

		protected override void Awake()
		{
			base.Awake();

			damageCollider = GetComponent<Collider>();
			bossCharacter = GetComponentInParent<AIBossCharacterManager>();
		}

		protected override void DamageTarget(CharacterManager damageTarget)
		{
			base.DamageTarget(damageTarget);

			// WE DONT WANT TO DAMAGE THE SAME TARGET MORE THAN ONCE PER ATTACK
			// SO WE ADD THEM TO A LIST THAT CHECKS BEFORE APPLYING DAMAGE
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

			damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
		}
	}
}