using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
	public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; // If the damage is caused by another characters attack, it will be stored here

        [Header("Damage")]
        public float physicalDamage = 0;                // In the furure will be split into standard, strike, slash and thrust damage types
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float windDamage = 0;
        public float holyDamage = 0;

        [Header("Final Damage")]
        public int finalDamageDealt = 0;                // The final damage that will be applied to the character after all calculations are done. This is the value that will be used to reduce the character's health.       

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;              // If character's poise is broken, they will be Stunned and play a damage animation      

        // (TO DO) BUILD UP
        // build up effect amount

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("Sound FX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSFX;            // Use on top of regular SFX if there is elemental damage present

        [Header("Direction Damage Taken From")]
        public float angleHitFrom;                      // Used to determine what damage animation to play 
        public Vector3 contactPoint;                    // Used to determine where the blood FX instantiate

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            if (character.isDead)
                return;

            // CHECK FOR "INVULNERABILITY"

            CalculateDamage(character);
            PlayDirectionalBaseOnDamageAnimation(character);
			// CHECK FOR BUILD UP (POISON, BLEED, ETC)
			PlayDamageSFX(character);
			PlayDamageVFX(character);

			// IF CHARACTER IS A.I, CHECK FOR NEW TARGET IF CHARACTER CAUSING DAMAGE IS PRESENT 
		}

        private void CalculateDamage(CharacterManager character)
        {
            if (characterCausingDamage != null)
            {
                // CHECK FOR DAMAGE MODIFIERS AND MODIFY BASE DAMAGE (PHYSICAL, ELEMENTAL DAMAGE BUFF)  
            }
			// CHECK CHARACTER FOR FLAT DEFENSE AND SUBTRACT THEM FROM DAMAGE

			// CHECK CHARACTER FOR ARMOR ABORPTION AND CALCULATE DAMAGE REDUCTION

            // ADD ALL DAMAGE TYPES TOGETHER, AND APPLY FINAL DAMAGE

            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + windDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }

            Debug.Log("Final Damage Dealt: " + finalDamageDealt);

			character.currentHealth -= finalDamageDealt;

			// CALCULATE POISE DAMAGE TO DETERMINE IF CHARATER WILL ME STUNNED OR NOT
		}

        private void PlayDamageVFX(CharacterManager character)
        {
            character.characterEffectsManager.PlayeBloodSplatterVFX(contactPoint);
		}

        private void PlayDamageSFX(CharacterManager character)
        {
            AudioClip slashDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.slashSFX);

			character.characterSoundFXManager.PlaySoundFX(slashDamageSFX);
		}

        private void PlayDirectionalBaseOnDamageAnimation(CharacterManager character)
		{
            if (character.isDead)
                return;

			// CALCULATE IF POISE IS BROKEN
			poiseIsBroken = true;

			// WHY ARE THE ANGLE LIKE THIS ?
			// YOU CAN IMAGINE THAT U ARE THE CHARACTER AND 0 DEGREE IS DIRECTLY IN BACK OF YOU, 90 DEGREE IS TO YOUR RIGHT,
            // -90 DEGREE IS TO YOUR LEFT AND 180 OR -180 DEGREE IS DIRECTLY BACK YOU.

			if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
				damageAnimation  = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.hit_Forward_Medium_List);
			}
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
				damageAnimation  = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.hit_Forward_Medium_List);
			}
			else if (angleHitFrom >= -45 && angleHitFrom <= 45)
			{
				damageAnimation  = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.hit_Backward_Medium_List);
			}
			else if (angleHitFrom >= -144 && angleHitFrom <= -45)
			{
				damageAnimation  = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.hit_Left_Medium_List);
			}
			else if (angleHitFrom >= 45 && angleHitFrom <= 144)
			{
				damageAnimation  = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.hit_Right_Medium_List);
			}

            if (poiseIsBroken)
            {
                character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
				character.characterAnimatorManager.PlayTargetAnimation(damageAnimation, true);
			}
		}
	}
}