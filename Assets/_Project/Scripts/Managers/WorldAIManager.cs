using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

namespace SG
{
	public class WorldAIManager : MonoBehaviour
	{
		public static WorldAIManager instance;

		[Header("Characters")]
		[SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
		[SerializeField] List<GameObject> spawnedCharacters;

		[Header("Bosses")]
		[SerializeField] List<AIBossCharacterManager> spawnedInBosses = new List<AIBossCharacterManager>();

		public void RegisterBoss(AIBossCharacterManager boss)
		{
			if (!spawnedInBosses.Contains(boss))
			{
				spawnedInBosses.Add(boss);
			}
		}

		public void UnregisterBoss(AIBossCharacterManager boss)
		{
			if (spawnedInBosses.Contains(boss))
			{
				spawnedInBosses.Remove(boss);
			}
		}

		public AIBossCharacterManager GetBossCharacterByID(int id)
		{
			return spawnedInBosses.FirstOrDefault(boss => boss.bossID == id);
		}

		public bool IsAnyBossFightActive()
		{
			foreach (var boss in spawnedInBosses)
			{
				if (boss != null && boss.bossFightIsActive)
				{
					return true;
				}
			}
			return false;
		}

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

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (aiCharacterSpawners != null)
			{
				aiCharacterSpawners.Clear();
			}
			if (spawnedCharacters != null)
			{
				spawnedCharacters.Clear();
			}
			if (spawnedInBosses != null)
			{
				spawnedInBosses.Clear();
			}
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
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

		public void ResetAllCharacters()
		{

			DespawnAllAICharacters();


			AICharacterSpawner[] allSpawners = FindObjectsByType<AICharacterSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None);


			foreach (var spawner in allSpawners)
			{
				if (spawner != null)
				{
					spawner.AttemptToSpawnCharacter();
				}
			}

			Debug.Log("[HỆ THỐNG CO-OP OFFLINE] Toàn bộ quái vật thường trên bản đồ đã được dọn dẹp sạch sẽ và hồi sinh trở lại tọa độ gốc thành công!");
		}
	}
}