using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SG
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase instance;

        public WeaponItem unarmedWeapon;

        [Header("Weapons")]
		[SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

        private List<Item> items = new List<Item>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
			}
            else
            {
                Destroy(gameObject);
            }

			// ADD ALL OF OUR WEAPONS TO THE ITEM DATABASE
			foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }

			// ASSIGN ALL OF OUR ITEMS A UNIQUE ITEM ID
			for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
		}

		public WeaponItem GetWeaponByID(int ID)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }

		public WeaponItem GetWeaponByName(string name)
		{
			return weapons.FirstOrDefault(weapon => weapon.itemName == name);
		}
	}
}