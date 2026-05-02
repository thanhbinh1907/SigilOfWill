using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	[CreateAssetMenu(menuName = "Character Actions/Spell Actions/New Spell Action")]
	public class SpellAction : ScriptableObject
	{
        public int spellID;
        public string spellAnimation;
        public int manaCost;
        public GameObject spellPrefab;

        [Header("Spell Base Damage")]
        public int fireDamage = 0;
        public int lightningDamage = 0;
        public int windDamage = 0;

		[Header("Spell Cast Type")]
		public bool isSpellFromSky = false;
		public bool isMeleeSpell = false;

		[Header("Projectile Settings")]
		public float projectileSpeed = 10f;

		public virtual void AttemptToPerformAction(PlayerManager player)
        {
            if (player.currentMana > 0)
            {
                player.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true);
                player.currentMana -= manaCost;
                player.playerCombatManager.currentSpellBeingCast = this;
			}
            else
            {
                Debug.Log("Not enough stamina to perform this action.");
			}
                
		}
	}
}