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

		public PlayerManager player;

		[Header("SAVE/LOAD")]
		[SerializeField] bool saveGame;
		[SerializeField] bool loadGame;

		[Header("World Scene Index")]
		[SerializeField] public int worldSceneIndex = 1;

		[Header("Save Data Writer")]
		private SaveFileDataWriter saveFileDataWriter;

		[Header("Current Character Data")]
		public CharacterSlot currentCharacterSlotBeingUsed;
		public CharacterSaveData currentCharacterData;
		private string saveFileName;

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
			DontDestroyOnLoad(gameObject);
			LoadAllCharacterProfiles();
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

		public void AttemptToCreateNewGame()
		{
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
			TitleScreenManager.instance.DisplayNoFreeCharacterSlotPopUp();
		}

		private void NewGame()
		{
			// SAVE THE NEWLY CREATED CHARACTER STATS, AND ITEM (WHEN CREATION SCREEN IS ADDED)
			SaveGame();
			StartCoroutine(LoadWorldScene());
		}

		public void LoadGame()
		{
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
			// SAVE CURRENT FILE UNDER A FILE NAME DEPEND ON WHICH SLOT WE ARE USING
			saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);
			saveFileDataWriter = new SaveFileDataWriter();

			//  GENERALLY WORK ON MULTIPLE PLATFORMS
			saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
			saveFileDataWriter.saveFileName = saveFileName;

			// PASS THE PLAYER INFO, FROM GAME, TO THEIR SAVE FILE
			if (player != null)
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
		private void LoadAllCharacterProfiles()
		{
			saveFileDataWriter = new SaveFileDataWriter();
			saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot01 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot02 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot03 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot04 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot05 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot06 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot07 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot08 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot09 = saveFileDataWriter.LoadSaveFile();
			}

			saveFileDataWriter.saveFileName = DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
			if (saveFileDataWriter.CheckToSeeIfFileExists())
			{
				characterSlot10 = saveFileDataWriter.LoadSaveFile();
			}
		}

		public IEnumerator LoadWorldScene()
		{
			// 1. LOAD WORLD SCENE
			// IF WE JUST WANT 1 WORLD SCENE USE THIS
			AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

			// IF WE WANT TO USE DIFFERENT SCENE FOR LEVELS IN OUR PROJECT USE THIS
			//AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);

			yield return loadOperation;

			// 2. SPAWN PLAYER INTO THE WORLD
			if (playerPrefab != null)
			{
				GameObject playerObj = Instantiate(playerPrefab);
				player = playerObj.GetComponent<PlayerManager>();
			}

			// 3. WAIT UNTIL THE PLAYER HAS BEEN SPAWNED INTO THE WORLD BEFORE WE ATTEMPT TO LOAD DATA ONTO THE PLAYER
			yield return new WaitUntil(() => player != null);

			// 4. LOAD THE PLAYER'S DATA ONTO THE PLAYER
			player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
		}

		// IF YOU WANT TO USE A MULTI SCENE SETUP, THERE IS NO CURRENT SCENE INDEX ON A NEW CHARACTER  
		/*
		private IEnumerable LoadWorldSceneNewGame()
		{

		}
		*/
		public int GetWorldSceneIndex()
		{
			return worldSceneIndex;
		}
	}
}
