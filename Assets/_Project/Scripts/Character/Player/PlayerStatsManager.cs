using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class PlayerStatsManager : CharacterStatsManager
	{
        PlayerManager player;

		protected override void Awake()
		{
			base.Awake();
			player = GetComponent<PlayerManager>();
		}

		protected override void Start()
		{
			base.Start();

			// WHY CALCULATE HERE ?
			// WHEN WE MAKE A CHARACTER CREATION MENU, AND SET THE STATS DEPENDING ON THE CLASS, THIS WILL BE CALCULATED THERE
			// UNTIL THEN HOWEVER, STATS ARE NEVER CALCULATED, SO WE DO IS HERE ON START, IF A SAVE FILE EXISTS THEY WILL BE OVER WRITTEN WHEN LOADING TO SCENE
			CalculateHealthBasedOnVitalityLevel(player.vitality);
			CalculateStaminaBasedOnEnduranceLevel(player.endurance);
			CalculateManaBasedOnIntelligenceLevel(player.intelligence);
		}
	}
}