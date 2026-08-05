using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SG {
	public class PlayerUIManager : MonoBehaviour
	{
		public static PlayerUIManager instance;

		[HideInInspector] public PlayerUIHudManager playerUIHudManager;
		[HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
		[HideInInspector] public PlayerUICharacterMenuManager playerUICharacterMenuManager;
		[HideInInspector] public PlayerUIEquipmentManager playerUIEquipmentManager;

		[Header("UI Windows")]
		public bool menuWindowIsOpen = false;
		public bool popupWindowIsOpen = false;

		[Header("Main Menu Settings (Offline)")]
		[SerializeField] private CanvasGroup hudCanvasGroup;
		[SerializeField] private CanvasGroup mainMenuCanvasGroup;

		[Header("Settings Menu")]
		[SerializeField] public GameObject settingsMenu;
		[SerializeField] public Slider bgmVolumeSlider;
		[SerializeField] public Slider sfxVolumeSlider;
		[SerializeField] public Button settingsReturnButton;

		public void ToggleMainMenu()
		{
			SetMainMenuActive(!menuWindowIsOpen);
		}

		public void SetMainMenuActive(bool active)
		{
			menuWindowIsOpen = active;

			if (menuWindowIsOpen)
			{

				if (hudCanvasGroup != null)
				{
					hudCanvasGroup.alpha = 0;
					hudCanvasGroup.interactable = false;
					hudCanvasGroup.blocksRaycasts = false;
				}


				if (mainMenuCanvasGroup != null)
				{
					mainMenuCanvasGroup.alpha = 1;
					mainMenuCanvasGroup.interactable = true;
					mainMenuCanvasGroup.blocksRaycasts = true;
				}


				if (PlayerInputManager.instance != null)
				{
					PlayerInputManager.instance.enabled = false;
				}


				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{

				if (hudCanvasGroup != null) { hudCanvasGroup.alpha = 1; hudCanvasGroup.interactable = true; hudCanvasGroup.blocksRaycasts = true; }


				if (mainMenuCanvasGroup != null) { mainMenuCanvasGroup.alpha = 0; mainMenuCanvasGroup.interactable = false; mainMenuCanvasGroup.blocksRaycasts = false; }


				GetComponentInChildren<PlayerUIEquipmentManager>()?.CloseEquipmentManagerMenu();


				if (PlayerInputManager.instance != null)
				{
					PlayerInputManager.instance.enabled = true;
				}


				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		private void Update()
		{
			if (menuWindowIsOpen)
			{

				bool escapePressed = UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
				if (escapePressed)
				{
					CloseAllMenuWindows();
				}
			}
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
			DontDestroyOnLoad(gameObject);

			playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
			playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
			playerUICharacterMenuManager = GetComponentInChildren<PlayerUICharacterMenuManager>(true);
			playerUIEquipmentManager = GetComponentInChildren<PlayerUIEquipmentManager>(true);
		}

		public void CloseAllMenuWindows()
		{
			if (settingsMenu != null && settingsMenu.activeSelf)
			{
				CloseSettingsMenu();
				return;
			}

			playerUICharacterMenuManager.CloseCharacterMenu();
			playerUIEquipmentManager.CloseEquipmentManagerMenu();
		}

		private void Start()
		{
			if (instance != this)
			{
				return;
			}
			DontDestroyOnLoad(gameObject);

			// Dynamically bind settings button inside pause menu
			Button settingsBtn = FindButtonByName(transform, "Settings");
			if (settingsBtn != null)
			{
				settingsBtn.onClick.RemoveAllListeners();
				settingsBtn.onClick.AddListener(OpenSettingsMenu);
			}

			// Bind sliders
			if (bgmVolumeSlider != null)
			{
				bgmVolumeSlider.onValueChanged.RemoveAllListeners();
				bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
			}
			if (sfxVolumeSlider != null)
			{
				sfxVolumeSlider.onValueChanged.RemoveAllListeners();
				sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
			}
			if (settingsReturnButton != null)
			{
				settingsReturnButton.onClick.RemoveAllListeners();
				settingsReturnButton.onClick.AddListener(CloseSettingsMenu);
			}

			Scene currentScene = SceneManager.GetActiveScene();
			if (currentScene.buildIndex == 0 || currentScene.name.Contains("Menu") || currentScene.name.Contains("Title"))
			{
				SetHUDActive(false);
			}
			else
			{
				SetHUDActive(true);
			}
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
			if (scene.buildIndex == 0 || scene.name.Contains("Menu") || scene.name.Contains("Title"))
			{
				SetHUDActive(false);
			}
			else
			{
				SetHUDActive(true);
			}

			if (playerUIHudManager == null)
			{
				playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>(true);
			}

			if (playerUIHudManager != null)
			{
				playerUIHudManager.ClearAllBossHPBars();
			}

			if (playerUIPopUpManager == null)
			{
				playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>(true);
			}

			if (playerUIPopUpManager != null)
			{
				playerUIPopUpManager.CloseDemoCompletionPopup();
			}
		}

		public void SetHUDActive(bool active)
		{
			if (hudCanvasGroup != null)
			{
				hudCanvasGroup.alpha = active ? 1 : 0;
				hudCanvasGroup.interactable = active;
				hudCanvasGroup.blocksRaycasts = active;
			}

			if (playerUIHudManager == null)
			{
				playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>(true);
			}

			if (playerUIHudManager != null)
			{
				playerUIHudManager.ToggleHUD(active);
			}
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		private Button FindButtonByName(Transform parent, string name)
		{
			foreach (var btn in parent.GetComponentsInChildren<Button>(true))
			{
				if (btn.gameObject.name == name)
					return btn;
			}
			return null;
		}

		public void OpenSettingsMenu()
		{
			if (mainMenuCanvasGroup != null)
			{
				mainMenuCanvasGroup.alpha = 0f;
				mainMenuCanvasGroup.interactable = false;
				mainMenuCanvasGroup.blocksRaycasts = false;
			}

			if (settingsMenu != null)
			{
				settingsMenu.SetActive(true);
			}

			if (WorldSoundFXManager.instance != null)
			{
				if (bgmVolumeSlider != null) bgmVolumeSlider.value = WorldSoundFXManager.instance.GetBGMVolume();
				if (sfxVolumeSlider != null) sfxVolumeSlider.value = WorldSoundFXManager.instance.GetSFXVolume();
			}

			if (bgmVolumeSlider != null)
			{
				bgmVolumeSlider.Select();
				if (UnityEngine.EventSystems.EventSystem.current != null)
				{
					UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(bgmVolumeSlider.gameObject);
				}
			}
		}

		public void CloseSettingsMenu()
		{
			if (settingsMenu != null)
			{
				settingsMenu.SetActive(false);
			}

			if (mainMenuCanvasGroup != null)
			{
				mainMenuCanvasGroup.alpha = 1f;
				mainMenuCanvasGroup.interactable = true;
				mainMenuCanvasGroup.blocksRaycasts = true;
			}

			Button settingsBtn = FindButtonByName(transform, "Settings");
			if (settingsBtn != null)
			{
				settingsBtn.Select();
				if (UnityEngine.EventSystems.EventSystem.current != null)
				{
					UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(settingsBtn.gameObject);
				}
			}
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
	}

}