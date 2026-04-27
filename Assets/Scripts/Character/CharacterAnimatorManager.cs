using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class CharacterAnimatorManager : MonoBehaviour
	{
		CharacterManager character;

		int horizontal;
		int vertical;

		protected virtual void Awake()
		{
			character = GetComponent<CharacterManager>();

			horizontal = Animator.StringToHash("Horizontal");
			vertical = Animator.StringToHash("Vertical");
		}

		public void UpdateAnimatorMovementParameters(float horizontalParameters, float verticalParameters, bool isSprinting)
		{
			float horizontalAmout = horizontalParameters;
			float verticalAmout = verticalParameters;

			if (isSprinting)
			{
				verticalAmout = 2;
			}
			character.animator.SetFloat(horizontal, horizontalAmout, 0.1f, Time.deltaTime);
			character.animator.SetFloat(vertical, verticalAmout, 0.1f, Time.deltaTime);
		}

		public virtual void PlayTargetAnimation(
			string targetAnimation, 
			bool isPerformingAction, 
			bool applyRootMotion = true, 
			bool canRotate = false, 
			bool canMove = false)
		{
			character.applyRootMotion = applyRootMotion;
			character.animator.CrossFade(targetAnimation, 0.2f);
			// can be used to stop character movement during an animation, such as attacking or being hit
			character.isPerformingAction = isPerformingAction;
			character.canRotate = canRotate;
			character.canMove = canMove;
		}
	}
}
