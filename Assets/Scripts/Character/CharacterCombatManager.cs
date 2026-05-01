using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class CharacterCombatManager : MonoBehaviour
    {
        CharacterCombatManager character;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterCombatManager>();
		}
	}
}