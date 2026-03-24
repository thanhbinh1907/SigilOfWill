using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SG
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        PlayerControl playerControls;

        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

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
			instance.enabled = false;
		}

		private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.instance.GertWorldSceneIndex()) 
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false;
			}
		}

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControl();
                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerMovement.Movement.canceled += i => movementInput = Vector2.zero;
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
            HandleMovementInput();
        }

        private void HandleMovementInput()
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
        }
    }
}