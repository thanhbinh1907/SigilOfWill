using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete Later")]
        [SerializeField] InstantCharacterEffect effectToTest;
        [SerializeField] bool processEffect = false;

		private void Update()
		{
            if (processEffect)
            {
                processEffect = false;

				// WHEN WE INSTANTIATE IT, THE ORIGINAL IS NOT EFFECTED
				// TakeStaminaDamageEffect effect = Instantiate(effectToTest) as TakeStaminaDamageEffect;
				// effect.staminaDamage = 55;

				// WHEN WE DONT INSTANTIATE IT, THE ORIGINAL IS CHANGED 
				// effectToTest.staminaDamage = 55;

				InstantCharacterEffect effect = Instantiate(effectToTest);

				ProcessInstantEffect(effect);
			}
		}
	}
}