using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class CharacterCombatManager : MonoBehaviour
    {
		[Header("Attack Target")]
		public CharacterManager currentTarget;
		protected virtual void Awake()
        {
            
		}
	}
}