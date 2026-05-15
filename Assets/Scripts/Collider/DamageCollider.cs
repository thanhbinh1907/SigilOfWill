using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Collider")]
        [SerializeField] protected Collider damageCollider;

		public CharacterManager characterCausingDamage;

		[Header("Damage")]
        public float physicalDamage = 0;
        public float fireDamage = 0;
        public float magicDamage = 0;
        public float lightningDamage = 0;
        public float windDamage = 0;
        public float holyDamage = 0;

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

            damageEffect.characterCausingDamage = characterCausingDamage;

			damageEffect.physicalDamage = physicalDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.windDamage = windDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contactPoint;

			damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

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

		protected virtual void OnDrawGizmos()
		{
			// Chỉ vẽ nếu được bật, có collider và collider đó đang hoạt động
			if (!showDebugGizmos || damageCollider == null || !damageCollider.enabled)
				return;

			Gizmos.color = new Color(1, 0, 0, 0.5f); // Màu đỏ trong suốt

			// Áp dụng Ma trận của Transform để khối debug xoay và tỉ lệ theo đúng vật thể
			Matrix4x4 oldMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;

			// Kiểm tra từng loại hình dáng cụ thể
			if (damageCollider is BoxCollider box)
			{
				// Vẽ hình hộp khớp với Size và Center của BoxCollider
				Gizmos.DrawCube(box.center, box.size);
			}
			else if (damageCollider is SphereCollider sphere)
			{
				// Vẽ hình cầu khớp với Radius và Center của SphereCollider
				Gizmos.DrawSphere(sphere.center, sphere.radius);
			}
			else if (damageCollider is CapsuleCollider capsule)
			{
				// Capsule không có hàm Draw sẵn, ta vẽ 2 đầu cầu để xác định phạm vi hitbox
				Vector3 pointOffset = Vector3.zero;
				float halfHeight = (capsule.height / 2f) - capsule.radius;

				// Xác định hướng của Capsule (0: X, 1: Y, 2: Z)
				if (capsule.direction == 0) pointOffset = Vector3.right * halfHeight;
				else if (capsule.direction == 1) pointOffset = Vector3.up * halfHeight;
				else if (capsule.direction == 2) pointOffset = Vector3.forward * halfHeight;

				Gizmos.DrawSphere(capsule.center + pointOffset, capsule.radius);
				Gizmos.DrawSphere(capsule.center - pointOffset, capsule.radius);

				// Vẽ đường nối giữa 2 đầu
				Gizmos.DrawLine(capsule.center + pointOffset, capsule.center - pointOffset);
			}

			// Trả lại matrix mặc định để không ảnh hưởng đến các Gizmos khác
			Gizmos.matrix = oldMatrix;
		}
	}
}