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

			if (PlayerInputManager.instance != null)
			{
				PlayerInputManager.instance.player = this;
			}
            if (PlayerUIManager.instance != null)
            {
				OnStaminaChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                OnStaminaChanged += playerStatsManager.ResetStaminaRegenTimer;

                maxStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(endurance);
                currentStamina = maxStamina;
                if (PlayerUIManager.instance != null)
                {
                    PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina);
                }
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
		}

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData) 
        {
            characterName = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPosition;
		}
    }
}