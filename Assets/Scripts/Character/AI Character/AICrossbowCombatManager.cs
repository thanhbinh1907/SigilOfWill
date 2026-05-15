using UnityEngine;

namespace SG
{
    public class AICrossbowCombatManager : AICharacterCombatManager
    {
        [Header("Arrow Settings")]
        public GameObject arrowPrefab;
        public Transform arrowSpawnPoint;
		public float arrowVelocity = 10f;

		[Header("Damage Settings")]
		public float baseDamage = 25;
        public float light_Attack_Modifier = 1f;
        public float heavy_Attack_Modifier = 2f;

        protected override void Awake()
        {
            base.Awake();
		}

		public void ShootArrow()
		{
			if (arrowPrefab != null && arrowSpawnPoint != null)
			{
				// Hướng bắn mặc định
				Vector3 shootDirection = arrowSpawnPoint.transform.forward;

				// KIỂM TRA MỤC TIÊU VÀ LOCK ON TRANSFORM
				if (aiCharacter.characterCombatManager.currentTarget != null)
				{
					Vector3 targetPosition;

					// Nếu mục tiêu có Lock On Transform, ta lấy vị trí đó làm điểm ngắm
					if (aiCharacter.characterCombatManager.currentTarget.characterCombatManager.lockOnTransform != null)
					{
						targetPosition = aiCharacter.characterCombatManager.currentTarget.characterCombatManager.lockOnTransform.position;
					}
					else
					{
						// Nếu không có, nhắm vào tâm của transform mục tiêu (thường là dưới chân) và cộng thêm chiều cao
						targetPosition = aiCharacter.characterCombatManager.currentTarget.transform.position;
						targetPosition.y += 1.5f;
					}

					// Tính toán hướng từ đầu nỏ đến điểm Lock On
					shootDirection = (targetPosition - arrowSpawnPoint.position).normalized;
				}

				// Sinh ra mũi tên và xoay theo hướng nhắm chuẩn xác
				GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(shootDirection));

				// Thiết lập sát thương (giữ nguyên cấu trúc bạn muốn)
				ProjectileDamageCollider arrowDamageCollider = arrow.GetComponent<ProjectileDamageCollider>();
				if (arrowDamageCollider != null)
				{
					arrowDamageCollider.characterCausingDamage = aiCharacter;
					arrowDamageCollider.physicalDamage = baseDamage;
					arrowDamageCollider.light_attack_Modifier = light_Attack_Modifier;
					arrowDamageCollider.heavy_attack_Modifier = heavy_Attack_Modifier;
					arrowDamageCollider.EnableDamageCollider();
				}

				// Đẩy mũi tên đi theo hướng đã tính
				Rigidbody arrowRigidbody = arrow.GetComponent<Rigidbody>();
				if (arrowRigidbody != null)
				{
					arrowRigidbody.AddForce(shootDirection * arrowVelocity, ForceMode.Impulse);
				}
			}
		}
	}
}