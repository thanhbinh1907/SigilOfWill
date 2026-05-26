using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    public class PlayerInteractionManager : MonoBehaviour
    {
        private PlayerManager player;
        
        [Header("Interaction Queue")]
        // Danh sách xếp hàng quản lý đa vật thể va chạm cục bộ Offline
        private List<Interactable> currentInteractableActions;

        private void Awake()
        {
            player = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            currentInteractableActions = new List<Interactable>();
        }

        public void Update()
        {
            // Kiểm tra an toàn: Nếu không có vật thể nào va chạm thì bỏ qua quét UI
            if (currentInteractableActions == null || currentInteractableActions.Count == 0) return;

            // Chặn không cho hiện prompt chữ nếu người chơi đang bận mở bảng Menu hòm đồ/trang bị
            if (PlayerUIManager.instance != null && 
               (PlayerUIManager.instance.menuWindowIsOpen || PlayerUIManager.instance.popupWindowIsOpen))
            {
                return;
            }

            // Gọi hàm quét xử lý hiển thị Text Prompt của vật thể ưu tiên số 1 (Index 0)
            CheckForInteractable();
        }

        private void CheckForInteractable()
        {
            if (currentInteractableActions.Count == 0) return;

            // Nếu phần tử đầu tiên bị null (vật thể bị hủy đột ngột), tiến hành dọn dẹp hàng đợi
            if (currentInteractableActions[0] == null)
            {
                currentInteractableActions.RemoveAt(0);
                return;
            }

            // Đẩy chuỗi văn bản tương tác của đối tượng sang cho UI hiển thị lên màn hình
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopup(currentInteractableActions[0].interactableText);
            }
        }

        public void Interact()
        {
            if (currentInteractableActions == null || currentInteractableActions.Count == 0) return;

            if (currentInteractableActions[0] != null)
            {
                // Kích hoạt hàm tương tác thực tế của thực thể đang xếp đầu tiên
                currentInteractableActions[0].Interact(player);
                
                // Tự động dọn dẹp hệ thống danh sách xếp hàng
                RefreshInteractionList();
            }
        }

        public void AddInteractionToList(Interactable interactableObject)
        {
            if (currentInteractableActions == null)
            {
                currentInteractableActions = new List<Interactable>();
            }

            if (!currentInteractableActions.Contains(interactableObject))
            {
                currentInteractableActions.Add(interactableObject);
            }
        }

        public void RemoveInteractionFromList(Interactable interactableObject)
        {
            if (currentInteractableActions != null && currentInteractableActions.Contains(interactableObject))
            {
                currentInteractableActions.Remove(interactableObject);
            }
            RefreshInteractionList();
        }

        private void RefreshInteractionList()
        {
            if (currentInteractableActions == null) return;

            // Thuật toán vòng lặp ngược chạy lùi dọn dẹp sạch sẽ toàn bộ ô nhớ trống rỗng an toàn
            for (int i = currentInteractableActions.Count - 1; i >= 0; i--)
            {
                if (currentInteractableActions[i] == null)
                {
                    currentInteractableActions.RemoveAt(i);
                }
            }

            // Nếu sau khi dọn dẹp mà danh sách trống hoàn toàn, ra lệnh cho UI ẩn bảng thông báo đi
            if (currentInteractableActions.Count == 0)
            {
                if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
                {
                    PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopupWindows();
                }
            }
        }
    }
}
