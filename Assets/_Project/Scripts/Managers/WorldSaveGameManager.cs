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
		public static System.Action OnNoFreeCharacterSlotsAvailable;

		public PlayerManager player;

		[Header("SAVE/LOAD")]
		[SerializeField] bool saveGame;
		[SerializeField] bool loadGame;

		[Header("World Scene Index")]
		[SerializeField] public int worldSceneIndex = 2;

		[Header("Starting Position")]
		[SerializeField] public Vector3 startingPosition = Vector3.zero;

		[Header("Save Data Writer")]
		private SaveFileDataWriter saveFileDataWriter;

		[Header("Current Character Data")]
		public CharacterSlot currentCharacterSlotBeingUsed;
		public CharacterSaveData currentCharacterData;
		private string saveFileName;

		[Header("Loading Status")]
		public bool isSceneLoading = false;
		[SerializeField] private CanvasGroup loadingScreenCanvasGroup;
		[SerializeField] private float minimumLoadingTime = 5f;
		[SerializeField] private List<Sprite> loadingScreenBackgrounds = new List<Sprite>();
		[SerializeField] private UnityEngine.UI.Image loadingScreenBackgroundImage;
		[SerializeField] private CanvasGroup loadingIconCanvasGroup;

		[Header("Character Slots")]
		public CharacterSaveData characterSlot01;
		public CharacterSaveData characterSlot02;
		public CharacterSaveData characterSlot03;
		public CharacterSaveData characterSlot04;
		public CharacterSaveData characterSlot05;
		public CharacterSaveData characterSlot06;
		public CharacterSaveData characterSlot07;
		public CharacterSaveData characterSlot08;
		public CharacterSaveData characterSlot09;
		public CharacterSaveData characterSlot10;


		[SerializeField] GameObject playerPrefab;

		private void Awake()
		{
			isSceneLoading = false;

			// IF THE INSTANCE IS NULL, SET IT TO THIS INSTANCE. OTHERWISE, DESTROY THIS GAME OBJECT TO ENFORCE THE SINGLETON PATTERN.
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
			if (instance != this)
			{
				return;
			}
			DontDestroyOnLoad(gameObject);
			LoadAllCharacterProfiles();


			StartCoroutine(PeriodicAutoSave());
		}

		private IEnumerator PeriodicAutoSave()
		{
			while (true)
			{
				yield return new WaitForSeconds(10f);


				if (!isSceneLoading && player != null && !player.isDead)
				{
					SaveGame();
				}
			}
		}

		private void OnApplicationQuit()
		{
			if (player != null && !isSceneLoading)
			{
				SaveGame();
			}
		}

		private void Update()
		{
			if (saveGame)
			{
				saveGame = false;
				SaveGame();
			}
			if (loadGame)
			{
				loadGame = false;
				LoadGame();
			}
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		public string GetDefaultCharacterNameBasedOnCharacterSlot(CharacterSlot characterSlot)
		{
			switch (characterSlot)
			{
				case CharacterSlot.CharacterSlot_01:
					return "Character Slot 1";
				case CharacterSlot.CharacterSlot_02:
					return "Character Slot 2";
				case CharacterSlot.CharacterSlot_03:
					return "Character Slot 3";
				case CharacterSlot.CharacterSlot_04:
					return "Character Slot 4";
				case CharacterSlot.CharacterSlot_05:
					return "Character Slot 5";
				case CharacterSlot.CharacterSlot_06:
					return "Character Slot 6";
				case CharacterSlot.CharacterSlot_07:
					return "Character Slot 7";
				case CharacterSlot.CharacterSlot_08:
					return "Character Slot 8";
				case CharacterSlot.CharacterSlot_09:
					return "Character Slot 9";
				case CharacterSlot.CharacterSlot_10:
					return "Character Slot 10";
				default:
					return "Character";
			}
		}

		public string DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot characterSlot)
		{
			string fileName = "";
			switch (characterSlot)
			{
				case CharacterSlot.CharacterSlot_01:
					fileName = "characterSlot_01";
					break;
				case CharacterSlot.CharacterSlot_02:
					fileName = "characterSlot_02";
					break;
				case CharacterSlot.CharacterSlot_03:
					fileName = "characterSlot_03";
					break;
				case CharacterSlot.CharacterSlot_04:
					fileName = "characterSlot_04";
					break;
				case CharacterSlot.CharacterSlot_05:
					fileName = "characterSlot_05";
					break;
				case CharacterSlot.CharacterSlot_06:
					fileName = "characterSlot_06";
					break;
				case CharacterSlot.CharacterSlot_07:
					fileName = "characterSlot_07";
					break;
				case CharacterSlot.CharacterSlot_08:
					fileName = "characterSlot_08";
					break;
				case CharacterSlot.CharacterSlot_09:
					fileName = "characterSlot_09";
					break;
				case CharacterSlot.CharacterSlot_10:
					fileName = "characterSlot_10";
					break;
				default:
					break;

			}
			return fileName;
		}

		public CharacterSaveData GetCharacterSaveDataBasedOnCharacterSlot(CharacterSlot characterSlot)
		{
			switch (characterSlot)
			{
				case CharacterSlot.CharacterSlot_01:
					return characterSlot01;
				case CharacterSlot.CharacterSlot_02:
					return characterSlot02;
				case CharacterSlot.CharacterSlot_03:
					return characterSlot03;
				case CharacterSlot.CharacterSlot_04:
					return characterSlot04;
				case CharacterSlot.CharacterSlot_05:
					return characterSlot05;
				case CharacterSlot.CharacterSlot_06:
					return characterSlot06;
				case CharacterSlot.CharacterSlot_07:
					return characterSlot07;
				case CharacterSlot.CharacterSlot_08:
					return characterSlot08;
				case CharacterSlot.CharacterSlot_09:
					return characterSlot09;
				case CharacterSlot.CharacterSlot_10:
					return characterSlot10;
				default:
					return null;
			}
		}

		public void AttemptToCreateNewGame()
		{
			if (isSceneLoading)
				return;

			saveFileDataWriter = new SaveFileDataWriter();
			saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				// IF THE FILE EXIST, WE CAN'T CREATE A NEW GAME, OTHERWISE, WE CAN CREATE A NEW GAME
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_01;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_02;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_03;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_04;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_05;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_06;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_07;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_08;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_09;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// CHECK TO SEE IF WE CAN CREATE A NEW FILE SAVE (CHECK FOR OTHER EXISTING FILE FIRST)
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
			if (!saveFileDataWriter.CheckToSeeIfFileExists())
			{
				currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_10;
				currentCharacterData = new CharacterSaveData();
				NewGame();
				return;
			}

			// IF THERE ARE NO FREE SLOT, NOTIFY PLAYER
			OnNoFreeCharacterSlotsAvailable?.Invoke();
		}

		private void NewGame()
		{
			isSceneLoading = true;


			if (currentCharacterData != null)
			{
				currentCharacterData.characterName = GetDefaultCharacterNameBasedOnCharacterSlot(currentCharacterSlotBeingUsed);
				currentCharacterData.sceneIndex = worldSceneIndex;
				currentCharacterData.xPosition = startingPosition.x;
				currentCharacterData.yPosition = startingPosition.y;
				currentCharacterData.zPosition = startingPosition.z;
			}

			// SAVE THE NEWLY CREATED CHARACTER STATS, AND ITEM (WHEN CREATION SCREEN IS ADDED)
			SaveGame();
			StartCoroutine(LoadWorldScene());
		}

		public void LoadGame()
		{
			if (isSceneLoading)
				return;

			isSceneLoading = true;

			saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

			saveFileDataWriter = new SaveFileDataWriter();

			//  GENERALLY WORK ON MULTIPLE PLATFORMS
			saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
			saveFileDataWriter.saveFileName = saveFileName;
			currentCharacterData = saveFileDataWriter.LoadSaveFile();

			StartCoroutine(LoadWorldScene());
		}

		public void SaveGame()
		{

			if (WorldAIManager.instance != null && WorldAIManager.instance.IsAnyBossFightActive())
			{
				Debug.Log("[HỆ THỐNG LƯU] Không thể lưu game khi đang chiến đấu với Boss!");
				return;
			}

			// SAVE CURRENT FILE UNDER A FILE NAME DEPEND ON WHICH SLOT WE ARE USING
			saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);
			saveFileDataWriter = new SaveFileDataWriter();

			//  GENERALLY WORK ON MULTIPLE PLATFORMS
			saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
			saveFileDataWriter.saveFileName = saveFileName;


			if (player != null && !player.isDead)
			{
				player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);
			}

			saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
		}

		public void DeleteGame(CharacterSlot characterSlot)
		{
			// CHOOSE FILE BASE ON NAME
			saveFileDataWriter = new SaveFileDataWriter();
			saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);

			saveFileDataWriter.DeleteSaveFile();
		}

		// LOAD ALL CHARACTER PROFILE ON DEVICE WHEN STARTING GAME
		public void LoadAllCharacterProfiles()
		{
			saveFileDataWriter = new SaveFileDataWriter();
			saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot01 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot01 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot02 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot02 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot03 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot03 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot04 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot04 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot05 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot05 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot06 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot06 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot07 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot07 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot08 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot08 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot09 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot09 = null;
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot10 = saveFileDataWriter.LoadSaveFile();
			}
			else
			{
				characterSlot10 = null;
			}
		}

		public IEnumerator LoadWorldScene()
		{
			// Show loading screen and disable inputs
			if (loadingScreenCanvasGroup != null)
			{
				// Randomize background sprite
				if (loadingScreenBackgroundImage != null && loadingScreenBackgrounds != null && loadingScreenBackgrounds.Count > 0)
				{
					int randomIndex = Random.Range(0, loadingScreenBackgrounds.Count);
					loadingScreenBackgroundImage.sprite = loadingScreenBackgrounds[randomIndex];
				}

				loadingScreenCanvasGroup.gameObject.SetActive(true);
				loadingScreenCanvasGroup.alpha = 1f;
				loadingScreenCanvasGroup.blocksRaycasts = true;
				loadingScreenCanvasGroup.interactable = true;
			}


			Time.timeScale = 0f;

			// Start pulsing loading icon if present
			Coroutine pulseCoroutine = null;
			if (loadingIconCanvasGroup != null)
			{
				pulseCoroutine = StartCoroutine(PulseLoadingIcon());
			}

			float loadingStartTime = Time.realtimeSinceStartup;

			// 1. LOAD WORLD SCENE
			// IF WE WANT TO USE DIFFERENT SCENE FOR LEVELS IN OUR PROJECT USE THIS
			AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);

			yield return loadOperation;

			// 2. SPAWN PLAYER INTO THE WORLD
			if (player == null)
			{
				if (playerPrefab != null)
				{
					GameObject playerObj = Instantiate(playerPrefab);
					player = playerObj.GetComponent<PlayerManager>();
				}
			}

			// 3. WAIT UNTIL THE PLAYER HAS BEEN SPAWNED INTO THE WORLD BEFORE WE ATTEMPT TO LOAD DATA ONTO THE PLAYER
			yield return new WaitUntil(() => player != null);

			// 4. LOAD THE PLAYER'S DATA ONTO THE PLAYER
			player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

			// Ensure the loading screen is displayed for at least minimumLoadingTime to mask loading lag
			float timeElapsed = Time.realtimeSinceStartup - loadingStartTime;
			if (timeElapsed < minimumLoadingTime)
			{
				yield return new WaitForSecondsRealtime(minimumLoadingTime - timeElapsed);
			}

			// Fade out loading screen smoothly
			if (loadingScreenCanvasGroup != null)
			{
				float fadeDuration = 0.5f;
				float fadeTimer = 0f;
				while (fadeTimer < fadeDuration)
				{
					fadeTimer += Time.unscaledDeltaTime;
					loadingScreenCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
					yield return null;
				}
				loadingScreenCanvasGroup.gameObject.SetActive(false);
				loadingScreenCanvasGroup.blocksRaycasts = false;
				loadingScreenCanvasGroup.interactable = false;
			}


			Time.timeScale = 1f;

			if (pulseCoroutine != null)
			{
				StopCoroutine(pulseCoroutine);
			}

			isSceneLoading = false;
		}

		private IEnumerator PulseLoadingIcon()
		{
			if (loadingIconCanvasGroup == null)
				yield break;

			loadingIconCanvasGroup.alpha = 0.1f;
			float speed = 2.5f; // speed of pulsing
			while (isSceneLoading)
			{
				// Pulse alpha between 0.1f and 1f
				loadingIconCanvasGroup.alpha = Mathf.PingPong(Time.unscaledTime * speed, 0.9f) + 0.1f;
				yield return null;
			}
		}

		public void RespawnPlayer()
		{
			if (isSceneLoading)
				return;

			isSceneLoading = true;
			StartCoroutine(LoadWorldScene());
		}

		public int GetWorldSceneIndex()
		{
			if (currentCharacterData != null)
			{
				return currentCharacterData.sceneIndex;
			}
			return worldSceneIndex;
		}
	}
}
