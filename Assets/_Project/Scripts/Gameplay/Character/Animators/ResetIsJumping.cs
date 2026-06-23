using SG;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class ResetIsJumping : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CharacterManager character = animator.GetComponentInParent<CharacterManager>();

            if (character != null)
            {
                // THIS CALL WHEN ACTION END
                character.isJumping = false;
            }
        }
    }
}