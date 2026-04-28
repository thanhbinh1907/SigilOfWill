using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class WeaponItem : Item
    {
		// ANIMATOR CONTROLLER OVERRIDE (CHANGE ATTACK ANIMATIONS BASED ON WEAPON YOU ARE CURRENTLY USING)

		[Header("Weapon Model")]
		public GameObject weaponModel;

		[Header("Weapon Requirement")]
		public int strengREQ = 0;
		public int dexREQ = 0;
		public int intREQ = 0;
		public int faithREQ = 0;

		[Header("Weapon Base Damage")]
		public int physicalDMG = 0;
		public int magicDMG = 0;
		public int fireDMG = 0;
		public int lightningDMG = 0;
		public int holyDMG = 0;
		public int WindDMG = 0;

		// WEAPON GUARD ABSORPTIONS (BLOCKING POWER)

		[Header("Weapon Base Poise Damage")]
		public float poiseDMG = 10;
		// OFFENSIVE POISE BONUS WHEN ATTACKING 

		// WEAPON MODIFIERS
		// LIGHT ATTACK MODIFIER
		// HEAVY ATTACK MODIFIER
		// CRITICAL ATTACK MODIFIER ETC 

		[Header("Stamina Cost")]
		public int baseStaminaCost = 0;
		// RUNNING ATTACK STAMINA COST MODIFIER 
		// LIGHT ATTACK STAMINA COST MODIFIER
		// HEAVY ATTACK STAMINA COST MODIFIER

		// ITEM BASE ACTION 

		// ASH OF WAR

		// BLOCKING SOUNDS
	}
}