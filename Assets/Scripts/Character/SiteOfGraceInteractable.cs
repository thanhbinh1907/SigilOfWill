using UnityEngine;
using System.Collections;

namespace SG
{
    public class SiteOfGraceInteractable : Interactable
    {
        [Header("Site Of Grace Settings")]
        public int siteOfGraceID = 0;
        public bool isActivated = false;

        [Header("Visual Effects")]
        [SerializeField] private GameObject activatedParticles;

        [Header("Interaction Text Variations")]
        [SerializeField] private string unactivatedInteractionText = "Restore Site of Grace";
        [SerializeField] private string activatedInteractionText = "Rest";
        [SerializeField] private string restingInteractionText = "Stand Up";

        [Header("Animator State Names")]
        [SerializeField] private string activateGraceAnimation = "Activate_Site_Of_Grace_01";
        [SerializeField] private string sitDownAnimation = "Sit_Down_At_Grace";
        [SerializeField] private string standUpAnimation = "Stand_Up_From_Grace";

        [Header("Resting State")]
        private PlayerManager restingPlayer;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // LUỒNG NẠP DỮ LIỆU OFFLINE TỪ RAM SAVE GAME:
            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                var saveData = WorldSaveGameManager.instance.currentCharacterData;

                // Nếu ID trạm nghỉ này chưa tồn tại trong từ điển lưu trữ, tiến hành khởi tạo mặc định là false
                if (saveData.sitesOfGrace != null)
                {
                    if (!saveData.sitesOfGrace.ContainsKey(siteOfGraceID))
                    {
                        saveData.sitesOfGrace.Add(siteOfGraceID, false);
                    }
                    else
                    {
                        // Nếu đã có dữ liệu, nạp trạng thái kích hoạt cục bộ lên biến bool
                        isActivated = saveData.sitesOfGrace[siteOfGraceID];
                    }
                }

