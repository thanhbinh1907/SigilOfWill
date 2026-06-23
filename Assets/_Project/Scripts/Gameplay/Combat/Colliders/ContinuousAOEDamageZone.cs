using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class ContinuousAOEDamageZone : DamageDealer
	{
		[Header("AOE Settings")]
		public float radius = 5f;
		public float damageInterval = 0.5f;
		public float duration = 5f;

		[Header("VFX Settings")]
		[SerializeField] private bool showDebugGizmos = true;
		public GameObject impactVFX;
		public float impactVFXDestroyTime = 2f;

		private void Start()
		{

			StartCoroutine(ApplyContinuousDamage());


			Destroy(gameObject, duration);
		}

		private IEnumerator ApplyContinuousDamage()
		{
			float elapsed = 0f;

			while (elapsed < duration)
			{
				ExecuteAOEDamage();
				yield return new WaitForSeconds(damageInterval);
				elapsed += damageInterval;
			}
		}

		private void ExecuteAOEDamage()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, radius, WorldUtilityManager.instance.GetCharacterLayers());
			List<CharacterManager> damagedCharacters = new List<CharacterManager>();

			foreach (var collider in colliders)
			{
				CharacterManager targetCharacter = collider.GetComponentInParent<CharacterManager>();

				if (targetCharacter != null)
				{

					if (targetCharacter == characterCausingDamage)
						continue;

					if (characterCausingDamage != null)
					{
						if (!WorldUtilityManager.instance.CanIDamageThisTarget(characterCausingDamage.characterGroup, targetCharacter.characterGroup))
							continue;
					}

					if (!damagedCharacters.Contains(targetCharacter))
					{
						damagedCharacters.Add(targetCharacter);

						if (targetCharacter.isInvulnerable)
							continue;


						ApplyDamage(targetCharacter, targetCharacter.transform.position);


						if (impactVFX != null)
						{
							Vector3 spawnPos = targetCharacter.transform.position;

							if (targetCharacter.characterCombatManager != null && targetCharacter.characterCombatManager.lockOnTransform != null)
							{
								spawnPos = targetCharacter.characterCombatManager.lockOnTransform.position;
							}

							GameObject hitEffect = Instantiate(impactVFX, spawnPos, Quaternion.identity);
							Destroy(hitEffect, impactVFXDestroyTime);
						}
					}
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (showDebugGizmos)
			{
				Gizmos.color = new Color(0, 0.5f, 1f, 0.3f);
				Gizmos.DrawSphere(transform.position, radius);
			}
		}
	}
}
