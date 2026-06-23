using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace SG
{
    public class AICharacterManager : CharacterManager
    {
        [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
        [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;

		[Header("Navmesh Agent")]
        public NavMeshAgent navMeshAgent;

		[Header("Current State")]
        [SerializeField] AIState currentState;

        [Header("States")]
        public IdleState idle;
        public PursueTargetState pursueTarget;
        public CombatStanceState combatStance;
        public AttackState attack;

		protected override void Awake()
        {
            base.Awake();

            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
            aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();

			navMeshAgent = GetComponentInChildren<NavMeshAgent>();

            // USE A COPY OF THE SCRIPTABLE OBJECTS, SO ORIGINALS ARE NOT MODIFIED
            idle = Instantiate(idle);
            pursueTarget = Instantiate(pursueTarget);
            combatStance = Instantiate(combatStance);

            currentState = idle;
		}

		protected override void Update()
		{
			base.Update();

            aiCharacterCombatManager.HandleActionRecovery(this);
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
            ProcessStateMachine();

		}

        private void ProcessStateMachine()
        {
            if (navMeshAgent != null && navMeshAgent.enabled && !navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning($">> [AI MANAGER] {name}'s NavMeshAgent is enabled but off-mesh! Disabling it to prevent crash.");
                navMeshAgent.enabled = false;
            }

            if (aiCharacterCombatManager.currentTarget != null)
            {
                aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
                aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetsDirection);
                aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);

                if (aiCharacterCombatManager.distanceFromTarget > aiCharacterCombatManager.distanceToLoseTarget)
                {
                    aiCharacterCombatManager.SetTarget(null);
                }
            }

            AIState nextState = currentState?.Tick(this);

            if (nextState != null)
            {
                currentState = nextState;
            }

			// THE POSITION/ROTATION SHOULD BE RESET ONLY AFTER THE STATE MACHINE HAS PROCESSED IT'S TICK
			navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;

			if (navMeshAgent.enabled)
            {
                if (navMeshAgent.velocity.sqrMagnitude > 0.05f)
                {
                    isMoving = true;
                }
                else
                {
                    isMoving = false;
				}
			}
            else
            {
                isMoving = false;
			}
		}

        public void SetCurrentState(AIState newState)
        {
            currentState = newState;
        }
	}
}