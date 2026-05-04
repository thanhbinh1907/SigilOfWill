using UnityEngine;

namespace SG
{
	public class WorldUtilityManager : MonoBehaviour
	{
		public static WorldUtilityManager instance;

		[Header("Layer")]
		[SerializeField] LayerMask characterLayers;
		[SerializeField] LayerMask environmentLayers;

		private void Awake()
		{
			if (instance == null)
				instance = this;
			else
				Destroy(gameObject);
		}

		public LayerMask GetCharacterLayers()
		{
			return characterLayers;
		}

		public LayerMask GetEnvironmentLayers()
		{
			return environmentLayers;
		}
	}
}