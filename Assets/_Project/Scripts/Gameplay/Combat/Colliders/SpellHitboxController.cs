using UnityEngine;
using System.Collections;

namespace SG
{
	public class SpellHitboxController : MonoBehaviour
	{
		[Header("Components")]
		[SerializeField] private DamageCollider damageCollider;
		[SerializeField] private ContinuousAOEDamageZone continuousZone;
		[SerializeField] private ParticleCollisionInstance particleCollision;

		[Header("Hitbox Timing Settings")]
		[SerializeField] private bool useCustomHitboxTiming = false;
		[SerializeField] private float activeDelay = 0f;
		[SerializeField] private float activeDuration = 0.3f;

		[Header("Destruction Settings")]
		[SerializeField] private bool autoDestroy = true;
		[SerializeField] private float destroyDelay = 5f;

		private void Awake()
		{
			// Tự động tìm kiếm các component nếu chưa kéo thủ công trong Inspector
			if (damageCollider == null) damageCollider = GetComponentInChildren<DamageCollider>();
			if (continuousZone == null) continuousZone = GetComponentInChildren<ContinuousAOEDamageZone>();
			if (particleCollision == null) particleCollision = GetComponentInChildren<ParticleCollisionInstance>();
		}

		public void InitializeSpell(CharacterManager caster, SpellAction spell)
		{
			// Tự động tìm tất cả các thành phần kế thừa từ DamageDealer trong prefab
			DamageDealer[] damageDealers = GetComponentsInChildren<DamageDealer>(true);
			foreach (var dealer in damageDealers)
			{
				dealer.characterCausingDamage = caster;
				dealer.fireDamage = spell.fireDamage;
				dealer.lightningDamage = spell.lightningDamage;
				dealer.windDamage = spell.windDamage;

				if (dealer is DamageCollider damageCol)
				{
					Collider col = damageCol.GetComponent<Collider>();
					if (col != null)
					{
						Debug.Log($"[SpellHitbox] Đã liên kết DamageCollider trên {gameObject.name}. Caster: {caster.name}. IsTrigger của Collider: {col.isTrigger}");
						if (!col.isTrigger)
						{
							Debug.LogWarning($"[SpellHitbox] WARNING: Collider trên {gameObject.name} CHƯA được tích 'Is Trigger'! Chiêu thức sẽ bay xuyên qua hoặc va chạm vật lý mà không gây sát thương.");
						}
					}
					else
					{
						Debug.LogError($"[SpellHitbox] ERROR: Không tìm thấy component Collider vật lý nào đi kèm với DamageCollider trên {gameObject.name}!");
					}
				}
				else
				{
					Debug.Log($"[SpellHitbox] Đã liên kết DamageDealer ({dealer.GetType().Name}) trên {gameObject.name}.");
				}
			}

			// Quản lý việc bật/tắt Hitbox theo thời gian trễ
			if (useCustomHitboxTiming)
			{
				StartCoroutine(ExecuteCustomHitboxTiming());
			}
			else
			{
				// Mặc định: Kích hoạt ngay lập tức cho các projectile thông thường
				if (damageCollider != null)
				{
					damageCollider.EnableDamageCollider();
					Debug.Log($"[SpellHitbox] Đã kích hoạt ngay lập tức DamageCollider cho {gameObject.name}.");
				}
			}

			// Quản lý việc tự hủy vật thể tránh rác Scene
			if (autoDestroy)
			{
				Destroy(gameObject, destroyDelay);
			}
		}

		private IEnumerator ExecuteCustomHitboxTiming()
		{
			if (damageCollider != null)
			{
				damageCollider.DisableDamageCollider();

				yield return new WaitForSeconds(activeDelay);

				damageCollider.EnableDamageCollider();

				yield return new WaitForSeconds(activeDuration);

				damageCollider.DisableDamageCollider();
			}
		}
	}
}
