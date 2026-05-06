using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        [Header("Detection")]
        [SerializeField] float detectionRadius = 15;

		private void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget == null)
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
			}
		}

	}
}