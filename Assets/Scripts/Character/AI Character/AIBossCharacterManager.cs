using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
	public class AIBossCharacterManager : AICharacterManager
	{
		[Header("Boss Settings")]
		public int bossID = 0;
		[SerializeField] bool hasBeenAwakened = false;
		[SerializeField] bool hasBeenDefeated = false;

		[Header("Boss Fog Wall")]
		[SerializeField] private List<FogWallInteractable> myFogWalls = new List<FogWallInteractable>();

		[Header("Test Debug")]
		[SerializeField] bool wakeBossUpDebug = false;
		[SerializeField] bool defeatBossDebug = false;

		protected override void Start()
		{
			base.Start();

			// IF OUR SAVE DATA DOES NOT CONTAIN THE BOSS ID, ADD IT 
			if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
			{
				WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
				WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
				Debug.Log($"[HỆ THỐNG] Đã nhận diện Boss ID {bossID} thành công trên RAM!");
			}
			// OTHERWISE, LOAD DATA THAT IS ALREADY EXISTING ON THIS BOSS
			else
			{
				hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
			}

			StartCoroutine(GetFogWallsFromWorldObjectManager());
		}

		protected override void Update()
		{
			base.Update();

			if (wakeBossUpDebug)
			{
				wakeBossUpDebug = false;
				WakeBoss();
			}
		}

		private IEnumerator GetFogWallsFromWorldObjectManager()
		{
			while (WorldObjectManager.instance == null || WorldObjectManager.instance.fogWalls.Count == 0)
			{
				yield return null;
			}

			myFogWalls.Clear();
			foreach (var fogWall in WorldObjectManager.instance.fogWalls)
			{
				if (fogWall.fogWallID == bossID)
				{
					myFogWalls.Add(fogWall);
				}
			}

			LoadBossAndFogWallStates();
		}

		private void LoadBossAndFogWallStates()
		{
			if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
			{
				var saveData = WorldSaveGameManager.instance.currentCharacterData;

				if (!saveData.bossesAwakened.ContainsKey(bossID))
				{
					saveData.bossesAwakened[bossID] = false;
					saveData.bossesDefeated[bossID] = false;
				}
				else
				{
					hasBeenAwakened = saveData.bossesAwakened[bossID];
					hasBeenDefeated = saveData.bossesDefeated[bossID];
				}

				if (hasBeenDefeated)
				{
					foreach (var fogWall in myFogWalls)
					{
						if (fogWall != null)
						{
							fogWall.IsActive = false;
						}
						gameObject.SetActive(false);
						return;
					}
				}

				if (hasBeenAwakened)
				{
					foreach (var fogWall in myFogWalls)
					{
						if (fogWall != null)
						{
							fogWall.IsActive = true;
						}
					}
				}
				else
				{
					foreach (var fogWall in myFogWalls)
					{
						if (fogWall != null)
						{
							fogWall.IsActive = false;
						}
					}
				}
			}
		}

		public void WakeBoss()
		{
			if (hasBeenDefeated)
				return;

			WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID] = true;
			WorldSaveGameManager.instance.SaveGame();

			foreach (var fogWall in myFogWalls)
			{
				fogWall.IsActive = true;
			}
			Debug.Log($"Boss ID {bossID} đã thức tỉnh! Tường sương mù đã dựng lên!");
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
