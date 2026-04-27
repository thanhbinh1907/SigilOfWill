using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.UI;
using UnityEngine.UI;

namespace SG
{
	public class TitleScreenManager : MonoBehaviour
	{
		public static TitleScreenManager instance;

		[Header("Menu")]
		[SerializeField] GameObject titleScreenMainMenu;
		[SerializeField] GameObject titleScreenLoadMenu;

		[Header("Button")]
		[SerializeField] Button loadMenuReturnButton;
		[SerializeField] Button mainMenuLoadGameButton;
		[SerializeField] Button mainMenuNewGameButton;
		[SerializeField] Button deleteCharacterPopUpConfirmButton;

		[Header("Pop Ups")]
		[SerializeField] GameObject noCharacterSlotsPopUp;
		[SerializeField] Button noChacterSlotsOkayButton;
		[SerializeField] GameObject deleteCharacterSlotPopUp;

		[Header("Character Slots")]
		public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;
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

		public void StartNewGame()
		{
			WorldSaveGameManager.instance.AttemptToCreateNewGame();
		}

		public void OpenLoadGameMenu()
		{
			// CLOSE THE MAIN MENU AND OPEN THE LOAD GAME MENU
			titleScreenMainMenu.SetActive(false);
			titleScreenLoadMenu.SetActive(true);
			deleteCharacterSlotPopUp.SetActive(false);

			// SELECT THE RETURN BUTTON FIRST
			loadMenuReturnButton.Select();
		}

		public void CloseLoadGameMenu()
		{
			titleScreenMainMenu.SetActive(true);
			titleScreenLoadMenu.SetActive(false);

			mainMenuLoadGameButton.Select();
		} 

		public void DisplayNoFreeCharacterSlotPopUp()
		{
			noCharacterSlotsPopUp.SetActive(true);
			noChacterSlotsOkayButton.Select();
		}
		 
		public void CloseNoFreeCharacterSlotPopUp()
		{
			noCharacterSlotsPopUp.SetActive(false);
			mainMenuNewGameButton.Select();
		}

		// CHARACTER SLOT

		public void SelectCharacterSlot(CharacterSlot characterSlot)
		{
			currentSelectedSlot = characterSlot;
		}

		public void SelectNoSlot()
		{
			currentSelectedSlot = CharacterSlot.NO_SLOT;
		}

		public void AttemptToDeleteCharacterSlot()
		{
			if (currentSelectedSlot != CharacterSlot.NO_SLOT)
			{
				deleteCharacterSlotPopUp.SetActive(true);
				deleteCharacterPopUpConfirmButton.Select();
			}
		}

		public void DeleteCharacterSlot()
		{
			deleteCharacterSlotPopUp.SetActive(false);
			WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);

			// WE DISABLE AND THEN ENABLE THE LOAD MENU TO REFRESH THE CHARACTER SLOTS
			titleScreenLoadMenu.SetActive(false);
			titleScreenLoadMenu.SetActive(true);

			loadMenuReturnButton.Select();
		}

		public void CloseDeleteCharacterPopUp()
		{
			deleteCharacterSlotPopUp.SetActive(false);
			loadMenuReturnButton.Select();
		}
	}
}