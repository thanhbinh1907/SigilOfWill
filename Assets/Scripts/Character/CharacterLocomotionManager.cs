using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace SG
{
	public class CharacterLocomotionManager : MonoBehaviour
	{
		CharacterManager character;

		[Header("Grounded Check & Jumping")]
		[SerializeField] protected float gravityForce = -5.55f;                   // THE FORCE AT WHICH THE CHARACTER WILL BE PULLED DOWN TO THE GROUND
		[SerializeField] LayerMask groundLayer;                                           
		[SerializeField] float groundedCheckSphereRadius = 1;           
		[SerializeField] protected Vector3 yVelocity;						// THE FORCE AT WHICH THE CHARACTER WILL BE PULLED DOWN TO THE GROUND
		[SerializeField] protected float groundedYVelocity = -20;			// THE Y VELOCITY VALUE TO BE APPLIED WHEN THE CHARACTER IS GROUNDED 
		[SerializeField] protected float fallStartYVelocity = -5;           // THE FORCE AT WHICH THE CHARACTER WILL START TO FALL WHEN THEY STEP OFF A LEDGE (RISE IF THEY FALL LONGER)
		protected bool fallingVelocityHasBeenSet = false;                   // A FLAG TO CHECK IF THE FALLING VELOCITY HAS BEEN SET TO PREVENT IT FROM BEING SET EVERY FRAME
		protected float inAirTimer = 0;                                     // A TIMER TO TRACK HOW LONG THE CHARACTER HAS BEEN IN THE AIR

		protected virtual void Awake()
		{	
			if (character == null)
			{
				character = GetComponent<CharacterManager>();
			}
		}

		protected virtual void Update()
		{
			HandleGroundCheck();

			if (character.isGrounded)
			{
				// if we are not attempting to jump and we are grounded, set the y velocity to the grounded y velocity
				if (yVelocity.y < 0)
				{
					inAirTimer = 0;
					fallingVelocityHasBeenSet = false;
					yVelocity.y = groundedYVelocity;
				}
			}
			else
			{
				if (!character.isJumping && !fallingVelocityHasBeenSet)
				{
					yVelocity.y = fallStartYVelocity;
					fallingVelocityHasBeenSet = true;
				}
				inAirTimer += Time.deltaTime;
				character.animator.SetFloat("inAirTimer", inAirTimer);

				yVelocity.y += gravityForce * Time.deltaTime;
			}
			// THERE SHOULD ALWAYS BE SOME FORCE APPLIED TO THE Y VELOCITY
			character.characterController.Move(yVelocity * Time.deltaTime);
		}

		protected void HandleGroundCheck()
		{
			character.isGrounded = Physics.CheckSphere(character.transform.position, groundedCheckSphereRadius, groundLayer);
		}

		// DRAW THE SPHERE USED FOR THE GROUNDED CHECK IN THE SCENE VIEW
		protected void OnDrawGizmosSelected()
		{
			Gizmos.DrawSphere(character.transform.position, groundedCheckSphereRadius);
		}
	}
}