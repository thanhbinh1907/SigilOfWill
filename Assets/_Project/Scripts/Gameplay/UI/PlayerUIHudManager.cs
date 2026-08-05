using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace SG
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [Header("Stat Bars")]
        [SerializeField] UI_StatBar healthBar;
        [SerializeField] UI_StatBar staminaBar;
        [SerializeField] UI_StatBar manaBar;

        [Header("HUD Canvas Groups")]
        [SerializeField] CanvasGroup[] hudCanvasGroups;

        public void ToggleHUD(bool status)
        {
            foreach (var canvasGroup in hudCanvasGroups)
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = status ? 1f : 0f;
                }
            }
        }

        [Header("Quick Slot Icon")]
        [SerializeField] Image rightWeaponQuickSlotIcon;
        [SerializeField] Image leftWeaponQuickSlotIcon;

		[Header("Boss Fight UI")]
		[SerializeField] GameObject bossHPBarObject;
		[SerializeField] Transform bossHPBarParent;
		[SerializeField] List<UI_BossHPBar> activeBossHPBars = new List<UI_BossHPBar>();
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

        public void SetNewStaminaValue(float oldValue, float newValue)
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

        public void SetRightWeaponQuickSlotIcon(int weaponID)
        {
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

            if (weapon == null)
            {
                Debug.Log("ITEM IS NULL");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                Debug.Log("ITEM HAS NO ICON");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            rightWeaponQuickSlotIcon.enabled = true;
        }

        public void SetLeftWeaponQuickSlotIcon(int weaponID)
        {
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

            if (weapon == null)
            {
                Debug.Log("ITEM IS NULL");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
			}

            if (weapon.itemIcon == null)
            {
                Debug.Log("ITEM HAS NO ICON");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }
            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            leftWeaponQuickSlotIcon.enabled = true;
		}

        public void AddBossHPBar(AIBossCharacterManager boss)
        {

            if (activeBossHPBars.Exists(bar => bar.GetBossCharacter() == boss))
                return;

            if (bossHPBarObject == null)
            {
                Debug.LogError("[HỆ THỐNG] Không thể hiển thị thanh máu Boss vì bossHPBarObject chưa được gán trên PlayerUIHudManager trong Inspector!");
                return;
            }

            GameObject barObj = Instantiate(bossHPBarObject, bossHPBarParent);
            UI_BossHPBar hpBar = barObj.GetComponent<UI_BossHPBar>();
            if (hpBar == null)
            {
                hpBar = barObj.GetComponentInChildren<UI_BossHPBar>();
            }

            if (hpBar != null)
            {
                hpBar.EnableBossHPBar(boss);
                activeBossHPBars.Add(hpBar);
            }
            else
            {
                Debug.LogError($"[HUD Manager] Không tìm thấy Component UI_BossHPBar trên Prefab {bossHPBarObject.name} (kể cả trên các đối tượng con)!");
            }
        }

        public void RemoveBossHPBar(AIBossCharacterManager boss)
        {
            UI_BossHPBar hpBar = activeBossHPBars.Find(bar => bar.GetBossCharacter() == boss);
            if (hpBar != null)
            {
                activeBossHPBars.Remove(hpBar);

                if (hpBar.transform.parent != null && hpBar.transform.parent != bossHPBarParent)
                {
                    Destroy(hpBar.transform.parent.gameObject);
                }
                else
                {
                    Destroy(hpBar.gameObject);
                }
            }
        }

        public void ClearAllBossHPBars()
        {
            foreach (var hpBar in activeBossHPBars)
            {
                if (hpBar != null)
                {
                    if (hpBar.transform.parent != null && hpBar.transform.parent != bossHPBarParent)
                    {
                        Destroy(hpBar.transform.parent.gameObject);
                    }
                    else
                    {
                        Destroy(hpBar.gameObject);
                    }
                }
            }
            activeBossHPBars.Clear();
        }
    }
}