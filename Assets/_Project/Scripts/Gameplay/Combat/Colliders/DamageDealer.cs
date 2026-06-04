using UnityEngine;

namespace SG
{
    public class DamageDealer : MonoBehaviour
    {
        [Header("Damage Causer")]
        public CharacterManager characterCausingDamage;

        [Header("Damage Value")]
        public float physicalDamage = 0;
        public float fireDamage = 0;
        public float magicDamage = 0;
        public float lightningDamage = 0;
        public float windDamage = 0;
        public float holyDamage = 0;

        protected virtual void ApplyDamage(CharacterManager target, Vector3 contactPoint)
        {
            if (target.isInvulnerable)
                return;

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.characterCausingDamage = characterCausingDamage;
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.windDamage = windDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contactPoint;

            if (characterCausingDamage != null)
            {
                damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, target.transform.forward, Vector3.up);
            }

            target.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }
    }
}
