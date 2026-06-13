using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
		[Header("Debug Menu")]
		[SerializeField] bool respawnCharacter = false;
		[SerializeField] bool switchRightWeapon = false;
		[SerializeField] bool testCastFireball = false;
		[SerializeField] bool testCastThunderbolt = false;
		[SerializeField] bool testCastWindblade = false;
		[SerializeField] bool testCastStrongFireball = false;
		[SerializeField] bool testCastStrongThunderbolt = false;
		[SerializeField] bool testCastStrongWindblade = false;
		[SerializeField] bool testFrontHit;
		[SerializeField] bool testBackHit;
		[SerializeField] bool testLeftHit;
		[SerializeField] bool testRightHit;


		public static PlayerManager instance;

		[HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
		[HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;
		[HideInInspector] public PlayerCamera playerCamera;
		[HideInInspector] public PlayerInteractionManager playerInteractionManager;



		protected override void Awake()
        {
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Destroy(gameObject);
				return;
			}

            base.Awake();

			DontDestroyOnLoad(gameObject);

			playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
			playerCamera = GetComponent<PlayerCamera>();
			playerInteractionManager = GetComponent<PlayerInteractionManager>();
		}

		protected override void Start()
		{
			base.Start();

			if (PlayerCamera.instance != null)
            {
                PlayerCamera.instance.player = this;
			}

            if (PlayerUIManager.instance != null)
            {
                PlayerInputManager.instance.player = this;

				// UPDATE UI STATS BAR WHEN A STATS CHANGE
				OnHealthChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
				
                OnStaminaChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                OnStaminaChanged += playerStatsManager.ResetStaminaRegenTimer;

                OnManaChanged += PlayerUIManager.instance.playerUIHudManager.SetNewManaValue;
                OnManaChanged += playerStatsManager.ResetManaRegenTimer;

			}

            if (WorldSaveGameManager.instance != null)
            {
                WorldSaveGameManager.instance.player = this;
			}
		}

		private void PlayerManager_OnManaChanged(float arg1, float arg2)
		{
			throw new System.NotImplementedException();
		}

		protected override void Update()
        {
            base.Update();
            playerLocomotionManager.HandleAllMovement();
            playerStatsManager.RegenerateStamina();
            playerStatsManager.RegenerateMana();
			PlayerCamera.instance.HandleAllCameraActions();

			if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
			{
				WorldSaveGameManager.instance.currentCharacterData.secondsPlayed += Time.deltaTime;
			}

			DebugMenu();
        }

		public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
		{
			if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
			{
				PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
			}

			yield return base.ProcessDeathEvent(manuallySelectDeathAnimation);

			// Tự động hồi sinh và tải lại game tại trạm nghỉ Grace gần nhất
			if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
			{
				var saveData = WorldSaveGameManager.instance.currentCharacterData;

				if (saveData.hasGraceSaved)
				{
					// Nếu đã từng ngồi/kích hoạt Grace, hồi sinh tại tọa độ Grace đó
					saveData.sceneIndex = saveData.lastGraceSceneIndex;
					saveData.xPosition = saveData.lastGraceXPosition;
					saveData.yPosition = saveData.lastGraceYPosition;
					saveData.zPosition = saveData.lastGraceZPosition;
				}
				else
				{
					// Nếu chưa ngồi Grace nào, hồi sinh tại scene ban đầu và vị trí mặc định khởi đầu
					saveData.sceneIndex = WorldSaveGameManager.instance.worldSceneIndex;
					saveData.xPosition = WorldSaveGameManager.instance.startingPosition.x;
					saveData.yPosition = WorldSaveGameManager.instance.startingPosition.y;
					saveData.zPosition = WorldSaveGameManager.instance.startingPosition.z;
				}

				// Reset chỉ số sinh mạng về tối đa để khi load game sẽ hồi phục hoàn toàn
				saveData.currentHealth = playerStatsManager.CalculateHealthBasedOnVitalityLevel(saveData.vitality);
				saveData.currentStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(saveData.endurance);
				saveData.currentMana = playerStatsManager.CalculateManaBasedOnIntelligenceLevel(saveData.intelligence);

				// Lưu lại trước khi load game để bảo toàn vị trí respawn mới
				WorldSaveGameManager.instance.SaveGame();

				// Tải lại game và load lại Scene (sẽ hồi sinh lại quái và đặt lại Player)
				WorldSaveGameManager.instance.RespawnPlayer();
			}
		}

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

			currentCharacterData.characterName = characterName;
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = currentHealth;
            currentCharacterData.currentStamina = currentStamina;
            currentCharacterData.currentMana = currentMana;

			currentCharacterData.vitality = vitality;
            currentCharacterData.endurance = endurance;
            currentCharacterData.intelligence = intelligence;

			if (playerInventoryManager.currentRightHandWeapon != null)
			{
				currentCharacterData.currentRightHandWeaponID = playerInventoryManager.currentRightHandWeapon.itemID;
			}
			if (playerInventoryManager.currentLeftHandWeapon != null)
			{
				currentCharacterData.currentLeftHandWeaponID = playerInventoryManager.currentLeftHandWeapon.itemID;
			}

			// SAVE INVENTORY & QUICK SLOTS
			if (currentCharacterData.itemsInventoryIDs == null)
				currentCharacterData.itemsInventoryIDs = new List<int>();
			else
				currentCharacterData.itemsInventoryIDs.Clear();

			foreach (var item in playerInventoryManager.itemsInventory)
			{
				if (item != null)
				{
					currentCharacterData.itemsInventoryIDs.Add(item.itemID);
				}
			}

			if (currentCharacterData.weaponsInRightHandSlotsIDs == null)
				currentCharacterData.weaponsInRightHandSlotsIDs = new List<int>();
			else
				currentCharacterData.weaponsInRightHandSlotsIDs.Clear();

			foreach (var weapon in playerInventoryManager.weaponsInRightHandSlots)
			{
				if (weapon != null)
				{
					currentCharacterData.weaponsInRightHandSlotsIDs.Add(weapon.itemID);
				}
				else
				{
					currentCharacterData.weaponsInRightHandSlotsIDs.Add(WorldItemDatabase.instance.unarmedWeapon.itemID);
				}
			}

			if (currentCharacterData.weaponsInLeftHandSlotsIDs == null)
				currentCharacterData.weaponsInLeftHandSlotsIDs = new List<int>();
			else
				currentCharacterData.weaponsInLeftHandSlotsIDs.Clear();

			foreach (var weapon in playerInventoryManager.weaponsInLeftHandSlots)
			{
				if (weapon != null)
				{
					currentCharacterData.weaponsInLeftHandSlotsIDs.Add(weapon.itemID);
				}
				else
				{
					currentCharacterData.weaponsInLeftHandSlotsIDs.Add(WorldItemDatabase.instance.unarmedWeapon.itemID);
				}
			}

			currentCharacterData.rightHandWeaponIndex = playerInventoryManager.rightHandWeaponIndex;
			currentCharacterData.leftHandWeaponIndex = playerInventoryManager.leftHandWeaponIndex;
		}

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData) 
        {
            isDead = false;

            characterName = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            
            // THỦ THUẬT UNITY: Tạm thời tắt CharacterController để tránh bị lỗi tự động giật ngược vị trí cũ khi dịch chuyển (Teleport)
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            transform.position = myPosition;

            if (characterController != null)
            {
                characterController.enabled = true;
            }

            if (playerAnimatorManager != null)
            {
                playerAnimatorManager.PlayTargetAnimation("Empty", false, false, true, true);
            }

            vitality = currentCharacterData.vitality;
            endurance = currentCharacterData.endurance;
            intelligence = currentCharacterData.intelligence;

			maxHealth = playerStatsManager.CalculateHealthBasedOnVitalityLevel(vitality);
			currentHealth = maxHealth;
			if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIHudManager != null)
			{
				PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth);
			}

			maxStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(endurance);
			currentStamina = maxStamina;
			if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIHudManager != null)
			{
				PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina);
			}

			maxMana = playerStatsManager.CalculateManaBasedOnIntelligenceLevel(intelligence);
			currentMana = maxMana;
			if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIHudManager != null)
			{
				PlayerUIManager.instance.playerUIHudManager.SetMaxManaValue(maxMana);
			}

			playerInventoryManager.currentRightHandWeapon = WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.currentRightHandWeaponID);
			playerInventoryManager.currentLeftHandWeapon = WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.currentLeftHandWeaponID);

			// LOAD INVENTORY
			playerInventoryManager.itemsInventory.Clear();
			if (currentCharacterData.itemsInventoryIDs != null)
			{
				foreach (var itemID in currentCharacterData.itemsInventoryIDs)
				{
					WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(itemID);
					if (weapon != null)
					{
						playerInventoryManager.AddItemToInventory(weapon);
					}
				}
			}

			// LOAD QUICK SLOTS
			for (int i = 0; i < 3; i++)
			{
				if (currentCharacterData.weaponsInRightHandSlotsIDs != null && i < currentCharacterData.weaponsInRightHandSlotsIDs.Count)
				{
					playerInventoryManager.weaponsInRightHandSlots[i] = WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.weaponsInRightHandSlotsIDs[i]);
				}
				else
				{
					playerInventoryManager.weaponsInRightHandSlots[i] = WorldItemDatabase.instance.unarmedWeapon;
				}
			}

			for (int i = 0; i < 3; i++)
			{
				if (currentCharacterData.weaponsInLeftHandSlotsIDs != null && i < currentCharacterData.weaponsInLeftHandSlotsIDs.Count)
				{
					playerInventoryManager.weaponsInLeftHandSlots[i] = WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.weaponsInLeftHandSlotsIDs[i]);
				}
				else
				{
					playerInventoryManager.weaponsInLeftHandSlots[i] = WorldItemDatabase.instance.unarmedWeapon;
				}
			}

			playerInventoryManager.rightHandWeaponIndex = currentCharacterData.rightHandWeaponIndex;
			playerInventoryManager.leftHandWeaponIndex = currentCharacterData.leftHandWeaponIndex;

			playerEquipmentManager.LoadWeaponsOnBothHands();
		}

		public override void ReviveCharacter()
		{
			base.ReviveCharacter();

			isDead = false;

			currentHealth = maxHealth;
            currentStamina = maxStamina;
            currentMana = maxMana;
			// RESTORE FOCUS POINT

			// PLAY REBIRTH EFFECTS
			playerAnimatorManager.PlayTargetAnimation("Empty", false, false, true, true);
		}
		// =============================================== DEBUG =============================================== //
		private void DebugMenu()
        {
            if (respawnCharacter) 
            {
                respawnCharacter = false;
                ReviveCharacter();
			}

			if (switchRightWeapon)
			{
				switchRightWeapon = false;
				playerEquipmentManager.SwitchRightWeapon();
			}

			if (testCastFireball)
			{
				testCastFireball = false; 
										  
				SpellAction spell = WorldSpellDatabase.instance.GetSpellActionByID(1);
				if (spell != null) spell.AttemptToPerformAction(this);
			}

			if (testCastThunderbolt)
			{
				testCastThunderbolt = false;
				SpellAction spell = WorldSpellDatabase.instance.GetSpellActionByID(2);
				if (spell != null) spell.AttemptToPerformAction(this);
			}

			if (testCastWindblade)
			{
				testCastWindblade = false;
				SpellAction spell = WorldSpellDatabase.instance.GetSpellActionByID(3);
				if (spell != null) spell.AttemptToPerformAction(this);
			}

			if (testCastStrongFireball)
			{
				testCastStrongFireball = false;
				SpellAction spell = WorldSpellDatabase.instance.GetSpellActionByID(4);
				if (spell != null) spell.AttemptToPerformAction(this);
			}

			if (testCastStrongThunderbolt)
			{
				testCastStrongThunderbolt = false;
				SpellAction spell = WorldSpellDatabase.instance.GetSpellActionByID(5);
				if (spell != null) spell.AttemptToPerformAction(this);
			}

			if (testCastStrongWindblade)
			{
				testCastStrongWindblade = false;
				SpellAction spell = WorldSpellDatabase.instance.GetSpellActionByID(6);
				if (spell != null) spell.AttemptToPerformAction(this);
			}

			if (testFrontHit) { testFrontHit = false; ForceDebugHit(180); }
			if (testBackHit) { testBackHit = false; ForceDebugHit(0); }
			if (testLeftHit) { testLeftHit = false; ForceDebugHit(-90); }
			if (testRightHit) { testRightHit = false; ForceDebugHit(90); }
		}

		private void ForceDebugHit(float angle)
		{
			// 1. Khởi tạo hiệu ứng sát thương từ Database
			TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

			// 2. Gán các thông số cần thiết
			damageEffect.physicalDamage = 10; // Sát thương giả định
			damageEffect.angleHitFrom = angle; // Góc đánh truyền vào
			damageEffect.contactPoint = transform.position + Vector3.up; // Điểm va chạm (ngay ngực nhân vật)

			// 3. Chạy quy trình xử lý hiệu ứng (Bao gồm Animation, SFX, VFX)
			characterEffectsManager.ProcessInstantEffect(damageEffect);
		}

		// TEST, WILL BE REMOVED LATER
		#if UNITY_EDITOR
		private void OnValidate()
		        {
			        // Kiểm tra nếu game đang chạy và các Manager đã tồn tại
			        if (Application.isPlaying && playerStatsManager != null && PlayerUIManager.instance != null)
			        {
				        // 1. Tính toán lại Max Health dựa trên Vitality mới
				        maxHealth = playerStatsManager.CalculateHealthBasedOnVitalityLevel(vitality);
				        // 2. Cập nhật thanh UI
				        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth);

				        // Tương tự cho Stamina
				        maxStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(endurance);
				        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina);

                        maxMana = playerStatsManager.CalculateManaBasedOnIntelligenceLevel(intelligence);
                        PlayerUIManager.instance.playerUIHudManager.SetMaxManaValue(maxMana);

					// 3. Kiểm tra nếu currentHealth <= 0 và chưa chết, thì xử lý chết
					if (currentHealth <= 0 && !isDead)
						{
							// Truy cập trực tiếp hàm xử lý chết vì Event không tự chạy
							StartCoroutine(ProcessDeathEvent());
						}
			        }
		        }
        #endif
	}
}