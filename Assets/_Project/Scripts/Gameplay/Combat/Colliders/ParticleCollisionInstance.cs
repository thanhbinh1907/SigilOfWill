/*This script created by using docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
    public class ParticleCollisionInstance : DamageDealer
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

        // Cooldown tránh việc 1 hạt va chạm nhiều lần trong cùng 1 frame gây chết quái lập tức
        private Dictionary<CharacterManager, float> damageCooldowns = new Dictionary<CharacterManager, float>();
        private float hitCooldown = 0.25f; // Giãn cách 0.25 giây giữa các lần trúng sét của mỗi mục tiêu

        void Start()
        {
            part = GetComponent<ParticleSystem>();
            if (part == null)
            {
                part = GetComponentInChildren<ParticleSystem>();
                if (part == null)
                {
                    Debug.LogError($"[ParticleCollisionInstance] Không tìm thấy Component ParticleSystem nào trên {gameObject.name} hoặc các con của nó!");
                }
            }
        }

        void OnParticleCollision(GameObject other)
        {
            if (part == null)
            {
                Debug.LogError($"[ParticleCollisionInstance] Bỏ qua va chạm vì 'part' (ParticleSystem) bị NULL!");
                return;
            }

            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            // Kiểm tra va chạm để trừ máu quái vật
            CharacterManager targetCharacter = other.GetComponentInParent<CharacterManager>();
            if (targetCharacter != null)
            {
                Debug.Log($"[ParticleCollisionInstance] Va chạm hạt xảy ra với nhân vật '{targetCharacter.name}'. Số lượng sự kiện va chạm: {numCollisionEvents}");
                
                if (targetCharacter == characterCausingDamage)
                {
                    Debug.Log($"[ParticleCollisionInstance] Bỏ qua va chạm vì mục tiêu là chính người gây sát thương (Caster).");
                    return;
                }

                if (targetCharacter.isDead)
                {
                    Debug.Log($"[ParticleCollisionInstance] Bỏ qua va chạm vì mục tiêu '{targetCharacter.name}' đã chết.");
                    return;
                }

                bool canDamage = true;
                if (damageCooldowns.ContainsKey(targetCharacter))
                {
                    if (Time.time - damageCooldowns[targetCharacter] < hitCooldown)
                    {
                        canDamage = false;
                        Debug.Log($"[ParticleCollisionInstance] Cooldown chưa hết đối với '{targetCharacter.name}'. Còn lại: {hitCooldown - (Time.time - damageCooldowns[targetCharacter]):F2}s");
                    }
                }

                if (canDamage)
                {
                    if (targetCharacter.isInvulnerable)
                    {
                        Debug.Log($"[ParticleCollisionInstance] Bỏ qua vì mục tiêu '{targetCharacter.name}' đang bất tử.");
                        return;
                    }

                    damageCooldowns[targetCharacter] = Time.time;

                    Vector3 contactPoint = targetCharacter.transform.position;
                    if (collisionEvents.Count > 0)
                    {
                        contactPoint = collisionEvents[0].intersection;
                    }

                    Debug.Log($"[ParticleCollisionInstance] ĐANG GÂY SÁT THƯƠNG lên '{targetCharacter.name}' tại điểm va chạm {contactPoint}. Lôi ApplyDamage...");
                    ApplyDamage(targetCharacter, contactPoint);
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