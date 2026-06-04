using UnityEngine;

namespace SG
{
	public class EventTriggerBossFight : MonoBehaviour
	{
		[Header("Boss Fight Settings")]
		public int bossID = 0;

		private void OnTriggerEnter(Collider other)
		{
			PlayerManager player = other.GetComponentInParent<PlayerManager>();
			if (player != null)
			{
				Debug.Log($"[HỆ THỐNG TRIGGER] Player '{player.name}' va chạm với EventTriggerBossFight của Boss ID {bossID}!");
				if (WorldAIManager.instance != null)
				{
					AIBossCharacterManager boss = WorldAIManager.instance.GetBossCharacterByID(bossID);
					if (boss != null)
					{
						Debug.Log($"[HỆ THỐNG TRIGGER] Đã tìm thấy Boss ID {bossID} ('{boss.characterName}'). Gọi WakeBoss().");
						boss.WakeBoss();
						gameObject.SetActive(false);
					}
					else
					{
						Debug.LogWarning($"[HỆ THỐNG TRIGGER] Không tìm thấy Boss ID {bossID} hoạt động trên scene để kích hoạt!");
					}
				}
				else
				{
					Debug.LogError("[HỆ THỐNG TRIGGER] WorldAIManager.instance đang bị NULL!");
				}
			}
		}
	}
}
