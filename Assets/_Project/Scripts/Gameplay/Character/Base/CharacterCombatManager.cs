using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class CharacterCombatManager : MonoBehaviour
    {
		protected CharacterManager character;
		public AttackType currentAttackType;

		[Header("Last Attack Animation Performed")]
		public string lastAttackAnimationPerformed;

		[Header("Attack Target")]
		public CharacterManager currentTarget;

		public Transform lockOnTransform;

		protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
		}

		public virtual void SetTarget(CharacterManager newTarget)
		{
			currentTarget = newTarget;
		}

		public void EnableIsInvulnerable()
		{
			character.isInvulnerable = true;
		}

		public void DisableIsInvulnerable()
		{
			character.isInvulnerable = false;
		}
	}
}