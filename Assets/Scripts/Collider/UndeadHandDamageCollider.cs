using UnityEngine;

namespace SG
{
    public class UndeadHandDamageCollider : DamageCollider
    {
		[SerializeField] AICharacterManager undeadCharacter;

		protected override void Awake()
		{
			base.Awake();
			
			damageCollider = GetComponent<Collider>();
			undeadCharacter = GetComponentInParent<AICharacterManager>();
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

			damageEffect.angleHitFrom = Vector3.SignedAngle(undeadCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

			damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
		}
	}
}