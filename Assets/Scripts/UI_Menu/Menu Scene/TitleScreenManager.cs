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
		[SerializeField] Button startGameButton;
		[SerializeField] Button quitButton;
		[SerializeField] Button settingsButton;

		[Header("Pop Ups")]
		[SerializeField] GameObject noCharacterSlotsPopUp;
		[SerializeField] Button noChacterSlotsOkayButton;
		[SerializeField] GameObject deleteCharacterSlotPopUp;

		[Header("Settings Menu")]
		[SerializeField] GameObject titleScreenSettingsMenu;
		[SerializeField] Slider bgmVolumeSlider;
		[SerializeField] Slider sfxVolumeSlider;
		[SerializeField] Button settingsReturnButton;

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

		private void Start()
		{
			// Tự động liên kết sự kiện slider để tránh lỗi cấu hình bằng tay trong Unity Inspector
			if (bgmVolumeSlider != null)
			{
				bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
			}
			if (sfxVolumeSlider != null)
			{
				sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
			}
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		private void OnEnable()
		{
			WorldSaveGameManager.OnNoFreeCharacterSlotsAvailable += DisplayNoFreeCharacterSlotPopUp;
		}

		private void OnDisable()
		{
			WorldSaveGameManager.OnNoFreeCharacterSlotsAvailable -= DisplayNoFreeCharacterSlotPopUp;
		}

		public void StartNewGame()
		{
			if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.isSceneLoading)
				return;

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

		public void OpenSettingsMenu()
		{
			titleScreenMainMenu.SetActive(false);
			titleScreenSettingsMenu.SetActive(true);

			// Load the current volumes into the sliders
			if (WorldSoundFXManager.instance != null)
			{
				bgmVolumeSlider.value = WorldSoundFXManager.instance.GetBGMVolume();
				sfxVolumeSlider.value = WorldSoundFXManager.instance.GetSFXVolume();
			}

			// Select the first setting control
			bgmVolumeSlider.Select();
		}

		public void CloseSettingsMenu()
		{
			titleScreenSettingsMenu.SetActive(false);

			settingsButton.Select();
		}

		public void SetBGMVolume(float volume)
		{
			if (WorldSoundFXManager.instance != null)
			{
				WorldSoundFXManager.instance.SetBGMVolume(volume);
			}
		}

		public void SetSFXVolume(float volume)
		{
			if (WorldSoundFXManager.instance != null)
			{
				WorldSoundFXManager.instance.SetSFXVolume(volume);
			}
		}

		public void CloseMainGameMenu()
		{
			startGameButton.gameObject.SetActive(true);
			settingsButton.gameObject.SetActive(true);
			quitButton.gameObject.SetActive(true);

			titleScreenMainMenu.SetActive(false);
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

		public void QuitGame()
		{
			Application.Quit();

			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#endif
		}

		public void DisableQuitButton()
		{
			quitButton.gameObject.SetActive(false);
		}

		public void DisableSettingsButton()
		{
			settingsButton.gameObject.SetActive(false);
		}
	}
}