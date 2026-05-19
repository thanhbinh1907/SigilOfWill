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
		[SerializeField] FrostGiantDamageCollider rightHandDamageCollider;
		[SerializeField] FrostGiantDamageCollider rightForceArmDamageCollider;

		[Header("Damage")]
		[SerializeField] int baseDamage = 40;
		[SerializeField] float AttackFront01DamageModifier = 1f;
		[SerializeField] float AttackFront02DamageModifier = 1f;
		[SerializeField] float AttackFront03DamageModifier = 1.5f;
		[SerializeField] float AttackBackDamageModifier = 1.5f;
		[SerializeField] float AttackGround01DamageModifier = 1.5f;
		[SerializeField] float AttackGround02DamageModifier = 2f;
		[SerializeField] float AttackJumpDamageModifier = 2f;
		[SerializeField] float AttackDashDamageModifier = 2f;
		//[SerializeField] float AttackThrowStoneDamageModifier = 3f;

		[Header("AOE Impact Point")]
		[SerializeField] private Transform oneHandImpactPoint;
		[SerializeField] private Transform twoHandImpactPoint;
		[SerializeField] private Transform jumpImpactPoint;

		[Header("AOE Radius")]
		[SerializeField] private float groundSlam01Radius = 2.5f;
		[SerializeField] private float groundSlam02Radius = 3.5f;
		[SerializeField] private float jumpSlamRadius = 4f;

		[Header("Gizmos Toggle")]
		[SerializeField] private bool showGroundSlam01Gizmo = true; 
		[SerializeField] private bool showGroundSlam02Gizmo = true; 
		[SerializeField] private bool showJumpSlamGizmo = true;

		protected virtual void Start()
		{
			if (aiCharacter != null)
			{
				if (leftHandDamageCollider != null) leftHandDamageCollider.characterCausingDamage = aiCharacter;
				if (leftForceArmDamageCollider != null) leftForceArmDamageCollider.characterCausingDamage = aiCharacter;

				if (rightHandDamageCollider != null) rightHandDamageCollider.characterCausingDamage = aiCharacter;
				if (rightForceArmDamageCollider != null) rightForceArmDamageCollider.characterCausingDamage = aiCharacter;
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

		public void SetAttackDashDamage()
		{
			rightHandDamageCollider.physicalDamage = baseDamage * AttackDashDamageModifier;
			leftHandDamageCollider.physicalDamage = baseDamage * AttackDashDamageModifier;
			rightForceArmDamageCollider.physicalDamage = baseDamage * AttackDashDamageModifier;
			leftForceArmDamageCollider.physicalDamage = baseDamage * AttackDashDamageModifier;
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

		public void ActivateGiantGroundSlam01()
		{
			float calculatedDamage = baseDamage * AttackGround01DamageModifier;
			Transform impactPoint = oneHandImpactPoint;
			ExecuteAOEExplosion(impactPoint.position, groundSlam01Radius, calculatedDamage);
		}

		public void ActivateGiantGroundSlam02()
		{
			float calculatedDamage = baseDamage * AttackGround02DamageModifier;
			Transform impactPoint = twoHandImpactPoint;
			ExecuteAOEExplosion(impactPoint.position, groundSlam02Radius, calculatedDamage);
		}

		public void ActiveGiantJumpSlam()
		{
			float calculatedDamage = baseDamage * AttackJumpDamageModifier;
			Transform impactPoint = jumpImpactPoint;
			ExecuteAOEExplosion(impactPoint.position, jumpSlamRadius, calculatedDamage);
		}

		private void ExecuteAOEExplosion(Vector3 impactPosition, float radius, float damageValue)
		{
			List<CharacterManager> damagedCharacters = new List<CharacterManager>();

			Collider[] colliders = Physics.OverlapSphere(impactPosition, radius, WorldUtilityManager.instance.GetCharacterLayers());

			foreach (var collider in colliders)
			{
				CharacterManager targetCharacter = collider.GetComponentInParent<CharacterManager>();

				if (targetCharacter != null)
				{
					if (targetCharacter == aiCharacter)
						continue;

					if (!damagedCharacters.Contains(targetCharacter))
					{
						damagedCharacters.Add(targetCharacter);

						if (targetCharacter.isInvulnerable)
							continue;

						TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
						damageEffect.physicalDamage = damageValue;
						damageEffect.poiseDamage = damageValue;

						targetCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
					}
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (showGroundSlam01Gizmo && oneHandImpactPoint != null)
			{
				Gizmos.color = Color.red; 
				Gizmos.DrawWireSphere(oneHandImpactPoint.position, groundSlam01Radius);
			}

			if (showGroundSlam02Gizmo && twoHandImpactPoint != null)
			{
				Gizmos.color = Color.yellow; 
				Gizmos.DrawWireSphere(twoHandImpactPoint.position, groundSlam02Radius);
			}

			if (showJumpSlamGizmo && jumpImpactPoint != null)
			{
				Gizmos.color = Color.blue; 
				Gizmos.DrawWireSphere(jumpImpactPoint.position, jumpSlamRadius);
			}
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