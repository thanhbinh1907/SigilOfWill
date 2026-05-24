/*This script created by using docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
    public class ParticleCollisionInstance : MonoBehaviour
    {
        public GameObject[] EffectsOnCollision;
        public float DestroyTimeDelay = 5;
        public bool UseWorldSpacePosition;
        public float Offset = 0;
        public Vector3 rotationOffset = new Vector3(0, 0, 0);
        public bool useOnlyRotationOffset = true;
        public bool UseFirePointRotation;
        public bool DestoyMainEffect = true;
        private ParticleSystem part;
        private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        private ParticleSystem ps;

        [Header("Gameplay Damage Settings")]
        public CharacterManager characterCausingDamage;
        public float physicalDamage = 0;
        public float fireDamage = 0;
        public float magicDamage = 0;
        public float lightningDamage = 0;
        public float windDamage = 0;
        public float holyDamage = 0;

        // Cooldown tránh việc 1 hạt va chạm nhiều lần trong cùng 1 frame gây chết quái lập tức
        private Dictionary<CharacterManager, float> damageCooldowns = new Dictionary<CharacterManager, float>();
        private float hitCooldown = 0.25f; // Giãn cách 0.25 giây giữa các lần trúng sét của mỗi mục tiêu

        void Start()
        {
            part = GetComponent<ParticleSystem>();
        }
        void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            // Kiểm tra va chạm để trừ máu quái vật
            CharacterManager targetCharacter = other.GetComponentInParent<CharacterManager>();
            if (targetCharacter != null && targetCharacter != characterCausingDamage && !targetCharacter.isDead)
            {
                bool canDamage = true;
                if (damageCooldowns.ContainsKey(targetCharacter))
                {
                    if (Time.time - damageCooldowns[targetCharacter] < hitCooldown)
                    {
                        canDamage = false;
                    }
                }

                if (canDamage && !targetCharacter.isInvulnerable)
                {
                    damageCooldowns[targetCharacter] = Time.time;

                    TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                    damageEffect.characterCausingDamage = characterCausingDamage;
                    damageEffect.physicalDamage = physicalDamage;
                    damageEffect.fireDamage = fireDamage;
                    damageEffect.magicDamage = magicDamage;
                    damageEffect.lightningDamage = lightningDamage;
                    damageEffect.windDamage = windDamage;
                    damageEffect.holyDamage = holyDamage;

                    if (collisionEvents.Count > 0)
                    {
                        damageEffect.contactPoint = collisionEvents[0].intersection;
                    }
                    else
                    {
                        damageEffect.contactPoint = targetCharacter.transform.position;
                    }

                    targetCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
                }
            }

            for (int i = 0; i < numCollisionEvents; i++)
            {
                foreach (var effect in EffectsOnCollision)
                {
                    var instance = Instantiate(effect, collisionEvents[i].intersection + collisionEvents[i].normal * Offset, new Quaternion()) as GameObject;
                    if (!UseWorldSpacePosition) instance.transform.parent = transform;
                    if (UseFirePointRotation) { instance.transform.LookAt(transform.position); }
                    else if (rotationOffset != Vector3.zero && useOnlyRotationOffset) { instance.transform.rotation = Quaternion.Euler(rotationOffset); }
                    else
                    {
                        instance.transform.LookAt(collisionEvents[i].intersection + collisionEvents[i].normal);
                        instance.transform.rotation *= Quaternion.Euler(rotationOffset);
                    }
                    Destroy(instance, DestroyTimeDelay);
                }
            }
            if (DestoyMainEffect == true)
            {
                Destroy(gameObject, DestroyTimeDelay + 0.5f);
            }
        }
    }
}