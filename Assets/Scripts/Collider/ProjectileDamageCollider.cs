using UnityEngine;

namespace SG
{
	public class ProjectileDamageCollider : DamageCollider
	{
		protected override void DamageTarget(CharacterManager damageTarget)
		{
			base.DamageTarget(damageTarget);
		}

		protected override void CalculateDamageAngle(TakeDamageEffect damageEffect, CharacterManager damageTarget)
		{
			base.CalculateDamageAngle(damageEffect, damageTarget);

			damageEffect.angleHitFrom = Vector3.SignedAngle(transform.forward, damageTarget.transform.forward, Vector3.up);
		}
	}
}
