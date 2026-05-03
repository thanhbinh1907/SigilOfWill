using UnityEngine;

namespace SG
{
	public class UnarmedDamageCollider : DamageCollider
	{
		[Header("Golem Body Part")]
		public CharacterManager golemManager;

		protected override void DamageTarget(CharacterManager damageTarget)
		{
			base.DamageTarget(damageTarget);
		}

		protected override void CalculateDamageAngle(TakeDamageEffect damageEffect, CharacterManager damageTarget)
		{
			base.CalculateDamageAngle(damageEffect, damageTarget);

			damageEffect.angleHitFrom = Vector3.SignedAngle(transform.root.forward, damageTarget.transform.forward, Vector3.up);
		}
	}
}