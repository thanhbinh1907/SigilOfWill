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
	}
}