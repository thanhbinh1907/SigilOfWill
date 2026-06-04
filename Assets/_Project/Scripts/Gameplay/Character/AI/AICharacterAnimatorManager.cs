using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class AICharacterAnimatorManager : CharacterAnimatorManager
    {
        AICharacterManager aiCharacter;

		override protected void Awake()
		{
			base.Awake();
			aiCharacter = GetComponentInParent<AICharacterManager>();
		}

		private void OnAnimatorMove()
		{
			if (!aiCharacter.isGrounded && !aiCharacter.isPerformingAction)
				return;

			Vector3 velocity = aiCharacter.animator.deltaPosition;

			velocity.y = 0;

			aiCharacter.characterController.Move(velocity);
			aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;
		}
	}
}