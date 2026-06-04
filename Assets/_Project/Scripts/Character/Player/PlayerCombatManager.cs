using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class PlayerCombatManager : CharacterCombatManager
	{
		public WeaponItem currentWeaponBeingUsed;

		PlayerManager player;

		[HideInInspector] public SpellAction currentSpellBeingCast;

		[Header("Casting Settings")]
		public float castingTimeoutDuration = 1.5f;
		private float castingTimeoutTimer;

		protected override void Awake()
		{
			base.Awake();
			player = GetComponent<PlayerManager>();
		}

		private void Update()
		{
			if (player.isCasting)
			{
				// Nếu đã nhả phím E, bắt đầu đếm ngược thời gian chờ gói tin từ Python
				if (PlayerInputManager.instance != null && !PlayerInputManager.instance.spellTriggerInput)
				{
					float oldTimer = castingTimeoutTimer;
					castingTimeoutTimer -= Time.deltaTime;
					
					// Log mỗi giây để theo dõi tiến trình timeout mà không gây spam
					if (Mathf.FloorToInt(oldTimer) != Mathf.FloorToInt(castingTimeoutTimer))
					{
						Debug.Log($">> [PLAYER COMBAT] Đang đếm ngược thời gian chờ gói tin từ Python. Còn lại: {castingTimeoutTimer:F1} giây.");
					}

					if (castingTimeoutTimer <= 0)
					{
						Debug.LogWarning("--- HẾT THỜI GIAN CHỜ UDP TỪ PYTHON (TIMEOUT 1.5s) ---");
						DisableCastingState();
						return;
					}
				}

				HandleCasting();
			}
		}

		public void PerformWeaponBasedAction(int actionID, int weaponID)
		{
			WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

			if (weaponAction != null)
			{
				WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

				if (weapon != null)
				{
					weaponAction.AttemptToPerformAction(player, weapon);
				}
			}
		}

		public virtual void DrainStaminaBasedOnAttack()
		{
			if (currentWeaponBeingUsed == null)
				return;

			float staminaDeducted = 0;

			switch (currentAttackType)
			{
				case AttackType.LightAttack01:
					staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.light_Attack_01_Modifier;
					break;
				default:
					break;
			}
			 
			player.currentStamina -= Mathf.RoundToInt(staminaDeducted);
		}

		public void EnableCastingState()
		{
			if (player.isPerformingAction)
			{
				Debug.LogWarning($">> [PLAYER COMBAT] Không thể cast phép vì player.isPerformingAction = true!");
				return;
			}
			if (player.currentMana <= 0)
			{
				Debug.LogWarning($">> [PLAYER COMBAT] Không thể cast phép vì Mana <= 0! Mana hiện tại: {player.currentMana}");
				return;
			}
			if (player.isCasting) return;

			WeaponItem weaponItem = player.playerInventoryManager.currentRightHandWeapon;
			Debug.Log($">> [PLAYER COMBAT] Yêu cầu vào trạng thái cast. Vũ khí tay phải hiện tại: {(weaponItem != null ? weaponItem.name : "null")}");
			if (weaponItem == null || !(weaponItem is StaffWeaponItem))
			{
				Debug.LogWarning($">> [PLAYER COMBAT] Không thể cast vì vũ khí tay phải không phải gậy phép (StaffWeaponItem)!");
				return;
			}

			player.isCasting = true;
			castingTimeoutTimer = castingTimeoutDuration; // Khởi tạo lại timer khi bắt đầu cast
			Debug.Log("--- BẮT ĐẦU CHỜ GIỌNG NÓI VÀ CỬ CHỈ TỪ PYTHON (Đè phím E) ---");

			// Xóa dữ liệu cũ khi bắt đầu đè phím E
			if (UDPReceiver.instance != null) UDPReceiver.instance.ResetUDPData();
		}

		public void DisableCastingState()
		{
			player.isCasting = false;
			Debug.Log("--- KẾT THÚC ĐÈ PHÍM E ---");
			if (UDPReceiver.instance != null) UDPReceiver.instance.ResetUDPData();
		}

		private void HandleCasting()
		{
			if (UDPReceiver.instance != null)
			{
				// Lấy dữ liệu từ UDP Gesture Receiver
				int currentGestureID = UDPReceiver.instance.currentGestureID;
				string currentVoice = UDPReceiver.instance.currentVoiceWord;

				// -1 nghĩa là chưa có gói tin UDP mới gửi tới từ Python (đang chờ gói tin)
				if (currentGestureID == -1)
				{
					return;
				}

				Debug.Log($">> [PLAYER COMBAT] Nhận thấy dữ liệu UDP từ Python khác -1: GestureID = {currentGestureID}, VoiceWord = '{currentVoice}'");

				int spellIDToCast = -1;
				string spellMode = "";

				// 1. Kiểm tra FIREBALL (Thường: 1, Cường hóa: 4)
				if (currentGestureID == 1 && currentVoice == "fireball")
				{
					spellIDToCast = 4; // Bắn ra phép cường hóa có ID = 4
					spellMode = "CƯỜNG HÓA (Cả cử chỉ & giọng nói)";
				}
				else if ((currentGestureID == 1 && currentVoice == "none") || (currentGestureID == 0 && currentVoice == "fireball"))
				{
					spellIDToCast = 1; // Phép thường có ID = 1
					spellMode = "THƯỜNG (Chỉ cử chỉ hoặc chỉ giọng nói)";
				}
				// 2. Kiểm tra THUNDERBOLT (Thường: 2, Cường hóa: 5)
				else if (currentGestureID == 2 && currentVoice == "thunderbolt")
				{
					spellIDToCast = 5; // Bắn ra phép cường hóa có ID = 5
					spellMode = "CƯỜNG HÓA (Cả cử chỉ & giọng nói)";
				}
				else if ((currentGestureID == 2 && currentVoice == "none") || (currentGestureID == 0 && currentVoice == "thunderbolt"))
				{
					spellIDToCast = 2; // Phép thường có ID = 2
					spellMode = "THƯỜNG (Chỉ cử chỉ hoặc chỉ giọng nói)";
				}
				// 3. Kiểm tra WINDBLADE (Thường: 3, Cường hóa: 6)
				else if (currentGestureID == 3 && currentVoice == "windblade")
				{
					spellIDToCast = 6; // Bắn ra phép cường hóa có ID = 6
					spellMode = "CƯỜNG HÓA (Cả cử chỉ & giọng nói)";
				}
				else if ((currentGestureID == 3 && currentVoice == "none") || (currentGestureID == 0 && currentVoice == "windblade"))
				{
					spellIDToCast = 3; // Phép thường có ID = 3
					spellMode = "THƯỜNG (Chỉ cử chỉ hoặc chỉ giọng nói)";
				}

				if (spellIDToCast != -1)
				{
					Debug.Log($"=> [THÀNH CÔNG] Phân tích Combo hợp lệ! Chọn Spell ID {spellIDToCast} | Dạng: {spellMode}");
					AttemptToCastSpell(spellIDToCast);
				}
				else
				{
					Debug.LogWarning($"[THẤT BẠI] Sai Combo hoặc không khớp bất kỳ phép nào: Cử chỉ {currentGestureID} & Giọng '{currentVoice}'");
				}

				// Reset dữ liệu và kết thúc trạng thái cast sau khi đã nhận và xử lý xong gói tin UDP
				UDPReceiver.instance.ResetUDPData();
				DisableCastingState();
			}
		}

		private void AttemptToCastSpell(int gestureID)
		{
			Debug.Log($">> [PLAYER COMBAT] Đang tìm SpellAction cho ID = {gestureID} trong WorldSpellDatabase...");
			SpellAction spellAction = WorldSpellDatabase.instance.GetSpellActionByID(gestureID);
			if (spellAction != null)
			{
				Debug.Log($"[THÀNH CÔNG] Đã tìm thấy Spell: {spellAction.name} (Animation: '{spellAction.spellAnimation}', Mana Cost: {spellAction.manaCost}). Đang chạy spellAction.AttemptToPerformAction...");
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
			if (currentSpellBeingCast != null)
			{
				currentSpellBeingCast.SpawnSpell(player);
			}
			else
			{
				Debug.LogError("Lỗi SpawnProjectile: Chưa lưu currentSpellBeingCast hoặc spell null.");
			}
		}

	}
}