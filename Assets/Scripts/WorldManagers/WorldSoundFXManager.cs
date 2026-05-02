using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager instance;

        [Header("Damage Sound")]
        public AudioClip[] slashSFX;

		[Header("Action Sounds")]
		public AudioClip rollSFX;

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

        public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
        {
            int index = Random.Range(0, array.Length);
            return array[index];
		}
	}
}