using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] MeleeWeaponDamageCollider meleeDamageCollider;

        public Transform spellSpawnPoint;

		private void Awake()
        {
            meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
		}

        public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
        {
            if (meleeDamageCollider != null)
            {
                meleeDamageCollider.characterCausingDamage = characterWieldingWeapon;
                meleeDamageCollider.physicalDamage = weapon.physicalDamage;
                meleeDamageCollider.magicDamage = weapon.magicDamage;
                meleeDamageCollider.fireDamage = weapon.fireDamage;
                meleeDamageCollider.lightningDamage = weapon.lightningDamage;
                meleeDamageCollider.holyDamage = weapon.holyDamage;
                meleeDamageCollider.windDamage = weapon.windDamage;
            }
			else
			{
				Debug.Log($"Vũ khí {weapon.itemName} không có MeleeWeaponDamageCollider. Bỏ qua gán sát thương cận chiến.");
			}
		}
	}
}