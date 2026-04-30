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
	}
}