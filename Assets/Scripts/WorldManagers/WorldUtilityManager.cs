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

		public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
		{
			if (attackingCharacter == CharacterGroup.Team1)
			{
				switch (targetCharacter)
				{
					case CharacterGroup.Team1: return false;
					case CharacterGroup.Team2: return true;
					default:
						break;	
				}
			}
			else if (attackingCharacter == CharacterGroup.Team2)
			{
				switch (targetCharacter)
				{
					case CharacterGroup.Team1: return true;
					case CharacterGroup.Team2: return false;
					default:
						break;
				}
			}

			return false;
		}
	}
}