using UnityEngine;

namespace SG
{
    public class Interactable : MonoBehaviour
    {
        [Header("Interactable Settings (Offline)")]
        public string interactableText;
        [SerializeField] protected Collider interactableCollider;

        protected virtual void Awake()
        {

            if (interactableCollider == null)
            {
                interactableCollider = GetComponent<Collider>();
            }
        }

        public virtual void Interact(PlayerManager player)
        {

            Debug.Log("[TƯƠNG TÁC] Đã kích hoạt tương tác gốc cơ sở!");
        }

        protected virtual void OnTriggerEnter(Collider other)
        {

            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player != null && player.playerInteractionManager != null)
            {

                player.playerInteractionManager.AddInteractionToList(this);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player != null && player.playerInteractionManager != null)
            {

                player.playerInteractionManager.RemoveInteractionFromList(this);
            }
        }
    }
}
