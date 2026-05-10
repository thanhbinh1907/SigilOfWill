using UnityEngine;

namespace SG
{
	public class ProjectileDamageCollider : DamageCollider
	{
		protected override void DamageTarget(CharacterManager damageTarget)
		{
			base.DamageTarget(damageTarget);
		}
	}
}
