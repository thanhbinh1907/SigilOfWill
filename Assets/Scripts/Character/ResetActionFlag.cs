using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class ResetActionFlag : StateMachineBehaviour
    {
		CharacterManager character;
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			if (character == null)
			{
				character = animator.GetComponentInParent<CharacterManager>();
			}
			
			// THIS CALL WHEN ACTION END
			character.isPerformingAction = false;
			character.applyRootMotion = false;
			character.canRotate = true;
			character.canMove = true;
		}
	}
}