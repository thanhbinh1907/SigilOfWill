using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SG
{
	public class WorldAIManager : MonoBehaviour
	{
		public static WorldAIManager instance;

		[Header("Debug")]
		[SerializeField] bool spawnAICharacters = false;
		[SerializeField] bool despawnAICharacters = false;

		[Header("Characters")]
		[SerializeField] GameObject[] aiCharacters;
		[SerializeField] List<GameObject> spawnedCharacters = new List<GameObject>();

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

			// WE ONLY WANT TO SPAWN THE CHARACTERS IF WE ARE IN THE WORLD SCENE, OTHERWISE WE WILL SPAWN THEM AGAIN WHEN WE LOAD THE WORLD SCENE
			if (SceneManager.GetActiveScene().buildIndex == WorldSaveGameManager.instance.worldSceneIndex)
			{
				StartCoroutine(WaitForSceneToLoadThenSpawnCharacter());
			}
		}

		private void Update()
		{
			if (spawnAICharacters)
			{
				spawnAICharacters = false;
				 SpawnAllAICharacters();
			}
			if (despawnAICharacters)
			{
				despawnAICharacters = false;
				DespawnAllAICharacters();
			}
		}

		private IEnumerator WaitForSceneToLoadThenSpawnCharacter()
		{
			while (!SceneManager.GetActiveScene().isLoaded)
			{
				yield return new WaitForSeconds(0.5f);
			}
			SpawnAllAICharacters();
		}

		private void SpawnAllAICharacters()
		{
			foreach (var character in aiCharacters)
			{
				GameObject spawnedCharacter = Instantiate(character);
				spawnedCharacters.Add(spawnedCharacter);
			}
		}

		private void DespawnAllAICharacters()
		{
			foreach (var character in spawnedCharacters)
			{
				Destroy(character);
			}
			spawnedCharacters.Clear();
		}

		private void DisableAllAICharacters()
		{
			// CAN BE USE TO DISABLE CHARACTERS THAT ARE FAR AWAY FROM THE PLAYER TO SAVE PERFORMANCE, AND THEN ENABLE THEM AGAIN WHEN THE PLAYER GETS CLOSER TO THEM
			// CHARACTER CAN BE SPLIT INTO AREAS
		}
	}
}