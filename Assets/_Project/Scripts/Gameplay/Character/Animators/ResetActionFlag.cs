using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class ResetActionFlag : StateMachineBehaviour
    {
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			CharacterManager character = animator.GetComponentInParent<CharacterManager>();

			if (character == null) return;

			// THIS CALL WHEN ACTION END
			character.isPerformingAction = false;
			character.applyRootMotion = false;
			character.canRotate = true;
			character.canMove = true;
			character.isJumping = false;
			character.isInvulnerable = false;
		}
	}
}