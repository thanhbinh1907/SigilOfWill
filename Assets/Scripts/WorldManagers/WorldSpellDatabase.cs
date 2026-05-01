using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SG
{
    public class WorldSpellDatabase : MonoBehaviour
    {
        public static WorldSpellDatabase instance;

        [Header("Spell List")]
        [SerializeField] private List<SpellAction> spellActions = new List<SpellAction>();

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public SpellAction GetSpellActionByID(int spellID)
		{
			return spellActions.FirstOrDefault(spell => spell.spellID == spellID);
		}
	}
}