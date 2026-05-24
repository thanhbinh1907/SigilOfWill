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
            Debug.Log($">> [SPELL ACTION] AttemptToPerformAction bắt đầu. Chiêu thức: {name}, Mana hiện tại của Player: {player.currentMana}, Mana tiêu hao: {manaCost}");
            if (player.currentMana > 0)
            {
                Debug.Log($">> [SPELL ACTION] Mana hợp lệ (>0). Đang chạy animation '{spellAnimation}' và trừ {manaCost} mana...");
                player.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true);
                player.currentMana -= manaCost;
                player.playerCombatManager.currentSpellBeingCast = this;
                Debug.Log($">> [SPELL ACTION] Đã chạy xong lệnh PlayTargetAnimation. Mana còn lại: {player.currentMana}");
			}
            else
            {
                Debug.LogWarning($">> [SPELL ACTION] Thất bại: Không đủ mana! Mana hiện tại: {player.currentMana}");
			}
		}
	}
}