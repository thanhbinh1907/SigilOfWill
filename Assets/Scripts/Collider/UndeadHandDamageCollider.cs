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

	}
}