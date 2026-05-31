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
				Vector3 shootDirection = arrowSpawnPoint.transform.forward;

				if (aiCharacter.characterCombatManager.currentTarget != null)
				{
					Vector3 targetPosition;

					if (aiCharacter.characterCombatManager.currentTarget.characterCombatManager.lockOnTransform != null)
					{
						targetPosition = aiCharacter.characterCombatManager.currentTarget.characterCombatManager.lockOnTransform.position;
					}
					else
					{
						targetPosition = aiCharacter.characterCombatManager.currentTarget.transform.position;
						targetPosition.y += 1.5f;
					}

					shootDirection = (targetPosition - arrowSpawnPoint.position).normalized;
				}

				GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(shootDirection));

				ProjectileDamageCollider arrowDamageCollider = arrow.GetComponent<ProjectileDamageCollider>();
				if (arrowDamageCollider != null)
				{
					arrowDamageCollider.characterCausingDamage = aiCharacter;
					arrowDamageCollider.physicalDamage = baseDamage;
					arrowDamageCollider.light_attack_Modifier = light_Attack_Modifier;
					arrowDamageCollider.heavy_attack_Modifier = heavy_Attack_Modifier;
					arrowDamageCollider.EnableDamageCollider();
				}

				Rigidbody arrowRigidbody = arrow.GetComponent<Rigidbody>();
				if (arrowRigidbody != null)
				{
					arrowRigidbody.AddForce(shootDirection * arrowVelocity, ForceMode.Impulse);
				}
			}
		}
	}
}