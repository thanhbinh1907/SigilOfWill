using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class AIFrostGiantCharacterManager : AIBossCharacterManager
    {
        public AIFrostGiantSoundFXManager frostGiantSoundFXManager;

		protected override void Awake()
		{
			base.Awake();

			frostGiantSoundFXManager = GetComponent<AIFrostGiantSoundFXManager>();
		}
	}
}