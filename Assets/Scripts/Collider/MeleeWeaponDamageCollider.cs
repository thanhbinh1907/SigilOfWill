using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage;

		protected override void DamageTarget(CharacterManager damageTarget)
		{
			base.DamageTarget(damageTarget);
		}

		protected override void CalculateDamageAngle(TakeDamageEffect damageEffect, CharacterManager damageTarget)
		{
			base.CalculateDamageAngle(damageEffect, damageTarget);

			damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);
		}
	}
}