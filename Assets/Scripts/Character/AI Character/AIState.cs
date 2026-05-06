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
    }
}