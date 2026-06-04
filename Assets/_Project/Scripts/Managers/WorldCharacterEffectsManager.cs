using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
		public static WorldCharacterEffectsManager instance;

		[Header("VFX")]
		public GameObject bloodSplatterVFX;

		[Header("Damage")]
		public TakeDamageEffect takeDamageEffect;

		[SerializeField] List<InstantCharacterEffect> instantEffects;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}

			GenerateEffectIDs();
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		private void GenerateEffectIDs()
		{
			for (int i = 0; i < instantEffects.Count; i++)
			{
				instantEffects[i].instantEffectID = i;
			}
		}
	}
}