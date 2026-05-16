using UnityEngine;
using System.Collections;


namespace SG
{
	public class AIBossCharacterManager : AICharacterManager
	{
		public int bossID = 0;
		[SerializeField] bool hasBeenDefeated = false;
		// WHEN THIS A.I SPAWNED, CHECK OUT SAVE FILE 
		// IF THE SAVE FILE DOES NOT CONTAIN A BOSS MONSTER WITH THIS ID, ADD IT
		// IF IT IS PRESENT, CHECK IF THE BOSS HAS BEEN DEFEATED
		// IF THE BOSS HAS BEEN DEFEATED, DISABLE THE BOSS MONSTER AND ITS COMPONENTS
		// IF THE BOSS HAS NOT BEEN DEFEATED, ALLOW THIS OBJECT TO CONTINUE TO BE ACTIVE

		protected override void Start()
		{
			base.Start();

			// IF OUR SAVE DATA DOES NOT CONTAIN THE BOSS ID, ADD IT 
			if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
			{
				WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
				WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
			}
			// OTHERWISE, LOAD DATA THAT IS ALREADY EXISTING ON THIS BOSS
			else
			{
				hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];

				if (hasBeenDefeated)
				{
					gameObject.SetActive(false);
				}
			}
		}

		public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
		{
			_currentHealth = 0;
			isDead = true;

			// RESET ANY FLAGS HERE THAT NEED TO BE RESET
			// NOTHING YET

			// IF WE ARE NOT GROUNDED,  PLAY AN AERIAL DEATH ANIMATION

			if (!manuallySelectDeathAnimation)
			{
				characterAnimatorManager.PlayTargetAnimation("Dead_01", true);
			}

			hasBeenDefeated = true;

			WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID] = true;
			WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID] = true;

			WorldSaveGameManager.instance.SaveGame();

			// PLAY SOME DEATH SFX

			yield return new WaitForSeconds(5);

			// AWARD PLAYER WITH RUNES

			// DISABLE CHARACTER CONTROLLER

			gameObject.SetActive(false);
		}
	}
}
