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
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
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

		public float GetAngleOfTarget(Transform characterTransform, Vector3 targetsDirection)
		{
			targetsDirection.y = 0;
			float viewableAngle = Vector3.Angle(characterTransform.forward, targetsDirection);
			Vector3 cross = Vector3.Cross(characterTransform.forward, targetsDirection);

			if (cross.y < 0)
				viewableAngle = -viewableAngle;

			return viewableAngle;
		}
	}
}