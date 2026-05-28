using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG { 
	public class PlayerUIManager : MonoBehaviour
	{
		public static PlayerUIManager instance;
		[HideInInspector] public PlayerUIHudManager playerUIHudManager;
		public PlayerUIHudManager playerHUDManager => playerUIHudManager;
		[HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
		[HideInInspector] public PlayerUICharacterMenuManager playerUICharacterMenuManager;
		[HideInInspector] public PlayerUIEquipmentManager playerUIEquipmentManager;

		[Header("UI Windows")]
		public bool menuWindowIsOpen = false;
		public bool popupWindowIsOpen = false;

		[Header("Main Menu Settings (Offline)")]
		[SerializeField] private CanvasGroup hudCanvasGroup;       // Kéo thả CanvasGroup của thanh Máu/Stamina vào đây
		[SerializeField] private CanvasGroup mainMenuCanvasGroup;  // Kéo thả CanvasGroup của Menu Tổng (Equipment, Inventory...) vào đây

		public void ToggleMainMenu()
		{
			menuWindowIsOpen = !menuWindowIsOpen;

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
				
				// Khóa thời gian game hoặc ngắt hành vi tấn công/nhảy của Player tại đây nếu cần
			}
			else
			{
				// HIỆN LẠI THANH HUD CHIẾN ĐẤU
				if (hudCanvasGroup != null) { hudCanvasGroup.alpha = 1; hudCanvasGroup.interactable = true; hudCanvasGroup.blocksRaycasts = true; }

				// ẨN MENU TỔNG
				if (mainMenuCanvasGroup != null) { mainMenuCanvasGroup.alpha = 0; mainMenuCanvasGroup.interactable = false; mainMenuCanvasGroup.blocksRaycasts = false; }
				
				// Đóng luôn màn hình con Trang bị nếu người chơi đang bật
				GetComponentInChildren<PlayerUIEquipmentManager>()?.CloseEquipmentManagerMenu();
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
			if (playerUICharacterMenuManager != null)
				playerUICharacterMenuManager.CloseCharacterMenu();
			if (playerUIEquipmentManager != null)
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