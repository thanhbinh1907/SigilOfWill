using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class CharacterAnimatorManager : MonoBehaviour
	{
		CharacterManager character;

		int horizontal;
		int vertical;

		[Header("Damage Animations")]
		public string lastDamageAnimationPlayed;

		[SerializeField] string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
		[SerializeField] string hit_Forward_Medium_02 = "Hit_Forward_Medium_02";

		[SerializeField] string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
		[SerializeField] string hit_Backward_Medium_02 = "Hit_Backward_Medium_02";

		[SerializeField] string hit_Left_Medium_01 = "Hit_Left_Medium_01";
		[SerializeField] string hit_Left_Medium_02 = "Hit_Left_Medium_02";

		[SerializeField] string hit_Right_Medium_01 = "Hit_Right_Medium_01";
		[SerializeField] string hit_Right_Medium_02 = "Hit_Right_Medium_02";

		public List<string> hit_Forward_Medium_List = new List<string>();
		public List<string> hit_Backward_Medium_List = new List<string>();
		public List<string> hit_Left_Medium_List = new List<string>();
		public List<string> hit_Right_Medium_List = new List<string>();

		protected virtual void Awake()
		{
			character = GetComponent<CharacterManager>();

			horizontal = Animator.StringToHash("Horizontal");
			vertical = Animator.StringToHash("Vertical");
		}

		protected virtual void Start()
		{
			// Add damage animations to lists
			hit_Forward_Medium_List.Add(hit_Forward_Medium_01);
			hit_Forward_Medium_List.Add(hit_Forward_Medium_02);

			hit_Backward_Medium_List.Add(hit_Backward_Medium_01);
			hit_Backward_Medium_List.Add(hit_Backward_Medium_02);

			hit_Left_Medium_List.Add(hit_Left_Medium_01);
			hit_Left_Medium_List.Add(hit_Left_Medium_02);

			hit_Right_Medium_List.Add(hit_Right_Medium_01);
			hit_Right_Medium_List.Add(hit_Right_Medium_02);
		}

		public string GetRandomAnimationFromList(List<string> animationList)
		{
			if (animationList == null || animationList.Count == 0)
			{
				return "";
			}

			List<string> finalList = new List<string>();

			foreach (var item in animationList)
			{
				if (!string.IsNullOrEmpty(item))
				{
					finalList.Add(item);
				}
			}

			if (finalList.Count > 1)
			{
				finalList.Remove(lastDamageAnimationPlayed);
			}

			if (finalList.Count == 0)
			{
				foreach (var item in animationList)
				{
					if (!string.IsNullOrEmpty(item))
					{
						finalList.Add(item);
					}
				}
			}

			if (finalList.Count == 0)
			{
				return "";
			}

			int randemValue = Random.Range(0, finalList.Count);

			return finalList[randemValue];
		}

		public void UpdateAnimatorMovementParameters(float horizontalParameters, float verticalParameters, bool isSprinting)
		{
			float horizontalAmout = horizontalParameters;
			float verticalAmout = verticalParameters;

			if (isSprinting)
			{
				verticalAmout = 2;
			}
			character.animator.SetFloat(horizontal, horizontalAmout, 0.1f, Time.deltaTime);
			character.animator.SetFloat(vertical, verticalAmout, 0.1f, Time.deltaTime);
		}

		public virtual void PlayTargetAnimation(
			string targetAnimation,
			bool isPerformingAction,
			bool applyRootMotion = true,
			bool canRotate = false,
			bool canMove = false)
		{
			if (string.IsNullOrEmpty(targetAnimation))
			{
				Debug.LogWarning("PlayTargetAnimation was called with a null or empty animation name!");
				return;
			}
			Debug.Log("Playing Animation: " + targetAnimation);
			if (character.animator == null)
			{
				Debug.LogError("character.animator is null on " + character.name);
				return;
			}
			character.applyRootMotion = applyRootMotion;
			character.animator.CrossFade(targetAnimation, 0.2f);
			// can be used to stop character movement during an animation, such as attacking or being hit
			character.isPerformingAction = isPerformingAction;
			character.canRotate = canRotate;
			character.canMove = canMove;
		}

		public virtual void PlayTargetAttackActionAnimation(AttackType attackType,
			string targetAnimation,
			bool isPerformingAction,
			bool applyRootMotion = true,
			bool canRotate = false,
			bool canMove = false)
		{
			// KEEP TRACK OF LAST ATTACK PERFORMED (FOR COMBOS)
			// KEEP TRACK OF CURRENT ATTACK TYPE (LIGHT, HEAVY, SPELL, ETC)
			// UPDATE ANIMATION SET TO CURRENT WEAPON ANIMATION
			// DECIDE IF OUR ATTACK CAN BE PARRIED

			if (string.IsNullOrEmpty(targetAnimation))
			{
				Debug.LogWarning("PlayTargetAttackActionAnimation was called with a null or empty animation name!");
				return;
			}
			Debug.Log("Playing Attack Animation: " + targetAnimation);
			if (character.animator == null)
			{
				Debug.LogError("character.animator is null on " + character.name);
				return;
			}
			if (character.characterCombatManager == null)
			{
				Debug.LogError("character.characterCombatManager is null on " + character.name);
				return;
			}
			character.characterCombatManager.currentAttackType = attackType;
			character.applyRootMotion = applyRootMotion;
			character.animator.CrossFade(targetAnimation, 0.2f);
			character.isPerformingAction = isPerformingAction;
			character.canRotate = canRotate;
			character.canMove = canMove;
		}
	}
}
