using UnityEngine;
using TMPro;

namespace SG
{
	public class UI_BossHPBar : UI_StatBar
	{
		[Header("Boss Details")]
		[SerializeField] TextMeshProUGUI bossNameText;
		[SerializeField] AIBossCharacterManager bossCharacter;

		public void EnableBossHPBar(AIBossCharacterManager boss)
		{
			bossCharacter = boss;

			if (bossNameText != null)
			{
				bossNameText.text = boss.characterName;
			}

			SetMaxStat(boss.maxHealth);
			SetStat(boss.currentHealth);


			bossCharacter.OnHealthChanged += SetBossHP;
		}

		private void OnDestroy()
		{
			if (bossCharacter != null)
			{
				bossCharacter.OnHealthChanged -= SetBossHP;
			}
		}

		private void SetBossHP(int oldValue, int newValue)
		{
			SetStat(newValue);
		}

		public AIBossCharacterManager GetBossCharacter()
		{
			return bossCharacter;
		}
	}
}
