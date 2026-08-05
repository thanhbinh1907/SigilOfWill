using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class DamageCollider : DamageDealer
    {
        [Header("Collider")]
        [SerializeField] protected Collider damageCollider;

		[Header("Debug")]
		[SerializeField] bool showDebugGizmos = true;

		[Header("Contact Point")]
        public Vector3 contactPoint;

        [Header("Character Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

		protected virtual void Awake()
		{
			damageCollider = GetComponent<Collider>();
			damageCollider.enabled = false;
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                if (damageTarget == characterCausingDamage)
                    return;

				try
				{
					contactPoint = other.ClosestPointOnBounds(transform.position);
				}
				catch
				{
					contactPoint = other.bounds.ClosestPoint(transform.position);
				}

				// CHECK IF WE CAN DAMAGE THIS TARGET BASED ON FRIENDLY FIRE SETTINGS
				if (characterCausingDamage != null && WorldUtilityManager.instance != null)
				{
					if (!WorldUtilityManager.instance.CanIDamageThisTarget(characterCausingDamage.characterGroup, damageTarget.characterGroup))
						return;
				}

				// CHECK IF TARGET IS BLOCKING

				// CHECK IF TARGET IS INVULNERABLE
				if (damageTarget.isInvulnerable)
					return;

				// DAMAGE

				DamageTarget(damageTarget);
			}
		}

        protected virtual void DamageTarget(CharacterManager damageTarget)
        {
            // WE DONT WANT TO DAMAGE THE SAME TARGET MORE THAN ONCE PER ATTACK
            // SO WE ADD THEM TO A LIST THAT CHECKS BEFORE APPLYING DAMAGE
            if (charactersDamaged.Contains(damageTarget))
                return;

            charactersDamaged.Add(damageTarget);

            ApplyDamage(damageTarget, contactPoint);
		}

        public virtual void EnableDamageCollider()
        {
            damageCollider.enabled = true;
		}

        public virtual void DisableDamageCollider()
        {
            damageCollider.enabled = false;
            charactersDamaged.Clear();
		}

		protected virtual void OnDrawGizmos()
		{

			if (!showDebugGizmos || damageCollider == null || !damageCollider.enabled)
				return;

			Gizmos.color = new Color(1, 0, 0, 0.5f);


			Matrix4x4 oldMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;


			if (damageCollider is BoxCollider box)
			{

				Gizmos.DrawCube(box.center, box.size);
			}
			else if (damageCollider is SphereCollider sphere)
			{

				Gizmos.DrawSphere(sphere.center, sphere.radius);
			}
			else if (damageCollider is CapsuleCollider capsule)
			{

				Vector3 pointOffset = Vector3.zero;
				float halfHeight = (capsule.height / 2f) - capsule.radius;


				if (capsule.direction == 0) pointOffset = Vector3.right * halfHeight;
				else if (capsule.direction == 1) pointOffset = Vector3.up * halfHeight;
				else if (capsule.direction == 2) pointOffset = Vector3.forward * halfHeight;

				Gizmos.DrawSphere(capsule.center + pointOffset, capsule.radius);
				Gizmos.DrawSphere(capsule.center - pointOffset, capsule.radius);


				Gizmos.DrawLine(capsule.center + pointOffset, capsule.center - pointOffset);
			}


			Gizmos.matrix = oldMatrix;
		}
	}
}