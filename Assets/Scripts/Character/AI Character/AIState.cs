using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class AIState : ScriptableObject
    {
        public virtual AIState Tick(AICharacterManager aiCharacter)
        {
			// DO SOME LOGIC TO FIND PLAYER

			// IF WE FIND PLAYER, RETURN A NEW STATE THAT WILL CHASE THE PLAYER

			// IF WE HAVE NOT FOUND PLAYER, CONTINUE TO RETURN THE ILDE STATE

			return this;
		}

		protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
		{
			ResetStateFlags(aiCharacter);
			return newState;
		}

		protected virtual void ResetStateFlags(AICharacterManager aiCharacter) 
		{
			// RESET ANY STATE FLAGS HERE SO WHEN YOU RETURN TO THE STATE, THEY ARE BLANK ONCE AGAIN
		}
	}
}