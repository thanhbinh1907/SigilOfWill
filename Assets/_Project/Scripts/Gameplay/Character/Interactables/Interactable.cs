using UnityEngine;

namespace SG
{
    public class Interactable : MonoBehaviour
    {
        [Header("Interactable Settings (Offline)")]
        public string interactableText; // Dòng chữ prompt hiện lên UI (Ví dụ: "Interact", "Pick Up")
        [SerializeField] protected Collider interactableCollider;

        protected virtual void Awake()
        {
            // Tự động tìm kiếm Collider trên chính đối tượng nếu chưa gán bằng tay
            if (interactableCollider == null)
            {
                interactableCollider = GetComponent<Collider>();
            }
        }

        public virtual void Interact(PlayerManager player)
        {
            // Hàm ảo để các lớp con (Item, Door, FogWall) ghi đè logic chuyên biệt sau này
            Debug.Log("[TƯƠNG TÁC] Đã kích hoạt tương tác gốc cơ sở!");
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            // Kiểm tra thực thể va chạm va chạm xem có phải là nhân vật người chơi chính hay không
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player != null && player.playerInteractionManager != null)
            {
                // Nạp chính thực thể này vào danh sách xếp hàng của người chơi cục bộ
                player.playerInteractionManager.AddInteractionToList(this);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player != null && player.playerInteractionManager != null)
            {
                // Rút thực thể khỏi danh sách xếp hàng khi người chơi quay lưng bước đi xa
                player.playerInteractionManager.RemoveInteractionFromList(this);
            }
        }
    }
}
