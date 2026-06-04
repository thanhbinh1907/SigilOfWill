using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class UIEquipmentInventorySlot : MonoBehaviour
    {
        public Image itemIcon;
        public Image highlightedIcon;
		[SerializeField] public Item currentItem;

		public void AddItem(Item item)
        {
            if (item == null)
            {
                return;
            }

            itemIcon.enabled = true;

            currentItem = item;
            itemIcon.sprite = item.itemIcon;
        }

        public void SelectSlot()
        {
            Debug.Log($">> [SLOT] SelectSlot called for: {(currentItem != null ? currentItem.itemName : "null")}");
            if (highlightedIcon != null)
            {
                highlightedIcon.enabled = true;
            }
            PlayerUIManager.instance.playerUIEquipmentManager.SetSelectedItem(currentItem, this);
        }

        public void DeselectSlot()
        {
            if (highlightedIcon != null)
            {
                highlightedIcon.enabled = false;
            }
        }

        public void EquipItem()
        {
            Debug.Log($">> [SLOT] EquipItem called for: {(currentItem != null ? currentItem.itemName : "null")}");
            PlayerUIManager.instance.playerUIEquipmentManager.SetSelectedItem(currentItem, this);
        }
    }
}
