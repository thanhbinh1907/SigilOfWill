using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class ContinuousAOEDamageZone : MonoBehaviour
	{
		[Header("Damage Causer")]
		public CharacterManager characterCausingDamage;

		[Header("AOE Settings")]
		public float radius = 5f;
		public float damageInterval = 0.5f;
		public float duration = 5f;

		[Header("Damage Value")]
		public float physicalDamage = 0;
		public float fireDamage = 0;
		public float magicDamage = 0;
		public float lightningDamage = 30;
		public float windDamage = 0;
		public float holyDamage = 0;

		[Header("VFX Settings")]
		[SerializeField] private bool showDebugGizmos = true;
		public GameObject impactVFX; // Prefab hiệu ứng sét giật xuống (Lightning hit)
		public float impactVFXDestroyTime = 2f;

		private void Start()
		{
			// Bắt đầu chu kỳ gây sát thương liên tục
			StartCoroutine(ApplyContinuousDamage());

			// Tự hủy toàn bộ đám mây/hiệu ứng sau thời gian duration
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
					// Tránh tự gây sát thương cho bản thân (người cast phép)
					if (targetCharacter == characterCausingDamage)
						continue;

					if (!damagedCharacters.Contains(targetCharacter))
					{
						damagedCharacters.Add(targetCharacter);

						if (targetCharacter.isInvulnerable)
							continue;

						// Tạo và áp dụng hiệu ứng nhận sát thương
						TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
						damageEffect.characterCausingDamage = characterCausingDamage;
						
						damageEffect.physicalDamage = physicalDamage;
						damageEffect.fireDamage = fireDamage;
						damageEffect.magicDamage = magicDamage;
						damageEffect.lightningDamage = lightningDamage;
						damageEffect.windDamage = windDamage;
						damageEffect.holyDamage = holyDamage;
						damageEffect.contactPoint = targetCharacter.transform.position; // Điểm nhận sát thương là tâm đối tượng

						targetCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);

						// Sinh ra hiệu ứng sét đánh (Lightning hit) tại vị trí mục tiêu
						if (impactVFX != null)
						{
							Vector3 spawnPos = targetCharacter.transform.position;
							// Ưu tiên lấy vị trí lock-on (giữa ngực) để sét đánh trúng cơ thể thay vì dưới chân
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
