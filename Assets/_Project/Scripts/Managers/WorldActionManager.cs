using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SG
{
    public class WorldActionManager : MonoBehaviour
    {
        public static WorldActionManager instance;

        [Header("Weapon Item Actions")]
        public WeaponItemAction[] weaponItemActions;

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
		}

		public void Start()
		{
			if (weaponItemActions != null)
			{
				for (int i = 0; i < weaponItemActions.Length; i++)
				{
					if (weaponItemActions[i] != null)
					{
						weaponItemActions[i].actionID = i;
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		public WeaponItemAction GetWeaponItemActionByID(int ID)
        {
            if (weaponItemActions == null) return null;
            return weaponItemActions.FirstOrDefault(action => action != null && action.actionID == ID);
		}
	}
}