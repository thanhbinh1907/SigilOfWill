using UnityEngine;

namespace SG
{
    public class CharacterUIManager : MonoBehaviour
    {
        [Header("UI References (Offline Context)")]
        public bool hasFloatingHPBar = true; // Cho phép bật/tắt linh hoạt ngoài Inspector cho từng loại quái/Boss
        [SerializeField] private UICharacterHPBar characterHPBar;

        private void Awake()
        {
            // Tự động truy tìm thành phần điều khiển thanh máu ở lớp con nếu chưa gán bằng tay
            if (characterHPBar == null)
            {
                characterHPBar = GetComponentInChildren<UICharacterHPBar>();
            }
        }

        // HÀM TRUNG GIAN ĐÓN NHẬN SỰ KIỆN BIẾN ĐỘNG MÁU CỤC BỘ:
        public void OnCharacterHPChanged(int oldValue, int newValue)
        {
            if (!hasFloatingHPBar || characterHPBar == null) return;

            // Nạp lượng máu cũ để thanh máu nổi làm cơ sở tính toán toán học sát thương lũy tiến
            characterHPBar.oldHealthValue = oldValue;
            
            // Ra lệnh cập nhật giao diện trực quan cục bộ
            characterHPBar.SetCharacterStat(newValue);
        }
    }
}
