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
			if (!aiCharacter.isGrounded)
				return;

			Vector3 velocity = aiCharacter.animator.deltaPosition;

			aiCharacter.characterController.Move(velocity);
			aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;
		}
	}
}