using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class CharacterCombatManager : MonoBehaviour
    {
		protected CharacterManager character;

		[Header("Last Attack Animation Performed")]
		public string lastAttackAnimationPerformed;

		[Header("Attack Target")]
		public CharacterManager currentTarget;

		public Transform lockOnTransform;

		protected virtual void Awake()
        {
            
		}

		public virtual void SetTarget(CharacterManager newTarget)
		{
			currentTarget = newTarget;
		}
	}
}