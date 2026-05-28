using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class PlayerUIEquipmentManager : MonoBehaviour
    {
        [Header("Menu Window")]
        [SerializeField] GameObject menuWindow;

        [Header("Weapon Slots UI Images")]
        [SerializeField] Image rightHandSlot01;
        [SerializeField] Image rightHandSlot02;
        [SerializeField] Image rightHandSlot03;
        [SerializeField] Image leftHandSlot01;
        [SerializeField] Image leftHandSlot02;
        [SerializeField] Image leftHandSlot03;

        [Header("Default Selected Button")]
        [SerializeField] Button defaultSelectedButton;

        public void OpenEquipmentManagerMenu()
        {
            menuWindow.SetActive(true);
            RefreshWeaponSlotIcons();

            if (defaultSelectedButton != null)
            {
                defaultSelectedButton.Select();
            }
        }

        public void CloseEquipmentManagerMenu()
        {
            menuWindow.SetActive(false);
        }

        private void RefreshWeaponSlotIcons()
        {
            // CHUYỂN HƯỚNG OFFLINE: Lấy thẳng qua Instance Singleton của PlayerManager cục bộ
            PlayerManager player = PlayerManager.instance; 

            if (player == null) return;

            // TAY PHẢI - Ô 1
            if (player.playerInventoryManager.weaponsInRightHandSlots[0] != null && 
                player.playerInventoryManager.weaponsInRightHandSlots[0].itemIcon != null)
            {
                rightHandSlot01.sprite = player.playerInventoryManager.weaponsInRightHandSlots[0].itemIcon;
                rightHandSlot01.enabled = true;
            }
            else
            {
                rightHandSlot01.enabled = false; // Ẩn icon nếu là tay không (Unarmed)
            }

            // TAY PHẢI - Ô 2
            if (player.playerInventoryManager.weaponsInRightHandSlots[1] != null && 
                player.playerInventoryManager.weaponsInRightHandSlots[1].itemIcon != null)
            {
                rightHandSlot02.sprite = player.playerInventoryManager.weaponsInRightHandSlots[1].itemIcon;
                rightHandSlot02.enabled = true;
            }
            else
            {
                rightHandSlot02.enabled = false;
            }

            // TAY PHẢI - Ô 3
            if (player.playerInventoryManager.weaponsInRightHandSlots[2] != null && 
                player.playerInventoryManager.weaponsInRightHandSlots[2].itemIcon != null)
            {
                rightHandSlot03.sprite = player.playerInventoryManager.weaponsInRightHandSlots[2].itemIcon;
                rightHandSlot03.enabled = true;
            }
            else
            {
                rightHandSlot03.enabled = false;
            }

            // TAY TRÁI - Ô 1
            if (player.playerInventoryManager.weaponsInLeftHandSlots[0] != null && 
                player.playerInventoryManager.weaponsInLeftHandSlots[0].itemIcon != null)
            {
                leftHandSlot01.sprite = player.playerInventoryManager.weaponsInLeftHandSlots[0].itemIcon;
                leftHandSlot01.enabled = true;
            }
            else
            {
                leftHandSlot01.enabled = false;
            }

            // TAY TRÁI - Ô 2
            if (player.playerInventoryManager.weaponsInLeftHandSlots[1] != null && 
                player.playerInventoryManager.weaponsInLeftHandSlots[1].itemIcon != null)
            {
                leftHandSlot02.sprite = player.playerInventoryManager.weaponsInLeftHandSlots[1].itemIcon;
                leftHandSlot02.enabled = true;
            }
            else
            {
                leftHandSlot02.enabled = false;
            }

            // TAY TRÁI - Ô 3
            if (player.playerInventoryManager.weaponsInLeftHandSlots[2] != null && 
                player.playerInventoryManager.weaponsInLeftHandSlots[2].itemIcon != null)
            {
                leftHandSlot03.sprite = player.playerInventoryManager.weaponsInLeftHandSlots[2].itemIcon;
                leftHandSlot03.enabled = true;
            }
            else
            {
                leftHandSlot03.enabled = false;
            }
        }
    }
}
