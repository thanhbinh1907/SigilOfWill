using UnityEngine;

namespace SG
{
	public class ProjectileDamageCollider : DamageCollider
	{
		public float light_attack_Modifier;
		public float heavy_attack_Modifier;

		[Header("Projectile Collision Settings")]
		public GameObject explodeVFX;
		public float explodeVFXDestroyTime = 2f;

		protected override void OnTriggerEnter(Collider other)
		{
			Debug.Log($"[ProjectileDamageCollider] OnTriggerEnter kích hoạt trên {gameObject.name} va chạm với '{other.name}' (Layer: {LayerMask.LayerToName(other.gameObject.layer)}, IsTrigger: {other.isTrigger})");

			// Bỏ qua nếu va chạm với chính nhân vật bắn ra phép
			CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
			if (damageTarget != null && damageTarget == characterCausingDamage)
			{
				Debug.Log($"[ProjectileDamageCollider] Bỏ qua va chạm vì mục tiêu là Caster: {damageTarget.name}");
				return;
			}

			// Bỏ qua các trigger collider khác (như vùng lock-on, vùng tương tác...)
			if (other.isTrigger)
			{
				Debug.Log($"[ProjectileDamageCollider] Bỏ qua va chạm vì collider chạm phải cũng là Trigger.");
				return;
			}

			// Xác định điểm va chạm
			contactPoint = other.ClosestPointOnBounds(transform.position);

			// Nếu chạm phải nhân vật/quái thì gây sát thương
			if (damageTarget != null)
			{
				if (damageTarget.isInvulnerable)
				{
					Debug.Log($"[ProjectileDamageCollider] Bỏ qua gây sát thương lên {damageTarget.name} vì đang bất tử.");
					return;
				}

				Debug.Log($"[ProjectileDamageCollider] Gây sát thương lên {damageTarget.name}. Chi tiết - Vật lý: {physicalDamage}, Lửa: {fireDamage}, Sét: {lightningDamage}, Gió: {windDamage}, Phép: {magicDamage}");
				DamageTarget(damageTarget);
			}
			else
			{
				Debug.Log($"[ProjectileDamageCollider] Va chạm với vật thể môi trường (không phải Character).");
			}

			// Nếu có hiệu ứng nổ (như Quả cầu lửa): Tạo vụ nổ và tự hủy đạn khi va chạm bất kỳ thứ gì (kể cả quái hay tường)
			if (explodeVFX != null)
			{
				Debug.Log($"[ProjectileDamageCollider] Tạo hiệu ứng nổ {explodeVFX.name} và tự hủy đạn.");
				GameObject explosion = Instantiate(explodeVFX, contactPoint, Quaternion.identity);
				Destroy(explosion, explodeVFXDestroyTime);
				Destroy(transform.root.gameObject);
			}
			// Nếu KHÔNG có hiệu ứng nổ (như Windblade Ultimate): Bay xuyên qua mọi thứ (quái + tường), không tự hủy!
		}

		protected override void DamageTarget(CharacterManager damageTarget)
		{
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

			switch (characterCausingDamage.characterCombatManager.currentAttackType)
			{
				case AttackType.LightAttack01:
					ApplyAttackDamageModifiers(light_attack_Modifier, damageEffect);
					break;
				case AttackType.HeavyAttack01:
					ApplyAttackDamageModifiers(heavy_attack_Modifier, damageEffect);
					break;
				default:
					break;
			}

			damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
		}

		private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
		{
			damage.physicalDamage *= modifier;
			damage.fireDamage *= modifier;
			damage.magicDamage *= modifier;
			damage.lightningDamage *= modifier;
			damage.windDamage *= modifier;
			damage.holyDamage *= modifier;
			damage.poiseDamage *= modifier;
		}
	}
}
