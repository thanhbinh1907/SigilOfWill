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

				AIBossCharacterManager bossCharacter = characterGameObject.GetComponent<AIBossCharacterManager>();
				if (bossCharacter == null)
				{
					bossCharacter = characterGameObject.GetComponentInChildren<AIBossCharacterManager>();
				}

				if (bossCharacter != null)
				{

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


				UnityEngine.AI.NavMeshAgent agent = instantiatedGameObject.GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
				if (agent != null)
				{
					UnityEngine.AI.NavMeshHit hit;
					if (UnityEngine.AI.NavMesh.SamplePosition(instantiatedGameObject.transform.position, out hit, 10.0f, UnityEngine.AI.NavMesh.AllAreas))
					{
						bool prevEnabled = agent.enabled;
						agent.enabled = false;
						instantiatedGameObject.transform.position = hit.position;
						agent.enabled = prevEnabled;
					}
					else
					{

						agent.enabled = false;
					}
				}

				WorldAIManager.instance.AddSpawnedCharacter(instantiatedGameObject);
			}
		}
	}
}