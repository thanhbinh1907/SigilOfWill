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


        [Header("Camera Movement Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        [Header("Player Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("Player Action Input")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput = false;
        [SerializeField] bool jumpInput = false;

        [Header("Lock On Input")]
        [SerializeField] bool lockOnInput = false; 

		[Header("Player Combat Input")]
        [SerializeField] bool spellTriggerInput = false;

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

            if (SceneManager.GetActiveScene().buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
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
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
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
        }
        void Update()
        {
            if (player == null) return;

            HandleAllInput();
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
		}

        // MOVEMENT INPUT

        private void HandlePlayerMovementInput()
        {
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
            if (jumpInput)
            {
                jumpInput = false;

                // IF WE HAVE A UI WINDOW OPEN, WE DON'T WANT TO JUMP, SO RETURN EARLY

                // ATTEMPT TO PERFORM JUMP
                player.playerLocomotionManager.AttemptToPerformJump();

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
    }
}