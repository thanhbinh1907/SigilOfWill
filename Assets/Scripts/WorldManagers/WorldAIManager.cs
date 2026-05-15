using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SG
{
	public class WorldAIManager : MonoBehaviour
	{
		public static WorldAIManager instance;

		[Header("Characters")]
		[SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
		[SerializeField] List<GameObject> spawnedCharacters;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
		{
			aiCharacterSpawners.Add(aiCharacterSpawner);
			aiCharacterSpawner.AttemptToSpawnCharacter();
		}

		public void AddSpawnedCharacter(GameObject character)
		{
			if (!spawnedCharacters.Contains(character)) 
			{
				spawnedCharacters.Add(character);
			}
		}

		private void DespawnAllAICharacters()
		{
			foreach (var character in spawnedCharacters)
			{
				if (character != null) 
				{
					Destroy(character);
				}
			}
			spawnedCharacters.Clear();
		}
	}
}