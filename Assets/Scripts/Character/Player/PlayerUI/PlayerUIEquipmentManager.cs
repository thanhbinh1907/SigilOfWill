using UnityEngine;
using UnityEngine.EventSystems;

namespace SG
{
    public class PlayerUIEquipmentManager : MonoBehaviour
    {
        [Header("Equipment Windows")]
        [SerializeField] private GameObject equipmentWindowGameObject; // Kéo thả Panel Trang bị vào đây

        [Header("Default Selected Slot")]
        [SerializeField] private GameObject firstSelectedWeaponSlot; // Kéo thả ô vũ khí Tay Phải 01 vào đây

        // Hàm mở màn hình trang bị khi bấm nút từ Menu Tổng
        public void OpenEquipmentWindow()
        {
            if (equipmentWindowGameObject == null) return;

            // Kích hoạt bật hiển thị Panel
            equipmentWindowGameObject.SetActive(true);

            // THAO TÁC QUAN TRỌNG: Ép hệ thống Unity UI tự động chọn và bôi sáng ô vũ khí đầu tiên
            if (firstSelectedWeaponSlot != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedWeaponSlot);
            }
        }

        public void CloseEquipmentWindow()
        {
            if (equipmentWindowGameObject != null)
            {
                equipmentWindowGameObject.SetActive(false);
            }
        }
    }
}