                // Cập nhật giao diện trực quan và văn bản gợi ý tương ứng ngay khi load map
                if (isActivated)
                {
                    if (activatedParticles != null) activatedParticles.SetActive(true);
                    interactableText = activatedInteractionText;
                }
                else
                {
                    if (activatedParticles != null) activatedParticles.SetActive(false);
                    interactableText = unactivatedInteractionText;
                }
            }
        }

        // GHI ĐÈ PHƯƠNG THỨC TƯƠNG TÁC GỐC
        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player == null) return;

            // Rẽ nhánh logic chiến đấu dựa vào trạng thái kích hoạt Offline
            if (!isActivated)
            {
                RestoreSightOfGrace(player);
            }
            else
            {
                if (restingPlayer == null)
                {
                    RestAtSightOfGrace(player);
                }
                else
                {
                    StandUpFromSightOfGrace(player);
                }
            }
        }

        // HÀM KÍCH HOẠT TRẠM NGHỈ LẦN ĐẦU TIÊN TRONG ĐỜI
        private void RestoreSightOfGrace(PlayerManager player)
        {
            isActivated = true;

            // Ghi trạng thái kích hoạt true vào file dữ liệu RAM và ra lệnh lưu ổ cứng
            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                if (WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace != null)
                {
                    WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace[siteOfGraceID] = true;
                }

                // Lưu tọa độ Grace ngồi gần nhất
                var saveData = WorldSaveGameManager.instance.currentCharacterData;
                saveData.hasGraceSaved = true;
                saveData.lastGraceSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                saveData.lastGraceXPosition = player.transform.position.x;
                saveData.lastGraceYPosition = player.transform.position.y;
                saveData.lastGraceZPosition = player.transform.position.z;

                WorldSaveGameManager.instance.SaveGame();
            }

            // Ép người chơi xoay người nhìn thẳng vào tâm của Trạm nghỉ chân
            Vector3 targetDirection = transform.position - player.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            if (targetDirection != Vector3.zero)
            {
                player.transform.rotation = Quaternion.LookRotation(targetDirection);
            }

            // Chuyển đổi dòng chữ Prompt gợi ý sang trạng thái đã kích hoạt
            interactableText = activatedInteractionText;

            // Chơi hoạt ảnh quỳ chúc phúc kịch tính điện ảnh
            if (player.playerAnimatorManager != null)
            {
                player.playerAnimatorManager.PlayTargetAnimation(activateGraceAnimation, true);
            }

            // Kích hoạt hiệu ứng bừng sáng vĩnh viễn
            if (activatedParticles != null) activatedParticles.SetActive(true);

            // Gọi hiển thị Pop-up chữ vàng vinh quang chúc mừng lên Canvas UI màn hình
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendGraceRestoredPopUp("GRACE RESTORED");
            }

            // Khóa va chạm tương tác trong 2 giây để tránh tương tác lại khi đang diễn hoạt ảnh
            if (interactableCollider != null) interactableCollider.enabled = false;
            
            // Xóa trạm nghỉ khỏi danh sách tương tác hiện tại của Player ngay lập tức để ẩn UI Prompt cũ
            if (player.playerInteractionManager != null)
            {
                player.playerInteractionManager.RemoveInteractionFromList(this);
            }

            StartCoroutine(WaitForAnimationAndPopupThenRestoreCollider(2f));
        }

        // HÀM NGỒI NGHỈ CHÂN (HỒI MÁU & HỒI SINH QUÁI VẬT TOÀN BẢN ĐỒ)
        private void RestAtSightOfGrace(PlayerManager player)
        {
            Debug.Log("[TRẠM NGHỈ] Người chơi đang ngồi nghỉ chân tại Trạm!");
            
            restingPlayer = player;

            // 1. Ép hướng người chơi xoay mặt thẳng vào tâm của Trạm nghỉ chân
            Vector3 targetDirection = transform.position - player.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            if (targetDirection != Vector3.zero)
            {
                player.transform.rotation = Quaternion.LookRotation(targetDirection);
            }

            // 2. Chơi hoạt ảnh ngồi xuống nghỉ chân
            if (player.playerAnimatorManager != null)
            {
                player.playerAnimatorManager.PlayTargetAnimation(sitDownAnimation, true);
            }

            // 3. HỒI PHỤC CHỈ SỐ CỤC BỘ: Đưa toàn bộ lượng Máu, Thể lực và Mana của Player về mức tối đa cực đại
            player.currentHealth = player.maxHealth;
            player.currentStamina = player.maxStamina;
            player.currentMana = player.maxMana;

            // 4. HỒI SINH QUÁI VẬT (RESPAWN LOGIC): Gọi sang bộ quản lý AI tập trung để dọn dẹp và hồi sinh quái vật loạt mới
            if (WorldAIManager.instance != null)
            {
                WorldAIManager.instance.ResetAllCharacters();
            }

            // 5. Thay đổi chữ Prompt sang "Đứng dậy"
            interactableText = restingInteractionText;

            // Cập nhật lại UI Prompt nếu có
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopup(interactableText);
            }

            // TỰ ĐỘNG LƯU GAME: Thực hiện lưu vị trí và chỉ số nhân vật khi ngồi nghỉ (giống Elden Ring)
            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                var saveData = WorldSaveGameManager.instance.currentCharacterData;
                saveData.hasGraceSaved = true;
                saveData.lastGraceSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                saveData.lastGraceXPosition = player.transform.position.x;
                saveData.lastGraceYPosition = player.transform.position.y;
                saveData.lastGraceZPosition = player.transform.position.z;

                WorldSaveGameManager.instance.SaveGame();
            }
        }

        // HÀM ĐỨNG DẬY KHỎI TRẠM NGHỈ CHÂN
        private void StandUpFromSightOfGrace(PlayerManager player)
        {
            Debug.Log("[TRẠM NGHỈ] Người chơi đứng dậy khỏi Trạm!");

            // 1. Chơi hoạt ảnh đứng dậy
            if (player.playerAnimatorManager != null)
            {
                player.playerAnimatorManager.PlayTargetAnimation(standUpAnimation, true);
            }

            restingPlayer = null;

            // 2. Khôi phục chữ Prompt về "Nghỉ ngơi"
            interactableText = activatedInteractionText;

            // Cập nhật lại UI Prompt nếu có
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopup(interactableText);
            }
        }

        private IEnumerator WaitForAnimationAndPopupThenRestoreCollider(float delay)
        {
            // Chờ khoảng thời gian delay trễ
            yield return new WaitForSeconds(delay);

            // Bật lại vòng va chạm để Prompt chữ "Rest" xuất hiện trở lại trên màn hình
            if (interactableCollider != null) interactableCollider.enabled = true;
        }
    }
}
