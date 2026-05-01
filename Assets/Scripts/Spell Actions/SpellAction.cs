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
        public int staminaCost;
        public GameObject spellPrefab;

        [Header("Spell Base Damage")]
        public int fireDamage = 0;
        public int lightningDamage = 0;
        public int windDamage = 0;

		[Header("Projectile Settings")]
		public float projectileSpeed = 10f;

		public virtual void AttemptToPerformAction(PlayerManager player)
        {
            if (player.currentStamina > 0)
            {
                player.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true);
                player.currentStamina -= staminaCost;
                player.playerCombatManager.currentSpellBeingCast = this;
			}
            else
            {
                Debug.Log("Not enough stamina to perform this action.");
			}
                
		}
	}
}