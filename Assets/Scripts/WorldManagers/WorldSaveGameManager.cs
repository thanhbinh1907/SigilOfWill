using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace SG
{
	public class WorldSaveGameManager : MonoBehaviour
	{
		public static WorldSaveGameManager instance;

		public int worldSceneIndex = 1;
		[SerializeField] GameObject playerPrefab;

		private void Awake()
		{
			// If the instance is null, set it to this instance. Otherwise, destroy this game object to enforce the singleton pattern.
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

		public IEnumerator LoadNewGame()
		{
			AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

			while (loadOperation.isDone == false)
			{
				yield return null;
			}
			
			if (playerPrefab != null)
			{
				Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
			}
		}
		public int GertWorldSceneIndex()
		{
			return worldSceneIndex;
		}
	}
}
