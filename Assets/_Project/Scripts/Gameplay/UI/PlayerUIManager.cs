using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		public void ToggleMainMenu()
		{
			SetMainMenuActive(!menuWindowIsOpen);
		}

		public void SetMainMenuActive(bool active)
		{
			menuWindowIsOpen = active;

			if (menuWindowIsOpen)
			{
				// ẨN THANH HUD CHIẾN ĐẤU
				if (hudCanvasGroup != null)
				{
					hudCanvasGroup.alpha = 0;
					hudCanvasGroup.interactable = false;
					hudCanvasGroup.blocksRaycasts = false;
				}

				// HIỆN MENU TỔNG
				if (mainMenuCanvasGroup != null)
				{
					mainMenuCanvasGroup.alpha = 1;
					mainMenuCanvasGroup.interactable = true;
					mainMenuCanvasGroup.blocksRaycasts = true;
				}
				
				// Tắt player input manager đi để không thể di chuyển
				if (PlayerInputManager.instance != null)
				{
					PlayerInputManager.instance.enabled = false;
				}

				// Mở khóa chuột để chọn ô vũ khí
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{
				// HIỆN LẠI THANH HUD CHIẾN ĐẤU
				if (hudCanvasGroup != null) { hudCanvasGroup.alpha = 1; hudCanvasGroup.interactable = true; hudCanvasGroup.blocksRaycasts = true; }

				// ẨN MENU TỔNG
				if (mainMenuCanvasGroup != null) { mainMenuCanvasGroup.alpha = 0; mainMenuCanvasGroup.interactable = false; mainMenuCanvasGroup.blocksRaycasts = false; }
				
				// Đóng luôn màn hình con Trang bị nếu người chơi đang bật
				GetComponentInChildren<PlayerUIEquipmentManager>()?.CloseEquipmentManagerMenu();

				// Bật lại player input manager
				if (PlayerInputManager.instance != null)
				{
					PlayerInputManager.instance.enabled = true;
				}

				// Khóa lại chuột khi chơi game
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		private void Update()
		{
			if (menuWindowIsOpen)
			{
				// Cho phép nhấn Escape để đóng menu khi PlayerInputManager bị tắt
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
			playerUICharacterMenuManager.CloseCharacterMenu();
			playerUIEquipmentManager.CloseEquipmentManagerMenu();
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}
	} 
	
}