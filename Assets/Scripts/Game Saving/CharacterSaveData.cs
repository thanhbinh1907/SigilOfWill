using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	[System.Serializable]
	// since we wamt to reference this class for every save file, this script is not a monobehaviour, and is instead serializable
	public class CharacterSaveData
    {
		[Header("SCENE INDEX")]
		public int sceneIndex;

		[Header("Character Name")]
		public string characterName = "Character";

		[Header("Time Played")]
		public float secondsPlayed;

		[Header("World Coordinates")]
		public float xPosition;
		public float yPosition;
		public float zPosition;

		[Header("Resources")]
		public int currentHealth;
		public float currentStamina;
		public float currentMana;

		[Header("Stats")]
		public int vitality = 10;
		public int endurance = 10;
		public int intelligence = 10;

		[Header("Equipment")]
		public int currentRightHandWeaponID;
		public int currentLeftHandWeaponID;

		[Header("Bosses")]
		public SerializableDictionary<int, bool> bossesAwakened;
		public SerializableDictionary<int, bool> bossesDefeated;

		[Header("Sites Of Grace")]
		public SerializableDictionary<int, bool> sitesOfGrace;

		[Header("Last Grace Position")]
		public bool hasGraceSaved = false;
		public int lastGraceSceneIndex;
		public float lastGraceXPosition;
		public float lastGraceYPosition;
		public float lastGraceZPosition;

		[Header("World Items Looted")]
		public SerializableDictionary<int, bool> worldItemsLooted;

		[Header("Inventory & Quick Slots")]
		public List<int> itemsInventoryIDs;
		public List<int> weaponsInRightHandSlotsIDs;
		public List<int> weaponsInLeftHandSlotsIDs;
		public int rightHandWeaponIndex;
		public int leftHandWeaponIndex;

		public CharacterSaveData()
		{
			bossesAwakened = new SerializableDictionary<int, bool>();
			bossesDefeated = new SerializableDictionary<int, bool>();
			sitesOfGrace = new SerializableDictionary<int, bool>();
			worldItemsLooted = new SerializableDictionary<int, bool>();
			itemsInventoryIDs = new List<int>();
			weaponsInRightHandSlotsIDs = new List<int>();
			weaponsInLeftHandSlotsIDs = new List<int>();
		}
	}
}

