using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    // ĐỔI KẾ THỪA: Chuyển từ MonoBehaviour sang kế thừa lớp tương tác cơ sở 'Interactable' của tập trước
    public class FogWallInteractable : Interactable
    {
        public enum DirectionAxis
        {
            Forward,
            Backward,
            Right,
            Left
        }

        [Header("Fog Wall Visuals")]
        [SerializeField] GameObject[] fogGameObjects;

        [Header("Collision Settings (Offline Context)")]
        [SerializeField] private Collider fogWallCollider; // Không cần gán nữa, code tự động tìm tất cả Collider ở con

        [Header("Fog Wall ID")]
        public int fogWallID = 0;

        [Header("Active")]
        public bool _isActive = true;

        [Header("Direction Settings (Offline Setup)")]
        [SerializeField] private DirectionAxis enterDirection = DirectionAxis.Right; // Trục hướng vào phòng boss (Trục đỏ trong hình là Right)

        [Header("Movement Settings (Offline Setup)")]
        [SerializeField] private float passThroughSpeed = 1.5f; // Tốc độ di chuyển đi xuyên qua sương mù (mét/giây)
        [SerializeField] private float passThroughDuration = 3.0f; // Thời gian thực hiện việc đi xuyên (giây)

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveStatusChanged(_isActive);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            // Tìm kiếm khối Collider chặn đường chính nếu chưa gán
            if (fogWallCollider == null)
            {
                Collider[] colliders = GetComponentsInChildren<Collider>();
                foreach (var col in colliders)
                {
                    if (!col.isTrigger)
                    {
                        fogWallCollider = col;
                        break;
                    }
                }
            }
        }

        public void Start()
        {
            if (WorldObjectManager.instance != null)
            {
                WorldObjectManager.instance.AddFogWallToList(this);
            }

            // Kiểm tra xem Boss tương ứng với Fog Wall này đã bị tiêu diệt trong file save chưa
            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                var saveData = WorldSaveGameManager.instance.currentCharacterData;
                if (saveData.bossesDefeated.ContainsKey(fogWallID))
                {
                    if (saveData.bossesDefeated[fogWallID])
                    {
                        _isActive = false; // Tắt sương mù ban đầu nếu boss đã bị tiêu diệt
                    }
                }
            }

            OnIsActiveStatusChanged(_isActive);
        }

        private void OnDestroy()
        {
            if (WorldObjectManager.instance != null)
            {
                WorldObjectManager.instance.RemoveFogWallFromList(this);
            }
        }

        private void OnIsActiveStatusChanged(bool isActive)
        {
            foreach (var fogObject in fogGameObjects)
            {
                if (fogObject != null)
                {
                    fogObject.SetActive(isActive);
                }
            }
        }

        // =================================================================================
        // GHI ĐÈ PHƯƠNG THỨC TƯƠNG TÁC XUYÊN SƯƠNG MÙ CHUẨN OFFLINE SINGLE-PLAYER (TẬP 53)
        // =================================================================================
        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player == null || player.playerAnimatorManager == null) return;

            // BƯỚC 1: Xoay nhân vật hướng trực diện vuông góc vào màn sương theo trục được chọn
            Vector3 targetDir = transform.forward;
            switch (enterDirection)
            {
                case DirectionAxis.Forward:
                    targetDir = transform.forward;
                    break;
                case DirectionAxis.Backward:
                    targetDir = -transform.forward;
                    break;
                case DirectionAxis.Right:
                    targetDir = transform.right;
                    break;
                case DirectionAxis.Left:
                    targetDir = -transform.right;
                    break;
            }

            // Khử bỏ độ lệch trục Y để nhân vật di chuyển thẳng trên mặt đất
            targetDir.y = 0;
            targetDir.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            player.transform.rotation = targetRotation;

            // BƯỚC 2: Phát hoạt ảnh đi xuyên sương mù (Tắt Root Motion để Code tự di chuyển bằng CharacterController)
            player.playerAnimatorManager.PlayTargetAnimation("Pass_Through_Fog_01", true, false);

            // BƯỚC 3: Kích hoạt trạng thái bất tử nội bộ của nhân vật khi đang diễn hoạt ảnh để chặn sát thương từ bên ngoài
            if (player.playerStatsManager != null)
            {
                // player.playerStatsManager.isInvulnerable = true;
            }

            // BƯỚC 4: Khởi chạy Coroutine tự động di chuyển nhân vật xuyên sương mù
            StartCoroutine(DisableCollisionsAndMovePlayer(player, targetDir));
        }

        // COROUTINE NGẮT VA CHẠM VÀ DI CHUYỂN NHÂN VẬT XUYÊN QUA CỬA
        private IEnumerator DisableCollisionsAndMovePlayer(PlayerManager player, Vector3 moveDirection)
        {
            if (player.characterController != null)
            {
                // 1. Tắt va chạm giữa người chơi và TẤT CẢ Collider của màn sương (tránh bị kẹt)
                Collider[] fogColliders = GetComponentsInChildren<Collider>();
                foreach (var col in fogColliders)
                {
                    if (col != null)
                    {
                        Physics.IgnoreCollision(player.characterController, col, true);
                    }
                }

                float elapsed = 0f;

                // 2. Di chuyển nhân vật đi thẳng xuyên qua cửa bằng CharacterController.Move
                while (elapsed < passThroughDuration)
                {
                    elapsed += Time.deltaTime;

                    // Chỉ di chuyển theo hướng ngang (X-Z), bỏ qua Y để hệ thống trọng lực gốc tự xử lý
                    Vector3 moveVelocity = moveDirection * passThroughSpeed;
                    moveVelocity.y = 0;

                    player.characterController.Move(moveVelocity * Time.deltaTime);

                    yield return null;
                }

                // 3. Bật lại va chạm bình thường sau khi kết thúc thời gian xuyên qua
                foreach (var col in fogColliders)
                {
                    if (col != null)
                    {
                        Physics.IgnoreCollision(player.characterController, col, false);
                    }
                }
            }
        }
    }
}
