using UnityEngine;

namespace SG
{
	public class UnarmedDamageCollider : DamageCollider
	{
		protected override void DamageTarget(CharacterManager damageTarget)
		{
			base.DamageTarget(damageTarget);
		}
	}
}