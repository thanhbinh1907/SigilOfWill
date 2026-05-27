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
			if (item is WeaponItem weaponItem)
			{
				if (!weaponsInventory.Contains(weaponItem))
				{
					weaponsInventory.Add(weaponItem);
				}
			}
			else
			{
				if (!itemsInventory.Contains(item))
				{
					itemsInventory.Add(item);
				}
			}
		}
	}
}