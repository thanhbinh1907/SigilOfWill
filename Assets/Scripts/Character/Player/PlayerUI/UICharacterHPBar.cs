using UnityEngine;
using TMPro;

namespace SG
{
    public class UICharacterHPBar : UI_StatBar
    {
        private CharacterManager character;

        [Header("Name Settings")]
        [SerializeField] bool displayCharacterNameOnDamage = false;
        [SerializeField] TextMeshProUGUI characterNameText;

        [Header("Damage Settings")]
        [SerializeField] TextMeshProUGUI characterDamageText;
        [SerializeField] int currentDamageTaken = 0;
        public int oldHealthValue = 0;

        [Header("Visibility Timer")]
        [SerializeField] float defaultTimeBeforeBarHides = 3f;
        [SerializeField] float hideTimer = 0f;

        protected override void Awake()
        {
            base.Awake();
            // Truy tìm ngược thực thể nhân vật cha sở hữu cụm UI này
            character = GetComponentInParent<CharacterManager>();
        }

        protected void Start()
        {
            // Mặc định ẩn thanh máu này đi khi vừa load game
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            // Khi thanh máu bị ẩn, reset lượng sát thương tích lũy về 0 cho lần đụng độ kế tiếp
            currentDamageTaken = 0;
        }

        private void Update()
        {
            // Hiệu ứng Billboard: Ép thanh máu luôn xoay mặt đối diện thẳng với Camera chính
            if (Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.forward);
            }

            // Bộ đếm thời gian tự động ẩn thanh máu
            if (hideTimer > 0)
            {
                hideTimer -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        // ĐÈ LOGIC CẬP NHẬT CHỈ SỐ MÁU OFFLINE SINGLE-PLAYER:
        public void SetCharacterStat(int newValue)
        {
            if (character == null) return;

            // Đảm bảo slider đã được khởi tạo
            CheckSlider();

            if (slider != null)
            {
                // Đồng bộ lại max value của thanh cuộn phòng trường hợp thực thể được buff lượng máu Max
                slider.maxValue = character.maxHealth;
                // Cập nhật chỉ số thanh slider đỏ tụt tương ứng
                slider.value = newValue;
            }

            // Tính toán khoảng cách chênh lệch để cộng dồn sát thương lũy tiến
            int damageDelta = oldHealthValue - newValue;
            currentDamageTaken += damageDelta;

            // Cập nhật văn bản chỉ số sát thương hiển thị
            if (characterDamageText != null)
            {
                if (currentDamageTaken < 0) // Trường hợp được hồi máu
                {
                    characterDamageText.color = Color.green;
                    characterDamageText.text = "+" + Mathf.Abs(currentDamageTaken).ToString();
                }
                else // Trường hợp bị trúng đòn sát thương
                {
                    characterDamageText.color = Color.red;
                    characterDamageText.text = "-" + currentDamageTaken.ToString();
                }
            }

            // Cập nhật hiển thị danh xưng văn bản nếu được cấu hình bật ngoài Inspector
            if (displayCharacterNameOnDamage && characterNameText != null)
            {
                characterNameText.gameObject.SetActive(true);
                characterNameText.text = character.characterName; // Lấy trực tiếp tên nhân vật từ characterName
            }

            // Kích hoạt bật hiển thị đối tượng UI và nạp lại thời gian chờ 3 giây
            hideTimer = defaultTimeBeforeBarHides;
            gameObject.SetActive(true);
        }
    }
}
