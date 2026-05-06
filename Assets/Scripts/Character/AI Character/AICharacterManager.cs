using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class AICharacterManager : CharacterManager
    {
        [Header("Current State")]
        [SerializeField] AIState currentState;

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
            ProcessStateMachine();
		}

        private void ProcessStateMachine()
        {
            AIState nextState = currentState?.Tick(this);

            if (nextState != null)
            {
                currentState = nextState;
            }
        }
	}
}