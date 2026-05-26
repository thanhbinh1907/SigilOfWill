using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public abstract class SpellAction : ScriptableObject
	{
		public int spellID;
		public string spellAnimation;
		public int manaCost;
		public GameObject spellPrefab;

		[Header("Spell Base Damage")]
		public int fireDamage = 0;
		public int lightningDamage = 0;
		public int windDamage = 0;

		public virtual void AttemptToPerformAction(PlayerManager player)
		{
			Debug.Log($">> [SPELL ACTION] AttemptToPerformAction bắt đầu. Chiêu thức: {name}, Mana hiện tại của Player: {player.currentMana}, Mana tiêu hao: {manaCost}");
			if (player.currentMana >= manaCost)
			{
				Debug.Log($">> [SPELL ACTION] Mana hợp lệ (>= {manaCost}). Đang chạy animation '{spellAnimation}' và trừ {manaCost} mana...");
				player.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true);
				player.currentMana -= manaCost;
				player.playerCombatManager.currentSpellBeingCast = this;
				Debug.Log($">> [SPELL ACTION] Đã chạy xong lệnh PlayTargetAnimation. Mana còn lại: {player.currentMana}");
			}
			else
			{
				Debug.LogWarning($">> [SPELL ACTION] Thất bại: Không đủ mana! Cần: {manaCost}, Hiện tại: {player.currentMana}");
			}
		}

		public abstract void SpawnSpell(PlayerManager player);

		protected void InitializeHitbox(GameObject spellObj, PlayerManager player)
		{
			SpellHitboxController hitboxController = spellObj.GetComponent<SpellHitboxController>();
			if (hitboxController == null) hitboxController = spellObj.GetComponentInChildren<SpellHitboxController>();

			if (hitboxController != null)
			{
				hitboxController.InitializeSpell(player, this);
			}
			else
			{
				Debug.LogWarning($"Spell Prefab '{name}' thiếu component SpellHitboxController!");
			}
		}
	}
}