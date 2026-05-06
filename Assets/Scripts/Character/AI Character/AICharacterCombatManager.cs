using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        [Header("Detection")]
        [SerializeField] float detectionRadius = 15;
        [SerializeField] float minimunDetectionAngle = -35;
        [SerializeField] float maximumDetectionAngle = 35;

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

                if  (targetCharacter.isDead)
                    continue;

				// CAN I ATTACK THIS CHARACTER, IF SO, MAKE THEM MY TARGET
				if (WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
				{
					//  IF A POTENTIAL TARGET IS FOUND, IT HAS TO BE INFONT OF US
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float viewableAngle = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if (viewableAngle > minimunDetectionAngle && viewableAngle < maximumDetectionAngle)
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
							aiCharacter.characterCombatManager.SetTarget(targetCharacter);
						}
					}
				}
			}
		}

	}
}