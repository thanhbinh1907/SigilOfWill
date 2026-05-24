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
			if (currentSpellBeingCast != null && currentSpellBeingCast.spellPrefab != null)
			{
				if (currentSpellBeingCast == null || currentSpellBeingCast.spellPrefab == null) return;

				//  THUNDERBOLT
				if (currentSpellBeingCast.isSpellFromSky)
				{
					Vector3 strikePosition;

					if (player.isLockOn)
					{
						strikePosition = player.playerCombatManager.currentTarget.transform.position;
					}
					else
					{
						strikePosition = player.transform.position + player.transform.forward * 5f;
					}

					strikePosition.y = player.transform.position.y;
					GameObject bolt = Instantiate(currentSpellBeingCast.spellPrefab, strikePosition, Quaternion.identity);

					DamageCollider damageCollider = bolt.GetComponentInChildren<DamageCollider>();
					if (damageCollider != null)
					{
						damageCollider.characterCausingDamage = player;
						damageCollider.lightningDamage = currentSpellBeingCast.lightningDamage;
						
						// Bắt đầu quy trình giáng sét đồng bộ
						StartCoroutine(ThunderboltStrike(damageCollider));
					}

					ContinuousAOEDamageZone continuousZone = bolt.GetComponent<ContinuousAOEDamageZone>();
					if (continuousZone == null) continuousZone = bolt.GetComponentInChildren<ContinuousAOEDamageZone>();
					if (continuousZone != null)
					{
						continuousZone.characterCausingDamage = player;
						continuousZone.lightningDamage = currentSpellBeingCast.lightningDamage;
						continuousZone.fireDamage = currentSpellBeingCast.fireDamage;
						continuousZone.windDamage = currentSpellBeingCast.windDamage;
					}

					// Đồng bộ sát thương trực tiếp từ va chạm hạt của Hovl Studio (nếu có)
					ParticleCollisionInstance particleCollision = bolt.GetComponent<ParticleCollisionInstance>();
					if (particleCollision == null) particleCollision = bolt.GetComponentInChildren<ParticleCollisionInstance>();
					if (particleCollision != null)
					{
						particleCollision.characterCausingDamage = player;
						particleCollision.lightningDamage = currentSpellBeingCast.lightningDamage;
						particleCollision.fireDamage = currentSpellBeingCast.fireDamage;
						particleCollision.windDamage = currentSpellBeingCast.windDamage;
					}

					Debug.Log($"Sấm sét '{currentSpellBeingCast.name}' đã được triệu hồi từ trên trời!");
				}
				//  WINDBLADE
				else if (currentSpellBeingCast.isMeleeSpell)
				{
					// Với phép cận chiến như Windblade thường, sinh ra tại cổ tay (rightHandSlot) thay vì đầu gậy phép để không bị lệch hiệu ứng chém
					Transform spawnLocation = player.playerEquipmentManager.rightHandSlot.transform;

					// Sinh ra hiệu ứng và gán nó làm con (Parent) của điểm spawn để nó di chuyển theo gậy/tay
					GameObject slash = Instantiate(currentSpellBeingCast.spellPrefab, spawnLocation.position, spawnLocation.rotation);

					DamageCollider damageCollider = slash.GetComponentInChildren<DamageCollider>();
					if (damageCollider != null)
					{
						damageCollider.characterCausingDamage = player;
						damageCollider.windDamage = currentSpellBeingCast.windDamage;

						StartCoroutine(ActiveMeleeSpellHitbox(damageCollider));

						Debug.Log($"Windblade '{currentSpellBeingCast.name}' đã được kích hoạt hitbox!");
					}
				}
				//  FIREBALL ULTIMATE (spellID = 4)
				else if (currentSpellBeingCast.spellID == 4)
				{
					Vector3 strikePosition;

					if (player.isLockOn && player.playerCombatManager.currentTarget != null)
					{
						strikePosition = player.playerCombatManager.currentTarget.transform.position;
					}
					else
					{
						strikePosition = player.transform.position + player.transform.forward * 5f;
					}

					strikePosition.y = player.transform.position.y;
					
					// Triệu hồi quả cầu lửa ultimate tại vị trí mục tiêu
					GameObject ultimateObj = Instantiate(currentSpellBeingCast.spellPrefab, strikePosition, Quaternion.identity);

					// Kích hoạt coroutine để quét gây sát thương nổ diện rộng trùng khớp với hiệu ứng nổ của DelayObjectMake (1.5s)
					StartCoroutine(ExecuteFireballUltimateExplosion(ultimateObj, strikePosition, currentSpellBeingCast.fireDamage));

					Debug.Log($"Fireball Ultimate '{currentSpellBeingCast.name}' đã được triệu hồi tại vị trí đích!");
				}
				//  FIREBALL THƯỜNG
				else
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

					Vector3 shootDirection;

					if (player.isLockOn && player.playerCombatManager.currentTarget != null)
					{
						Transform targetTransform = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform;
						Vector3 targetPos = targetTransform != null ? targetTransform.position : player.playerCombatManager.currentTarget.transform.position;
						shootDirection = targetPos - spawnLocation.position;
					}
					else
					{
						shootDirection = player.transform.forward;
					}
					
					shootDirection.Normalize();

					// Sinh ra quả cầu lửa hướng thẳng về phía mục tiêu/hướng bắn
					GameObject projectile = Instantiate(currentSpellBeingCast.spellPrefab, spawnLocation.position, Quaternion.LookRotation(shootDirection));

					DamageCollider damageCollider = projectile.GetComponentInChildren<DamageCollider>();
					if (damageCollider != null)
					{
						damageCollider.characterCausingDamage = player;
						damageCollider.fireDamage = currentSpellBeingCast.fireDamage;
						damageCollider.lightningDamage = currentSpellBeingCast.lightningDamage;
						damageCollider.windDamage = currentSpellBeingCast.windDamage;
						damageCollider.EnableDamageCollider(); // Kích hoạt collider để gây sát thương khi bay chạm mục tiêu
					}

					Rigidbody rb = projectile.GetComponent<Rigidbody>();
					if (rb != null)
					{
						rb.linearVelocity = shootDirection * currentSpellBeingCast.projectileSpeed;
					}

					Debug.Log($"Spell Projectile '{currentSpellBeingCast.name}' đã được bắn ra!");
				}
			}
			else
			{
				Debug.LogError("Lỗi SpawnProjectile: Thiếu SpellPrefab hoặc chưa lưu currentSpellBeingCast.");
			}
		}

		private IEnumerator ThunderboltStrike(DamageCollider damageCollider)
		{

			if (damageCollider != null)
			{
				damageCollider.DisableDamageCollider();

				yield return new WaitForSeconds(0.3f);

				damageCollider.EnableDamageCollider();

				yield return new WaitForSeconds(0.2f);

				damageCollider.DisableDamageCollider();
			}
		}

		private IEnumerator ActiveMeleeSpellHitbox(DamageCollider damageCollider)
		{
			damageCollider.DisableDamageCollider();
			yield return new WaitForSeconds(0.1f);

			damageCollider.EnableDamageCollider();

			yield return new WaitForSeconds(0.3f);

			damageCollider.DisableDamageCollider();
		}

		private IEnumerator ExecuteFireballUltimateExplosion(GameObject ultimateObj, Vector3 explosionPosition, float fireDamageValue)
		{
			// Chờ 1.5 giây để khớp với thời gian nổ của hiệu ứng (m_startDelay trong DelayObjectMake)
			yield return new WaitForSeconds(1.5f);

			// Thực hiện gây sát thương diện rộng (AOE)
			float radius = 7f; // Bán kính vụ nổ Fireball Ultimate lớn (7 mét)
			Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius, WorldUtilityManager.instance.GetCharacterLayers());
			List<CharacterManager> damagedCharacters = new List<CharacterManager>();

			foreach (var collider in colliders)
			{
				CharacterManager targetCharacter = collider.GetComponentInParent<CharacterManager>();
				if (targetCharacter != null && targetCharacter != player && !targetCharacter.isDead)
				{
					if (!damagedCharacters.Contains(targetCharacter))
					{
						damagedCharacters.Add(targetCharacter);

						if (targetCharacter.isInvulnerable)
							continue;

						// Tạo và áp dụng hiệu ứng nhận sát thương lửa
						TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
						damageEffect.characterCausingDamage = player;
						damageEffect.fireDamage = fireDamageValue;
						damageEffect.physicalDamage = 0;
						damageEffect.magicDamage = 0;
						damageEffect.lightningDamage = 0;
						damageEffect.windDamage = 0;
						damageEffect.holyDamage = 0;
						damageEffect.contactPoint = targetCharacter.transform.position; // Điểm nhận sát thương là tâm đối tượng

						targetCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
					}
				}
			}

			Debug.Log($">> [FIREBALL ULTIMATE] Vụ nổ đã gây sát thương cho {damagedCharacters.Count} mục tiêu với {fireDamageValue} sát thương lửa!");

			// Chờ thêm 3.5 giây (tổng cộng 5 giây) để hiệu ứng hạt bay hết rồi mới hủy GameObject cha tránh rác Scene
			yield return new WaitForSeconds(3.5f);
			if (ultimateObj != null)
			{
				Destroy(ultimateObj);
			}
		}

	}
}