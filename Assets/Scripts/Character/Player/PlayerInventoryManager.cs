using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class PlayerInventoryManager : CharacterInventoryManager
	{
		public WeaponItem currentRightHandWeapon;
		public WeaponItem currentLeftHandWeapon;

		[Header("Quick Slots")]
		public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
		public int rightHandWeaponIndex = 1;
		public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
		public int leftHandWeaponIndex = 1;

		[Header("Inventory")]
		public List<Item> itemsInventory = new List<Item>();
		public List<WeaponItem> weaponsInventory = new List<WeaponItem>();

		public void AddItemToInventory(Item item)
		{
			itemsInventory.Add(item);
		}

		public void RemoveItemFromInventory(Item item)
		{
			itemsInventory.Remove(item);

			for (int i = itemsInventory.Count - 1; i > -1; i--)
			{
				if (itemsInventory[i] == null)
				{
					itemsInventory.RemoveAt(i);
				}
			}
		}
	}
}