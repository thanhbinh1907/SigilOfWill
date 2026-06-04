using UnityEngine;

namespace SG
{
    public class AICharacterSpawner : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject characterGameObject;
        [SerializeField] GameObject instantiatedGameObject;

		private void Awake()
		{

		}

		private void Start()
		{
			if (WorldAIManager.instance != null)
			{
				WorldAIManager.instance.SpawnCharacter(this);
			}
			gameObject.SetActive(false);
		}

		public void AttemptToSpawnCharacter()
		{
			if (characterGameObject != null)
			{
				// Kiểm tra xem GameObject này có phải là Boss không
				AIBossCharacterManager bossCharacter = characterGameObject.GetComponent<AIBossCharacterManager>();
				if (bossCharacter == null)
				{
					bossCharacter = characterGameObject.GetComponentInChildren<AIBossCharacterManager>();
				}

				if (bossCharacter != null)
				{
					// Nếu là Boss, kiểm tra xem đã bị đánh bại trong file save hiện tại chưa
					if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
					{
						int bossID = bossCharacter.bossID;
						if (WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.ContainsKey(bossID))
						{
							if (WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID])
							{
								Debug.Log($"[SPAWNER] Boss ID {bossID} đã bị đánh bại từ trước (defeated = true). Bỏ qua không spawn.");
								return;
							}
						}
					}
				}

				instantiatedGameObject = Instantiate(characterGameObject, transform.position, transform.rotation);
				WorldAIManager.instance.AddSpawnedCharacter(instantiatedGameObject);
			}
		}
	}
}