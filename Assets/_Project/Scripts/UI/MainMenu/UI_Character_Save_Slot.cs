using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SG
{
    public class UI_Character_Save_Slot : MonoBehaviour
    {
        SaveFileDataWriter saveFileWriter;

        [Header("Game Slot")]
        public CharacterSlot characterSlot;

        [Header("Character Info")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timePlayed;

        private void OnEnable()
        {
            LoadSaveSlot();
        }

        private void LoadSaveSlot()
        {
            if (WorldSaveGameManager.instance == null)
            {
                gameObject.SetActive(false);
                return;
            }

            saveFileWriter = new SaveFileDataWriter();
            saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);

            // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                CharacterSaveData slotData = WorldSaveGameManager.instance.GetCharacterSaveDataBasedOnCharacterSlot(characterSlot);
                if (slotData != null)
                {
                    characterName.text = slotData.characterName;

                    // Format and display time played in HH:mm:ss format
                    System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(slotData.secondsPlayed);
                    timePlayed.text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // if it doesn't exist, disable the save slot
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void LoadGameFromCharacterSaveSlot()
        {
			if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.isSceneLoading)
				return;

            WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
            WorldSaveGameManager.instance.LoadGame();
		}

        public void SelectCurrentSlot()
        {
			if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.isSceneLoading)
				return;

            TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
		}
    }
}