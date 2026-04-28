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

		[Header("Stats")]
		public int vitality = 10;
		public int endurance = 10;
	}
}

