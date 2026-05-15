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
				instantiatedGameObject = Instantiate(characterGameObject, transform.position, transform.rotation);
				WorldAIManager.instance.AddSpawnedCharacter(instantiatedGameObject);
			}
		}
	}
}