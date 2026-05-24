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

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

		public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
        {
            int index = Random.Range(0, array.Length);
            return array[index];
		}

        /*
        public AudioClip ChooseRandomFootStepSoundBasedOnGround(GameObject steppedOnObject, CharacterManager character) 
        {
            if (steppedOnObject.tag == "Dirt")
            {
                return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsDirt);
			}
            else if (steppedOnObject.tag == "Stone")
			{
                return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsStone);
			}

            return null;
        }
        */
	}
}