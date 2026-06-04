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

	}
}