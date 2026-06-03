using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SG
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        public PlayerManager player;

        PlayerControl playerControls;


        [Header("CAMERA MOVEMENT INPUT")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        [Header("PLAYER MOVEMENT INPUT")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("PLAYER ACTION INPUT")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput = false;
        [SerializeField] bool jumpInput = false;
        //[SerializeField] bool leftMouseInput = false;

		[Header("LOCK ON INPUT")]
        [SerializeField] bool lockOnInput = false; 

		[Header("PLAYER COMBAT INPUT")]
        [SerializeField] public bool spellTriggerInput = false;

		[Header("WEAPON SWITCH INPUT")]
		public bool switchRightWeaponInput = false;
		public bool switchLeftWeaponInput = false;

		[Header("PLAYER INTERACTION INPUT")]
		[SerializeField] bool interactionInput = false;

		[Header("UI Inputs")]
		public bool closeMenuInput;
		public bool openCharacterMenuInput;

		private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChange;

            if (WorldSaveGameManager.instance != null && SceneManager.GetActiveScene().buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false;
            }
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            if (WorldSaveGameManager.instance != null && newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                instance.enabled = false;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControl();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerAction.Dodge.performed += i => dodgeInput = true;
                playerControls.PlayerAction.Jump.performed += i => jumpInput = true;

                // HOLDING THE INPUT WILL SET SPRINTINPUT TO TRUE, RELEASING IT WILL SET IT TO FALSE
                playerControls.PlayerAction.Sprint.performed += i => sprintInput = true;
                playerControls.PlayerAction.Sprint.canceled += i => sprintInput = false;

                //playerControls.PlayerAction.LeftMouse.performed += i => leftMouseInput = true;

				// SWITCH WEAPON INPUT
				playerControls.PlayerAction.SwitchRightWeapon.performed += i => switchRightWeaponInput = true;
				playerControls.PlayerAction.SwitchLeftWeapon.performed += i => switchLeftWeaponInput = true;

				// LOCK ON 
				playerControls.PlayerAction.LockOn.performed += i => lockOnInput = true;

				playerControls.PlayerCombat.SpellTrigger.performed += i =>
				{
					spellTriggerInput = true;
					Debug.Log(">>(Player Input Manager) ĐÃ NHẬN DIỆN PHÍM E ĐƯỢC BẤM XUỐNG!"); 
				};
				playerControls.PlayerCombat.SpellTrigger.canceled += i =>
				{
					spellTriggerInput = false;
					Debug.Log(">> (Player Input Manager) ĐÃ NHẢ PHÍM E!"); 
				};

				// INTERACTION
				playerControls.PlayerAction.Interact.performed += i => interactionInput = true;

				// UI INPUTS
				playerControls.PlayerAction.OpenCharacterMenu.performed += i => openCharacterMenuInput = true;
			}
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;

			if (instance == this)
			{
				instance = null;
			}
		}
        void Update()
        {
            if (player == null) return;

            HandleAllInput();

            // Cập nhật trạng thái con trỏ chuột dựa trên UI
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.menuWindowIsOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                if (WorldSaveGameManager.instance != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        private void HandleAllInput()
        {
            HandleLockOnInput();
			HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleCastSpellInput();
            //HandleLeftMouseInput();
			HandleSwitchWeaponInput();
			HandleInteractionInput();
			HandleCloseUIInput();
			HandleOpenCharacterMenuInput();
		}

        // MOVEMENT INPUT

        private void HandlePlayerMovementInput()
        {
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.menuWindowIsOpen)
            {
                verticalInput = 0;
                horizontalInput = 0;
                moveAmount = 0;
                if (player != null) player.isMoving = false;
                return;
            }

            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

            // SNAPPING MOVEAMOUNT TO EITHER 0.5 OR 1 FOR WALK/RUN DISTINCTION
            if (moveAmount <= 0.5f && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1f;
            }

            if (player == null) 
                return;

            if (moveAmount != 0)
            {
                player.isMoving = true;
			}
            else
            {
                player.isMoving = false;
			}
            
			if (player.isLockOn)
            {
                if (player.isSprinting) 
                {
                    player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, verticalInput, player.isSprinting);
				}
                else
                {
					player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.isSprinting);
				}
            }
            // HORIZONTAL = 0 BECAUSE WE ONLY WANT NON-STRAFING MOVEMENT 
            // WE USE HORIZONTAL WHEN WE WANT STRAFING MOVEMENT OR LOCKED ON
            // IF WE ARE NOT LOCKED ON, ONLY USE MOVEAMOUNT  
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);
            }
        }

        private void HandleCameraMovementInput()
        {
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.menuWindowIsOpen)
            {
                cameraVerticalInput = 0;
                cameraHorizontalInput = 0;
                return;
            }

            cameraInput = playerControls.PlayerCamera.Movement.ReadValue<Vector2>();

            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        // ACTION INPUT

        private void HandleDodgeInput()
        {
            if (dodgeInput)
            {
                dodgeInput = false;

                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleSprintInput()
        {
            if (sprintInput)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.isSprinting = false;
            }
        }

        private void HandleJumpInput()
        {
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.menuWindowIsOpen)
                return;

            if (jumpInput)
            {
                jumpInput = false;

                // ATTEMPT TO PERFORM JUMP
                player.playerLocomotionManager.AttemptToPerformJump();

            }
        }

        /*
        private void HandleLeftMouseInput()
        {
            if (leftMouseInput)
            {
                leftMouseInput = false;

                player.SetCharacterActionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(
                    player.playerInventoryManager.currentRightHandWeapon.oh_RB_Action.actionID, 
                    player.playerInventoryManager.currentRightHandWeapon.itemID);
            }
		}
        */

		private void HandleSwitchWeaponInput()
        {
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.menuWindowIsOpen)
                return;

            bool keyboard1Pressed = UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame;
            bool keyboard2Pressed = UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame;

            // Reset the old input triggers so they don't stack up or interfere
            switchRightWeaponInput = false;
            switchLeftWeaponInput = false;

            if (keyboard1Pressed)
            {
                player.playerEquipmentManager.SwitchRightWeapon();
            }
            if (keyboard2Pressed)
            {
                player.playerEquipmentManager.SwitchLeftWeapon();
            }
		}

		// LOCK ON INPUT

		private void HandleLockOnInput()
        {
            if (player.isLockOn)
            {
                if (player.playerCombatManager.currentTarget == null)
                    return;

                if (player.playerCombatManager.currentTarget.isDead)
                {
                    PlayerCamera.instance.HandleLockOnTargets();

                    if (player.playerCombatManager.currentTarget == null || player.playerCombatManager.currentTarget.isDead)
                    {
                        player.isLockOn = false;
                        player.playerCombatManager.currentTarget = null;
                    }
				}
			}

            if (lockOnInput)
            {
                lockOnInput = false;
                if (player.isLockOn)
                {
                    player.isLockOn = false;
                    player.playerCombatManager.currentTarget = null;
                }
                else
                {
                    PlayerCamera.instance.HandleLockOnTargets();
				}
			}
		}

		private void HandleCastSpellInput()
        {
            if (spellTriggerInput)
            {
                player.playerCombatManager.EnableCastingState();
			}
        }

		private void HandleInteractionInput()
		{
			if (interactionInput)
			{
				interactionInput = false;

				// Nếu bảng thông báo nhận vật phẩm đang hiển thị trên UI, bấm phím tương tác lần nữa sẽ ẩn nó đi
				if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
				{
					if (PlayerUIManager.instance.playerUIPopUpManager.IsItemPopupActive())
					{
						PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopupWindows();
						return;
					}
				}

				if (player != null && player.playerInteractionManager != null)
				{
					player.playerInteractionManager.Interact();
				}
			}
		}

		private void HandleCloseUIInput()
		{
			// Kiểm tra nhấn phím Escape trên bàn phím
			bool escapePressed = UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;

			if (escapePressed)
			{
				closeMenuInput = true;
			}

			if (closeMenuInput)
			{
				closeMenuInput = false;
				if (PlayerUIManager.instance != null && PlayerUIManager.instance.menuWindowIsOpen)
				{
					PlayerUIManager.instance.CloseAllMenuWindows();
				}
			}
		}

		private void HandleOpenCharacterMenuInput()
		{
			if (openCharacterMenuInput)
			{
				openCharacterMenuInput = false;

				if (PlayerUIManager.instance != null)
				{
					if (PlayerUIManager.instance.playerUIPopUpManager != null)
						PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopupWindows(); 

					PlayerUIManager.instance.CloseAllMenuWindows();

					if (PlayerUIManager.instance.playerUICharacterMenuManager != null)
						PlayerUIManager.instance.playerUICharacterMenuManager.OpenCharacterMenu(); 
				}
			}
		}
	}
}