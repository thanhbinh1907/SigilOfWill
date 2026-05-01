using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [SerializeField] UI_StatBar healthBar;
		[SerializeField] UI_StatBar staminaBar;
        [SerializeField] UI_StatBar manaBar;

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
            manaBar.gameObject.SetActive(false);
			manaBar.gameObject.SetActive(true);

		}

        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(newValue);
        }

        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxStat(maxHealth);
		} 

		public void SetNewStaminaValue(float oldValue,float newValue)
        {
            staminaBar.SetStat(newValue);
		}

        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina); 
        }

        public void SetNewManaValue(float oldValue, float newValue)
        {
            manaBar.SetStat(newValue);
		}

        public void SetMaxManaValue(int maxMana)
        {
            manaBar.SetMaxStat(maxMana);
		}
	}
}