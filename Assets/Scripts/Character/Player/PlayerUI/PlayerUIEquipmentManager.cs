using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SG
{
    public class PlayerUIEquipmentManager : MonoBehaviour
    {
        [Header("Menu Window")]
        [SerializeField] GameObject menu;

        [Header("Weapon Slots")]
        [SerializeField] Image rightHandSlot01;
        [SerializeField] Image rightHandSlot02;
        [SerializeField] Image rightHandSlot03;
        [SerializeField] Image leftHandSlot01;
        [SerializeField] Image leftHandSlot02;
        [SerializeField] Image leftHandSlot03;

        [Header("Equipment Inventory")]
		public EquipmentSlotType currentSelectedEquipmentSlot;
		[SerializeField] GameObject equipmentInventoryWindow; 
		[SerializeField] GameObject equipmentInventorySlotPrefab;
		[SerializeField] Transform equipmentInventoryContentWindow;

		private bool isSlotSelected = false;

		public void OpenEquipmentManagerMenu() 
        {
            PlayerUIManager.instance.menuWindowIsOpen = true;
            menu.SetActive(true);
            equipmentInventoryWindow.SetActive(true);
            isSlotSelected = false;
            ClearEquipmentInventory();
			RefreshWeaponSlotIcons();
            ResetAllSlotHighlights();
            LoadWeaponInventory();
		}

        public void CloseEquipmentManagerMenu() 
        {
            PlayerUIManager.instance.menuWindowIsOpen = false;
            menu.SetActive(false);
            ResetAllSlotHighlights();
        }

        public void SetSelectedItem(Item item, UIEquipmentInventorySlot selectedSlot)
        {
            Debug.Log($">> [EQUIPMENT UI] Selected weapon from inventory: {(item != null ? item.itemName : "null")}");
            if (item != null && isSlotSelected)
            {
                EquipWeapon(item);
            }
            else if (!isSlotSelected)
            {
                Debug.LogWarning(">> [EQUIPMENT UI] No slot is selected! Please select a slot first.");
            }
        }

        private void RefreshWeaponSlotIcons()
        {
            PlayerManager player = PlayerManager.instance;

            // RIGHT HAND SLOT 01
            WeaponItem rightHandWeapon01 = player.playerInventoryManager.weaponsInRightHandSlots[0];
            if (rightHandWeapon01 != null && rightHandWeapon01.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                rightHandSlot01.enabled = true;
                rightHandSlot01.sprite = rightHandWeapon01.itemIcon;
            }
            else
            {
                rightHandSlot01.enabled = false;
                rightHandSlot01.sprite = null;
            }

            // RIGHT HAND SLOT 02
            WeaponItem rightHandWeapon02 = player.playerInventoryManager.weaponsInRightHandSlots[1];
            if (rightHandWeapon02 != null && rightHandWeapon02.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                rightHandSlot02.enabled = true;
                rightHandSlot02.sprite = rightHandWeapon02.itemIcon;
            }
            else
            {
                rightHandSlot02.enabled = false;
                rightHandSlot02.sprite = null;
            }

            // RIGHT HAND SLOT 03
            WeaponItem rightHandWeapon03 = player.playerInventoryManager.weaponsInRightHandSlots[2];
            if (rightHandWeapon03 != null && rightHandWeapon03.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                rightHandSlot03.enabled = true;
                rightHandSlot03.sprite = rightHandWeapon03.itemIcon;
            }
            else
            {
                rightHandSlot03.enabled = false;
                rightHandSlot03.sprite = null;
            }

			// LEFT HAND SLOT 01
            WeaponItem leftHandWeapon01 = player.playerInventoryManager.weaponsInLeftHandSlots[0];
            if (leftHandWeapon01 != null && leftHandWeapon01.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                leftHandSlot01.enabled = true;
                leftHandSlot01.sprite = leftHandWeapon01.itemIcon;
            }
            else
            {
                leftHandSlot01.enabled = false;
                leftHandSlot01.sprite = null;
			}

            // LEFT HAND SLOT 02
            WeaponItem leftHandWeapon02 = player.playerInventoryManager.weaponsInLeftHandSlots[1];
            if (leftHandWeapon02 != null && leftHandWeapon02.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                leftHandSlot02.enabled = true;
                leftHandSlot02.sprite = leftHandWeapon02.itemIcon;
            }
            else
            {
                leftHandSlot02.enabled = false;
                leftHandSlot02.sprite = null;
			}

            // LEFT HAND SLOT 03
            WeaponItem leftHandWeapon03 = player.playerInventoryManager.weaponsInLeftHandSlots[2];
            if (leftHandWeapon03 != null && leftHandWeapon03.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                leftHandSlot03.enabled = true;
                leftHandSlot03.sprite = leftHandWeapon03.itemIcon;
            }
            else
            {
                leftHandSlot03.enabled = false;
                leftHandSlot03.sprite = null;
			}
		}

        private void ClearEquipmentInventory()
        {
            foreach (Transform item in equipmentInventoryContentWindow)
            {
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
			}
		}

		public void LoadEquipmenInventory(EquipmentSlotType clickedSlot)
        {
            Debug.Log($">> [EQUIPMENT UI] LoadEquipmenInventory called. Clicked Slot: {clickedSlot}");

            // If clicking the slot that is already highlighted/selected, un-equip and toggle off highlight
            if (isSlotSelected && currentSelectedEquipmentSlot == clickedSlot)
            {
                UnEquipWeaponAtSlot(clickedSlot);
                isSlotSelected = false;
                ResetAllSlotHighlights();
                RefreshWeaponSlotIcons();
                ClearEquipmentInventory();
                LoadWeaponInventory();
                return;
            }

            currentSelectedEquipmentSlot = clickedSlot;
            isSlotSelected = true;
            HighlightSelectedSlot();

            ClearEquipmentInventory();
            equipmentInventoryWindow.SetActive(true);

            switch (currentSelectedEquipmentSlot)
            {
                case EquipmentSlotType.RightWeapon01:
                case EquipmentSlotType.RightWeapon02:
                case EquipmentSlotType.RightWeapon03:
                case EquipmentSlotType.LeftWeapon01:
                case EquipmentSlotType.LeftWeapon02:
                case EquipmentSlotType.LeftWeapon03:
					LoadWeaponInventory();
					break;
                default:
                    break;
            }
        }

        private void UnEquipWeaponAtSlot(EquipmentSlotType slot)
        {
            PlayerManager player = PlayerManager.instance;
            WeaponItem[] weaponsSlots = null;
            int slotIndex = 0;
            bool isRightHand = true;

            switch (slot)
            {
                case EquipmentSlotType.RightWeapon01:
                    weaponsSlots = player.playerInventoryManager.weaponsInRightHandSlots;
                    slotIndex = 0;
                    isRightHand = true;
                    break;
                case EquipmentSlotType.RightWeapon02:
                    weaponsSlots = player.playerInventoryManager.weaponsInRightHandSlots;
                    slotIndex = 1;
                    isRightHand = true;
                    break;
                case EquipmentSlotType.RightWeapon03:
                    weaponsSlots = player.playerInventoryManager.weaponsInRightHandSlots;
                    slotIndex = 2;
                    isRightHand = true;
                    break;
                case EquipmentSlotType.LeftWeapon01:
                    weaponsSlots = player.playerInventoryManager.weaponsInLeftHandSlots;
                    slotIndex = 0;
                    isRightHand = false;
                    break;
                case EquipmentSlotType.LeftWeapon02:
                    weaponsSlots = player.playerInventoryManager.weaponsInLeftHandSlots;
                    slotIndex = 1;
                    isRightHand = false;
                    break;
                case EquipmentSlotType.LeftWeapon03:
                    weaponsSlots = player.playerInventoryManager.weaponsInLeftHandSlots;
                    slotIndex = 2;
                    isRightHand = false;
                    break;
            }

            if (weaponsSlots != null)
            {
                WeaponItem equippedWeapon = weaponsSlots[slotIndex];
                
                // If there is indeed a weapon equipped (not unarmed)
                if (equippedWeapon != null && equippedWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    Debug.Log($">> [EQUIPMENT UI] Un-equipping weapon: {equippedWeapon.itemName} from slot {slot}");
                    
                    // Return it to inventory
                    player.playerInventoryManager.AddItemToInventory(equippedWeapon);
                    
                    // Set slot to unarmed
                    weaponsSlots[slotIndex] = WorldItemDatabase.instance.unarmedWeapon;

                    // Update active weapons on hand if we are un-equipping the currently active hand weapon
                    if (isRightHand)
                    {
                        if (player.playerInventoryManager.rightHandWeaponIndex == slotIndex)
                        {
                            player.playerInventoryManager.currentRightHandWeapon = WorldItemDatabase.instance.unarmedWeapon;
                            player.playerEquipmentManager.LoadRightWeapon();
                        }
                    }
                    else
                    {
                        if (player.playerInventoryManager.leftHandWeaponIndex == slotIndex)
                        {
                            player.playerInventoryManager.currentLeftHandWeapon = WorldItemDatabase.instance.unarmedWeapon;
                            player.playerEquipmentManager.LoadLeftWeapon();
                        }
                    }
                }
            }
        }

        public void EquipWeapon(Item item)
        {
            PlayerManager player = PlayerManager.instance;
            WeaponItem newWeapon = item as WeaponItem;

            Debug.Log($">> [EQUIPMENT UI] EquipWeapon starting. Weapon: {(newWeapon != null ? newWeapon.itemName : "Null")}, Target Slot: {currentSelectedEquipmentSlot}");

            if (newWeapon == null)
            {
                Debug.LogWarning(">> [EQUIPMENT UI] Selected item is not a WeaponItem!");
                return;
            }

            WeaponItem[] weaponsSlots = null;
            int slotIndex = 0;
            bool isRightHand = true;

            switch (currentSelectedEquipmentSlot)
            {
                case EquipmentSlotType.RightWeapon01:
                    weaponsSlots = player.playerInventoryManager.weaponsInRightHandSlots;
                    slotIndex = 0;
                    isRightHand = true;
                    break;
                case EquipmentSlotType.RightWeapon02:
                    weaponsSlots = player.playerInventoryManager.weaponsInRightHandSlots;
                    slotIndex = 1;
                    isRightHand = true;
                    break;
                case EquipmentSlotType.RightWeapon03:
                    weaponsSlots = player.playerInventoryManager.weaponsInRightHandSlots;
                    slotIndex = 2;
                    isRightHand = true;
                    break;
                case EquipmentSlotType.LeftWeapon01:
                    weaponsSlots = player.playerInventoryManager.weaponsInLeftHandSlots;
                    slotIndex = 0;
                    isRightHand = false;
                    break;
                case EquipmentSlotType.LeftWeapon02:
                    weaponsSlots = player.playerInventoryManager.weaponsInLeftHandSlots;
                    slotIndex = 1;
                    isRightHand = false;
                    break;
                case EquipmentSlotType.LeftWeapon03:
                    weaponsSlots = player.playerInventoryManager.weaponsInLeftHandSlots;
                    slotIndex = 2;
                    isRightHand = false;
                    break;
            }

            if (weaponsSlots != null)
            {
                WeaponItem oldWeapon = weaponsSlots[slotIndex];
                Debug.Log($">> [EQUIPMENT UI] Swapping weapon at index {slotIndex}. Old: {(oldWeapon != null ? oldWeapon.itemName : "None")}, New: {newWeapon.itemName}");

                weaponsSlots[slotIndex] = newWeapon;
                player.playerInventoryManager.RemoveItemFromInventory(newWeapon);

                if (oldWeapon != null && oldWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(oldWeapon);
                    Debug.Log($">> [EQUIPMENT UI] Returned old weapon: {oldWeapon.itemName} to inventory.");
                }

                if (isRightHand)
                {
                    if (player.playerInventoryManager.rightHandWeaponIndex == slotIndex)
                    {
                        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
                        player.playerEquipmentManager.LoadRightWeapon();
                        Debug.Log($">> [EQUIPMENT UI] Updated active Right Hand weapon to: {newWeapon.itemName}");
                    }
                }
                else
                {
                    if (player.playerInventoryManager.leftHandWeaponIndex == slotIndex)
                    {
                        player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
                        player.playerEquipmentManager.LoadLeftWeapon();
                        Debug.Log($">> [EQUIPMENT UI] Updated active Left Hand weapon to: {newWeapon.itemName}");
                    }
                }
            }
            else
            {
                Debug.LogWarning(">> [EQUIPMENT UI] weaponsSlots is null! (Slot not mapped correctly)");
            }

            // Turn off highlight
            isSlotSelected = false;
            ResetAllSlotHighlights();

            // Refresh UI immediately
            RefreshWeaponSlotIcons();
            ClearEquipmentInventory();
            LoadWeaponInventory();
        }

        private void HighlightSelectedSlot()
        {
            ResetAllSlotHighlights();
            Image activeSlotImage = GetImageForSlot(currentSelectedEquipmentSlot);
            if (activeSlotImage != null)
            {
                Image slotBg = activeSlotImage.GetComponentInParent<Image>();
                if (slotBg != null && slotBg != activeSlotImage)
                {
                    slotBg.color = new Color(1f, 0.8f, 0f, 1f); // Gold / Orange highlight
                }
            }
        }

        private void ResetAllSlotHighlights()
        {
            Image[] slotImages = { rightHandSlot01, rightHandSlot02, rightHandSlot03, leftHandSlot01, leftHandSlot02, leftHandSlot03 };
            foreach (var img in slotImages)
            {
                if (img != null)
                {
                    Image slotBg = img.GetComponentInParent<Image>();
                    if (slotBg != null && slotBg != img)
                    {
                        slotBg.color = Color.white; // Reset to standard white/default
                    }
                }
            }
        }

        private Image GetImageForSlot(EquipmentSlotType slotType)
        {
            switch (slotType)
            {
                case EquipmentSlotType.RightWeapon01: return rightHandSlot01;
                case EquipmentSlotType.RightWeapon02: return rightHandSlot02;
                case EquipmentSlotType.RightWeapon03: return rightHandSlot03;
                case EquipmentSlotType.LeftWeapon01: return leftHandSlot01;
                case EquipmentSlotType.LeftWeapon02: return leftHandSlot02;
                case EquipmentSlotType.LeftWeapon03: return leftHandSlot03;
                default: return null;
            }
        }

        private void LoadWeaponInventory()
        {
            List<WeaponItem> weaponsInInventory = new List<WeaponItem>();

            PlayerManager player = PlayerManager.instance;

            for (int i = 0; i < player.playerInventoryManager.itemsInventory.Count; i++)
            {
                WeaponItem weapon = player.playerInventoryManager.itemsInventory[i] as WeaponItem;

                if (weapon != null)
                {
                    weaponsInInventory.Add(weapon);
                }
            }

            if (weaponsInInventory.Count <= 0)
            {
                return;
			}

			for (int i = 0; i < weaponsInInventory.Count; i++)
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
                UIEquipmentInventorySlot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UIEquipmentInventorySlot>();
                equipmentInventorySlot.AddItem(weaponsInInventory[i]);
            }
        }
	}
}
