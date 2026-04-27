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
            saveFileWriter = new SaveFileDataWriter();
            saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;

            // save slot 1
            if (characterSlot == CharacterSlot.CharacterSlot_01)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);

                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 2
            else if (characterSlot == CharacterSlot.CharacterSlot_02)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);

                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 3
            else if (characterSlot == CharacterSlot.CharacterSlot_03)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);
                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 4
            else if (characterSlot == CharacterSlot.CharacterSlot_04)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);
                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot04.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 5
            else if (characterSlot == CharacterSlot.CharacterSlot_05)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);
                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot05.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 6
            else if (characterSlot == CharacterSlot.CharacterSlot_06)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);
                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot06.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 7
            else if (characterSlot == CharacterSlot.CharacterSlot_07)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);
                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot07.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 8
            else if (characterSlot == CharacterSlot.CharacterSlot_08)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);
                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot08.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 9
            else if (characterSlot == CharacterSlot.CharacterSlot_09)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);
                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot09.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // save slot 10
            else if (characterSlot == CharacterSlot.CharacterSlot_10)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(characterSlot);
                // if the file exist, load the data and display the character name and time played, otherwise, disable the save slot
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot10.characterName;
                }
                // if it doesn't exist, disable the save slot
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void LoadGameFromCharacterSaveSlot()
        {
            WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
            WorldSaveGameManager.instance.LoadGame();
		}

        public void SelectCurrentSlot() 
        {
            TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
		}
    }
}