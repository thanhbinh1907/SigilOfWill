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

                // WHY ARE WE INSTANTIATING A COPY OF THIS, INSTEAD OF JUST USING IT AS IT IS
                InstantCharacterEffect effect = Instantiate(effectToTest);
				ProcessInstantEffect(effect);
			}
		}
	}
}