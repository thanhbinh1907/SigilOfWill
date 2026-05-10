using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        [Header("Action Recovery")]
        public float actionRecoveryTimer = 0;

		[Header("Target Information")]
        public float distanceFromTarget;
		public float viewableAngle;
        public Vector3 targetsDirection;

        [Header("Detection")]
        [SerializeField] float detectionRadius = 15;
        public float minimunFOV = -35;
        public float maximumFOV = 35;

        [Header("Attack Rotation Speed")]
        public float attackRotationSpeed = 25;

        protected override void Awake()
        {
            base.Awake();

            lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
		}

		public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null)
                return;

            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].GetComponent<CharacterManager>();

                if (targetCharacter == null)
                    continue;

                if (targetCharacter == aiCharacter)
                    continue;

                if (targetCharacter.isDead)
                    continue;

                // CAN I ATTACK THIS CHARACTER, IF SO, MAKE THEM MY TARGET
                if (WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
                {
                    //  IF A POTENTIAL TARGET IS FOUND, IT HAS TO BE INFONT OF US
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if (angleOfPotentialTarget > minimunFOV && angleOfPotentialTarget < maximumFOV)
                    {
                        // LASTLY, WE CHECK ENVIRONMENT BLOCK
                        if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position,
                                             targetCharacter.characterCombatManager.lockOnTransform.position,
                                             WorldUtilityManager.instance.GetEnvironmentLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                            Debug.Log("BLOCKED");
                        }
                        else
                        {
                            targetsDirection = targetCharacter.transform.position - transform.position;
                            viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetsDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                            PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }

        public void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            // PLAY A PIVOT ANIMATION DEPENDING ON VIEW ANGLE OF TARGET
            if (aiCharacter.isPerformingAction)
                return;

            if (viewableAngle >= 20 && viewableAngle <= 60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Right_45", true);
            }
            else if (viewableAngle <= -20 && viewableAngle >= -60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Left_45", true);
            }
            else if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Right_90", true);
            }
            else if (viewableAngle <= -61 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Left_90", true);
            }
            else if (viewableAngle >= 111 && viewableAngle <= 145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Right_135", true);
            }
            else if (viewableAngle <= -111 && viewableAngle >= -145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetAnimation("Turn_Left_135", true);
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

        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isMoving)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
			}
		}

        public void RotateTowardsTargetWhilistAttacking(AICharacterManager aiCharacter)
        {
            if (currentTarget == null)
                return;

            if (!aiCharacter.canRotate)
                return;

            if (!aiCharacter.isPerformingAction)
                return;

			Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
		}

		public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if (actionRecoveryTimer > 0)
            {
                if (!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
				}
			}
		}
    }
}