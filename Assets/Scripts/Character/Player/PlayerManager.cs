using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;

        [Header("Character Info")]
        public string characterName = "Character";

		protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
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

		}

		protected override void Update()
        {
            base.Update();
            playerLocomotionManager.HandleAllMovement();
            playerStatsManager.RegenerateStamina();
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
			        }
		        }
        #endif
	}
}