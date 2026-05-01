using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class PlayerCombatManager : CharacterCombatManager
	{
		PlayerManager player;
		[HideInInspector] public SpellAction currentSpellBeingCast;

		protected override void Awake()
		{
			base.Awake();
			player = GetComponent<PlayerManager>();
		}

		private void Update()
		{
			HandleGestureCasting();
		}

		public void EnableCastingState()
		{
			if (player.isPerformingAction)
				return;
			if (player.currentMana <= 0)
				return;
			if (player.isCasting) 
				return;

			WeaponItem weaponItem = player.playerInventoryManager.currentRightHandWeapon;
			if (weaponItem == null || !(weaponItem is StaffWeaponItem))
				return;

			player.isCasting = true;
			Debug.Log("--- BẮT ĐẦU CHỜ NHẬN DIỆN CỬ CHỈ (Đè phím E) ---");

			if (UDPGestureReceiver.instance != null)
			{
				UDPGestureReceiver.instance.ResetGesture();
			}
		}

		public void DisableCastingState()
		{
			player.isCasting = false;
			Debug.Log("--- KẾT THÚC CHỜ NHẬN DIỆN CỬ CHỈ (Nhả phím E) ---");
		}

		private void HandleGestureCasting()
		{
			// Mở cửa cho UDP chạy vào.
			if (UDPGestureReceiver.instance != null)
			{
				int currentGestureID = UDPGestureReceiver.instance.currentGestureID;

				//  CÓ ID TỪ PYTHON TRẢ VỀ
				if (currentGestureID >= 0)
				{
					// In ra log để xem Unity nhận được số mấy
					Debug.Log($"[UDP NHẬN ĐƯỢC]: Cử chỉ ID = {currentGestureID}");

					// Ở Python: Neutral = 0, Fireball = 1, Thunderbolt = 2...
					// Chỉ cast khi nhận được ID từ 1 trở lên (Bỏ qua Neutral)
					if (currentGestureID >= 1)
					{
						Debug.Log($"=> Quyết định tung chiêu với ID: {currentGestureID}");
						AttemptToCastSpell(currentGestureID);

						// Reset UDP và Tắt trạng thái Casting
						UDPGestureReceiver.instance.ResetGesture();
						DisableCastingState();
					}
				}
			}
		}

		private void AttemptToCastSpell(int gestureID)
		{
			SpellAction spellAction = WorldSpellDatabase.instance.GetSpellActionByID(gestureID);
			if (spellAction != null)
			{
				Debug.Log($"[THÀNH CÔNG] Đã tìm thấy Spell: {spellAction.name}. Đang chạy Animation...");
				spellAction.AttemptToPerformAction(player);
			}
			else
			{
				Debug.LogWarning($"[LỖI] Không tìm thấy SpellAction nào trong Database có ID = {gestureID}!");
			}
		}

		public void SpawnProjectile()
		{
			Debug.Log("Đang gọi Animation Event: SpawnProjectile...");
			if (currentSpellBeingCast != null && currentSpellBeingCast.spellPrefab != null)
			{
				Transform spawnLocation = null;

				if (player.playerEquipmentManager != null && player.playerEquipmentManager.rightWeaponManager != null)
				{
					spawnLocation = player.playerEquipmentManager.rightWeaponManager.spellSpawnPoint;
				}

				if (spawnLocation == null)
				{
					spawnLocation = player.playerEquipmentManager.rightHandSlot.transform;
					Debug.LogWarning("Vũ khí không có Spell Spawn Point. Lấy tạm vị trí tay phải.");
				}

				GameObject projectile = Instantiate(currentSpellBeingCast.spellPrefab, spawnLocation.position, player.transform.rotation);

				Rigidbody rb = projectile.GetComponent<Rigidbody>();
				if (rb != null)
				{
					Vector3 shootDirection = PlayerCamera.instance.transform.forward;
					shootDirection.y = 0;
					rb.linearVelocity = shootDirection * currentSpellBeingCast.projectileSpeed;
					Debug.Log("Quả cầu lửa đã được ném đi!");
				}
			}
			else
			{
				Debug.LogError("Lỗi SpawnProjectile: Thiếu SpellPrefab hoặc chưa lưu currentSpellBeingCast.");
			}
		}
	}
}