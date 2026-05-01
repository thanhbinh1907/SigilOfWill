using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace SG
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        PlayerManager player;

        public WeaponModelInstatiationSlot rightHandSlot;
        public WeaponModelInstatiationSlot leftHandSlot;

        [SerializeField] public WeaponManager rightWeaponManager;
        [SerializeField] public WeaponManager leftWeaponManager;

		public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;

		protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();

            InitializeWeaponSlots();
		}

        protected override void Start()
        {
            base.Start();
            LoadWeaponsOnBothHands();
        }

		private void InitializeWeaponSlots()
        {
            WeaponModelInstatiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstatiationSlot>();

            foreach (var weaponSlot in weaponSlots)
            {
                if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    rightHandSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
                {
                    leftHandSlot = weaponSlot;
                }
			}
		}

        public void LoadWeaponsOnBothHands()
        {
            LoadRightWeapon();
            LoadLeftWeapon();
		}

		// RIGHT HAND WEAPON

        public void SwitchRightWeapon()
        {
			player.playerAnimatorManager.PlayTargetAnimation("Swap_Right_Weapon_01", false, true, true, true);

			WeaponItem selectedWeapon = null;
			int index = player.playerInventoryManager.rightHandWeaponIndex;

			for (int i = 0; i < 3; i++) 
			{
				index += 1;
				if (index > 2) index = 0; 

				WeaponItem weaponInSlot = player.playerInventoryManager.weaponsInRightHandSlots[index];

				if (weaponInSlot != null && weaponInSlot.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
				{
					selectedWeapon = weaponInSlot;
					player.playerInventoryManager.rightHandWeaponIndex = index;
					break; 
				}
			}

			if (selectedWeapon != null)
			{
				player.playerInventoryManager.currentRightHandWeapon = selectedWeapon;
			}
			else
			{
				player.playerInventoryManager.currentRightHandWeapon = WorldItemDatabase.instance.unarmedWeapon;
				player.playerInventoryManager.rightHandWeaponIndex = -1;
			}

			LoadRightWeapon();
		}

		public void LoadRightWeapon()
        {
            if (player.playerInventoryManager.currentRightHandWeapon != null)
            {
				// UNLOAD CURRENT WEAPON MODEL
                rightHandSlot.UnloadWeapon();

				// LOAD NEW WEAPON MODEL
				rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
				rightHandSlot.LoadWeapon(rightHandWeaponModel);
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
			}
		}

		// LEFT HAND WEAPON

        public void SwitchLeftWeapon()
        {

		}

		public void LoadLeftWeapon()
        {
            if (player.playerInventoryManager.currentLeftHandWeapon != null)
            {
                leftHandSlot.UnloadWeapon();

                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
			}
        }
	}
}