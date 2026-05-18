using SG;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class AIFrostGiantCombatManager : AICharacterCombatManager
	{
		[Header("Damage Collider")]
		[SerializeField] FrostGiantDamageCollider leftHandDamageCollider;
		[SerializeField] FrostGiantDamageCollider leftForceArmDamageCollider;
		[SerializeField] FrostGiantDamageCollider leftLegDamageCollider;
		[SerializeField] FrostGiantDamageCollider leftFootDamageCollider;

		[SerializeField] FrostGiantDamageCollider rightHandDamageCollider;
		[SerializeField] FrostGiantDamageCollider rightForceArmDamageCollider;
		[SerializeField] FrostGiantDamageCollider rightLegDamageCollider;
		[SerializeField] FrostGiantDamageCollider rightFootDamageCollider;


		[Header("Damage")]
		[SerializeField] int baseDamage = 40;
		[SerializeField] float stompDamage = 40;
		[SerializeField] float AttackFront01DamageModifier = 1f;
		[SerializeField] float AttackFront02DamageModifier = 1f;
		[SerializeField] float AttackFront03DamageModifier = 1.5f;
		[SerializeField] float AttackBackDamageModifier = 1.5f;
		[SerializeField] float AttackGround01DamageModifier = 1.5f;
		[SerializeField] float AttackGround02DamageModifier = 2f;
		[SerializeField] float AttackJumpDamageModifier = 2f;
		[SerializeField] float AttackDashDamageModifier = 2f;
		[SerializeField] float AttackThrowStoneDamageModifier = 3f;

		protected virtual void Start()
		{
			if (aiCharacter != null)
			{
				if (leftHandDamageCollider != null) leftHandDamageCollider.characterCausingDamage = aiCharacter;
				if (leftForceArmDamageCollider != null) leftForceArmDamageCollider.characterCausingDamage = aiCharacter;
				if (leftLegDamageCollider != null) leftLegDamageCollider.characterCausingDamage = aiCharacter;
				if (leftFootDamageCollider != null) leftFootDamageCollider.characterCausingDamage = aiCharacter;

				if (rightHandDamageCollider != null) rightHandDamageCollider.characterCausingDamage = aiCharacter;
				if (rightForceArmDamageCollider != null) rightForceArmDamageCollider.characterCausingDamage = aiCharacter;
				if (rightLegDamageCollider != null) rightLegDamageCollider.characterCausingDamage = aiCharacter;
				if (rightFootDamageCollider != null) rightFootDamageCollider.characterCausingDamage = aiCharacter;
			}
		}

		public void SetAttackFront01Damage()
		{
			rightHandDamageCollider.physicalDamage = baseDamage * AttackFront01DamageModifier;
			leftHandDamageCollider.physicalDamage = baseDamage * AttackFront01DamageModifier;
			rightForceArmDamageCollider.physicalDamage = baseDamage * AttackFront01DamageModifier;
			leftForceArmDamageCollider.physicalDamage = baseDamage * AttackFront01DamageModifier;
		}

		public void SetAttackFront02Damage()
		{
			rightHandDamageCollider.physicalDamage = baseDamage * AttackFront02DamageModifier;
			leftHandDamageCollider.physicalDamage = baseDamage * AttackFront02DamageModifier;
			rightForceArmDamageCollider.physicalDamage = baseDamage * AttackFront02DamageModifier;
			leftForceArmDamageCollider.physicalDamage = baseDamage * AttackFront02DamageModifier;
		}

		public void SetAttackFront03Damage()
		{
			rightHandDamageCollider.physicalDamage = baseDamage * AttackFront03DamageModifier;
			leftHandDamageCollider.physicalDamage = baseDamage * AttackFront03DamageModifier;
			rightForceArmDamageCollider.physicalDamage = baseDamage * AttackFront03DamageModifier;
			leftForceArmDamageCollider.physicalDamage = baseDamage * AttackFront03DamageModifier;
		}

		public void SetAttackBackDamage()
		{
			rightHandDamageCollider.physicalDamage = baseDamage * AttackBackDamageModifier;
			leftHandDamageCollider.physicalDamage = baseDamage * AttackBackDamageModifier;
			rightForceArmDamageCollider.physicalDamage = baseDamage * AttackBackDamageModifier;
			leftForceArmDamageCollider.physicalDamage = baseDamage * AttackBackDamageModifier;
		}

		public void SetAttackGround01Damage()
		{
			rightHandDamageCollider.physicalDamage = baseDamage * AttackGround01DamageModifier;
			leftHandDamageCollider.physicalDamage = baseDamage * AttackGround01DamageModifier;
			rightForceArmDamageCollider.physicalDamage = baseDamage * AttackGround01DamageModifier;
			leftForceArmDamageCollider.physicalDamage = baseDamage * AttackGround01DamageModifier;
		}

		public void SetAttackGround02Damage()
		{
			rightHandDamageCollider.physicalDamage = baseDamage * AttackGround02DamageModifier;
			leftHandDamageCollider.physicalDamage = baseDamage * AttackGround02DamageModifier;
			rightForceArmDamageCollider.physicalDamage = baseDamage * AttackGround02DamageModifier;
			leftForceArmDamageCollider.physicalDamage = baseDamage * AttackGround02DamageModifier;
		}

		public void SetAttackJumpDamage()
		{
			rightLegDamageCollider.physicalDamage = baseDamage * AttackJumpDamageModifier;
			leftLegDamageCollider.physicalDamage = baseDamage * AttackJumpDamageModifier;
			rightFootDamageCollider.physicalDamage = baseDamage * AttackJumpDamageModifier;
			leftFootDamageCollider.physicalDamage = baseDamage * AttackJumpDamageModifier;
		}

		public void SetAttackDashDamage()
		{
			rightHandDamageCollider.physicalDamage = baseDamage * AttackFront03DamageModifier;
			leftHandDamageCollider.physicalDamage = baseDamage * AttackFront03DamageModifier;
			rightForceArmDamageCollider.physicalDamage = baseDamage * AttackFront03DamageModifier;
			leftForceArmDamageCollider.physicalDamage = baseDamage * AttackFront03DamageModifier;

			rightLegDamageCollider.physicalDamage = baseDamage * AttackDashDamageModifier;
			leftLegDamageCollider.physicalDamage = baseDamage * AttackDashDamageModifier;
			rightFootDamageCollider.physicalDamage = baseDamage * AttackDashDamageModifier;
			leftFootDamageCollider.physicalDamage = baseDamage * AttackDashDamageModifier;
		}

		public void OpenRightHandCollider()
		{
			//aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
			rightHandDamageCollider.EnableDamageCollider();
			rightForceArmDamageCollider.EnableDamageCollider();
		}

		public void CloseRightHandCollider()
		{
			rightHandDamageCollider.DisableDamageCollider();
			rightForceArmDamageCollider.DisableDamageCollider();
		}

		public void OpenLeftHandCollider()
		{
			//aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
			leftHandDamageCollider.EnableDamageCollider();
			leftForceArmDamageCollider.EnableDamageCollider();
		}

		public void CloseLeftHandCollider()
		{
			leftHandDamageCollider.DisableDamageCollider();
			leftForceArmDamageCollider.DisableDamageCollider();
		}

		public void OpenRightLegCollider()
		{
			//aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
			rightLegDamageCollider.EnableDamageCollider();
			rightFootDamageCollider.EnableDamageCollider();
		}

		public void CloseRightLegCollider()
		{
			rightLegDamageCollider.DisableDamageCollider();
			rightFootDamageCollider.DisableDamageCollider();
		}

		public void OpenLeftLegCollider()
		{
			//aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
			leftLegDamageCollider.EnableDamageCollider();
			leftFootDamageCollider.EnableDamageCollider();
		}

		public void CloseLeftLegCollider()
		{
			leftLegDamageCollider.DisableDamageCollider();
			leftFootDamageCollider.DisableDamageCollider();
		}

		public void ActivateGiantStomp()
		{

		}

		public override void PivotTowardsTarget(AICharacterManager aiCharacter)
		{
			if (aiCharacter.isPerformingAction)
				return;

			if (viewableAngle >= 61 && viewableAngle <= 110)
			{
				aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Right_90", true);
			}
			else if (viewableAngle <= -61 && viewableAngle >= -110)
			{
				aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Left_90", true);
			}
			else if (viewableAngle >= 146 && viewableAngle <= 180)
			{
				aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Right_180", true);
			}
			else if (viewableAngle <= -146 && viewableAngle >= -180)
			{
				aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Left_180", true);
			}
		}
	}
}