using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor.Rendering;
using System.Xml;

namespace SG
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player; 

        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        [Header("Movement Setting")]
		private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
		[SerializeField] float walkingSpeed = 1.5f;
        [SerializeField] float runningSpeed = 4.5f;
        [SerializeField] float sprintSpeed = 7f;
		[SerializeField] float rotationSpeed = 10;
        [SerializeField] int sprintingStaminaCost = 2;

        [Header("Jump")]
		[SerializeField] float jumpStaminaCost = 25;
		[SerializeField] float jumpHeight = 4;
        [SerializeField] float jumpForwardSpeed = 5;
        [SerializeField] float freeFallSpeed = 2;
        private Vector3 jumpDirection;

		[Header("Dodge")]
        private Vector3 rollDirection;
        [SerializeField] float dodgeStaminaCost = 25;

		protected override void Awake()
		{
			base.Awake();

            player = GetComponent<PlayerManager>();
		}

        public void HandleAllMovement() 
        {
			HandleGroundedMovement();
            HandleRotation();
            HandleJumpingMovement();
            HandleFreeFallMovement();
		}
       
        private void GetVerticalAndHorizontalMovement()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;

            moveAmount = PlayerInputManager.instance.moveAmount;
		}

		private void HandleGroundedMovement()
        {
			if (!player.canMove)
			{
				return;
			}

			GetVerticalAndHorizontalMovement();
			moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (player.isSprinting)
            {
				player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
			}
            else
            {
				// Check if the player is sprinting
				if (PlayerInputManager.instance.moveAmount > 0.5f)
				{
					player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
				}
				// Check if the player is walking
				else if (PlayerInputManager.instance.moveAmount <= 0.5f && PlayerInputManager.instance.moveAmount > 0)
				{
					player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
				}
			}
        }

        private void HandleJumpingMovement()
        {
            if (player.isJumping)
            {
                player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
			}

		}

        private void HandleFreeFallMovement()
        {
            if(!player.isGrounded)
            {
                Vector3 freeFallDirection;

                freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
                freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
                freeFallDirection.y = 0;

                player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            if (!player.canRotate)
                return;

            if (player.isLockOn && player.playerCombatManager.currentTarget != null)
            {
				if (player.isSprinting)
				{
					Vector3 targetDirection = PlayerCamera.instance.transform.forward * verticalMovement;
					targetDirection += PlayerCamera.instance.transform.right * horizontalMovement;
					targetDirection.y = 0;
					targetDirection.Normalize();

					if (targetDirection == Vector3.zero)
						targetDirection = transform.forward;

					Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
					transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
				}
				else
				{
					Vector3 targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
					targetDirection.y = 0;
					targetDirection.Normalize();

					Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
					transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
				}
			}
            else
            {
				// BUILD CAMERA-RELATIVE INPUT DIRECTION
				Vector3 inputDirection = PlayerCamera.instance.transform.forward * verticalMovement;
				inputDirection += PlayerCamera.instance.transform.right * horizontalMovement;
				inputDirection.y = 0f;

				// IF THERE IS MEANINGFUL INPUT, UPDATE THE TARGET ROTATION DIRECTION AND NORMALIZE IT.
				// OTHERWISE KEEP THE PREVIOUS TARGETROTATIONDIRECTION SO THE PLAYER DOESN'T SNAP BACK.
				const float inputThreshold = 0.0001f;
				if (inputDirection.sqrMagnitude > inputThreshold)
				{
					targetRotationDirection = inputDirection.normalized;
				}

				// ENSURE WE HAVE A VALID FALLBACK DIRECTION ON FIRST FRAME
				if (targetRotationDirection == Vector3.zero)
				{
					targetRotationDirection = transform.forward;
				}

				Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
				Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
				transform.rotation = targetRotation;
			}
		}

        public void HandleSprinting()
        {
            if (player.isPerformingAction)
            {
                player.isSprinting = false;
			}

			// IF WE OUT OF STAMINA, WE CAN'T SPRINT
            if(player.currentStamina <= 0)
            {
                player.isSprinting = false;
                return; 
            }

			// IF WE ARE MOVING WE CAN SPRINT, OTHERWISE WE CAN'T SPRINT
			if (moveAmount >= 0.5f)
            {
                player.isSprinting = true;
            }
		    else
            {
                player.isSprinting = false;
			}

            if (player.isSprinting)
            {
                player.currentStamina -= sprintingStaminaCost * Time.deltaTime;
            }
		}

		public void AttemptToPerformDodge()
        {
            if (player.isPerformingAction)
                return;

            if (player.currentStamina <= 0)
                return;

			if (PlayerInputManager.instance.moveAmount > 0)
			{
				rollDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
				rollDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
				rollDirection.y = 0;
				rollDirection.Normalize();

				Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
				player.transform.rotation = playerRotation;
                
                player.playerAnimatorManager.PlayTargetAnimation("Roll_Forward_01", true, true);
			}
            else
            {
                player.playerAnimatorManager.PlayTargetAnimation("Back_Step_01", true, true);
			}

            player.currentStamina -= dodgeStaminaCost;
		}

        public void AttemptToPerformJump()
        {
			// IF WE ARE PERFORMING AN ACTION, WE CAN'T JUMP, SO RETURN EARLY
			if (player.isPerformingAction)
				return;
			// IF WE ARE OUT OF STAMINA, WE CAN'T JUMP, SO RETURN EARLY
			if (player.currentStamina <= 0)
				return;
			// IF WE ARE ALREADY JUMPING, WE CAN'T JUMP, SO RETURN EARLY
			if (player.isJumping)
                return;
			// IF WE ARE GROUNDED, WE CAN JUMP, SO RETURN EARLY
			if (!player.isGrounded)
                return;

            player.playerAnimatorManager.PlayTargetAnimation("Main_Jump_01", false);

            player.isJumping = true;

			player.currentStamina -= jumpStaminaCost;

            jumpDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            jumpDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
            
            jumpDirection.y = 0;

            if (jumpDirection != Vector3.zero)
            {
				// IF WE ARE SPRINTING, JUMP DIRECTION IS FULL DIRECTION
				if (player.isSprinting)
				{
					jumpDirection *= 1;
				}
				// IF WE ARE RUNNING, JUMP DIRECTION IS HALF DIRECTION
				else if (PlayerInputManager.instance.moveAmount > 0.5f)
				{
					jumpDirection *= 0.5f;
				}
				// IF WE ARE WALKING, JUMP DIRECTION IS QUARTER DIRECTION
				else if (PlayerInputManager.instance.moveAmount <= 0.5f && PlayerInputManager.instance.moveAmount > 0)
				{
					jumpDirection *= 0.25f;
				}
			}
		}

        public void ApplyJumpingVelocity()
        {
			yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
		}
	}
}
