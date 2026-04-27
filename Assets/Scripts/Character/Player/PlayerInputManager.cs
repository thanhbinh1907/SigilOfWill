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

                // Holding the input will set sprintInput to true, releasing it will set it to false
                playerControls.PlayerAction.Sprint.performed += i => sprintInput = true;
                playerControls.PlayerAction.Sprint.canceled += i => sprintInput = false;
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
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
		}

        // MOVEMENT INPUT

        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

            // Snapping moveAmount to either 0.5 or 1 for walk/run distinction
            if (moveAmount <= 0.5f && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1f;
            }

            // horizontal = 0 because we only want non-strafing movement 
            // we use horizontal when we want strafing movement or locked on
            // if we are not locked on, only use moveAmount 
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);
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
    }
}