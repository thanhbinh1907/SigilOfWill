using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Collider")]
        protected Collider damageCollider;

		[Header("Damage")]
        public float physicalDamage = 0;
        public float fireDamage = 0;
        public float magicDamage = 0;
        public float lightningDamage = 0;
        public float windDamage = 0;
        public float holyDamage = 0;

        [Header("Contact Point")]
        private Vector3 contactPoint;

        [Header("Character Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

		private void OnTriggerEnter(Collider other)
		{
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

				// CHECK IF WE CAN DAMAGE THIS TARGET BASED ON FRIENDLY FIRE SETTINGS

				// CHECK IF TARGET IS BLOCKING

				// CHECK IF TARGET IS INVULNERABLE 

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

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.windDamage = windDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contactPoint;

            damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
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
	}
}