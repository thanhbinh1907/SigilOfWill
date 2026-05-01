using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        [Header("Debug Menu")]
        [SerializeField] bool respawnCharacter = false;
        [SerializeField] bool switchRightWeapon = false; 

		[HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
		[HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;

		[Header("Character Info")]
        public string characterName = "Character";

		protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
		}

		private void Start()
		{
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

			}

            if (WorldSaveGameManager.instance != null)
            {
                WorldSaveGameManager.instance.player = this;
			}

            OnHealthChanged += CheckHP;

		}

		protected override void Update()
        {
            base.Update();
            playerLocomotionManager.HandleAllMovement();
            playerStatsManager.RegenerateStamina();

            DebugMenu();
        }

		public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
		{
            PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();

			return base.ProcessDeathEvent(manuallySelectDeathAnimation);

            // CHECK FOR PLAYERS THAT ARE ALIVE, IF 0 RESPAWN CHARACTERS

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

			currentCharacterData.vitality = vitality;
            currentCharacterData.endurance = endurance;
		}

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData) 
        {
            characterName = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPosition;

            vitality = currentCharacterData.vitality;
            endurance = currentCharacterData.endurance;

			maxHealth = playerStatsManager.CalculateHealthBasedOnVitalityLevel(vitality);
            currentHealth = maxHealth;
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth);

			maxStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(endurance);
			currentStamina = maxStamina;
			PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina);

		}

		public override void ReviveCharacter()
		{
			base.ReviveCharacter();

            currentHealth = maxHealth;
            currentStamina = maxStamina;
            // RESTORE FOCUS POINT

            // PLAY REBIRTH EFFECTS
            playerAnimatorManager.PlayTargetAnimation("Empty", false);
		}

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