using UnityEngine;

namespace SG
{
    public class PickUpItemInteractable : Interactable
    {
        [Header("Item Pickup Settings")]
        public ItemPickupType pickupType = ItemPickupType.WorldSpawn;
        [SerializeField] private Item itemResource; // Gán ScriptableObject thông số vật phẩm vào đây

        [Header("World Spawn ID Data")]
        public int itemResourceID = 0;
        [SerializeField] private bool hasBeenLooted = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // CHỈ XỬ LÝ KIỂM TRA NẾU ĐÂY LÀ VẬT PHẨM ĐẶT CỐ ĐỊNH TRÊN MAP (WORLD SPAWN)
            if (pickupType == ItemPickupType.WorldSpawn)
            {
                CheckIfWorldItemWasAlreadyLooted();
            }
        }

        private void CheckIfWorldItemWasAlreadyLooted()
        {
            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                var saveData = WorldSaveGameManager.instance.currentCharacterData;

                if (saveData.worldItemsLooted != null)
                {
                    // Nếu ID vật phẩm này chưa có tên trong từ điển lưu trữ tiến trình, tiến hành khởi tạo mặc định là false
                    if (!saveData.worldItemsLooted.ContainsKey(itemResourceID))
                    {
                        saveData.worldItemsLooted.Add(itemResourceID, false);
                    }
                    else
                    {
                        // Nạp trạng thái đã nhặt từ ổ cứng RAM lên biến bool cục bộ
                        hasBeenLooted = saveData.worldItemsLooted[itemResourceID];
                    }
                }

                // Nếu dữ liệu báo vật phẩm này đã bị người chơi nhặt từ mạng trước, tự động hủy biến mất ngay khi load game
                if (hasBeenLooted)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        // GHI ĐÈ PHƯƠNG THỨC TƯƠNG TÁC CHÍNH CHUẨN OFFLINE
        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player == null || itemResource == null) return;

            // 1. PHÁT AUDIO: Gọi sound manager phát âm thanh thu thập vật phẩm đặc trưng công khai
            if (player.characterSoundFXManager != null && WorldSoundFXManager.instance != null)
            {
                player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickupItemSFX);
            }

            // 2. NẠP HÀNH LÝ: Đẩy thực thể vật phẩm vào danh sách túi đồ cá nhân của Player
            if (player.playerInventoryManager != null)
            {
                player.playerInventoryManager.AddItemToInventory(itemResource);
            }

            // 3. ĐỒ HỌA HIỂN THỊ: Gọi Pop-up manager vẽ giao diện tên, hình ảnh icon lên màn hình UI (Mặc định số lượng là 1)
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendItemPopup(itemResource, 1);
            }

            // 4. LƯU TIẾN TRÌNH THẾ GIỚI: Nếu là đồ World Spawn, cập nhật trạng thái đã looted và ghi đè lưu game
            if (pickupType == ItemPickupType.WorldSpawn)
            {
                if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
                {
                    if (WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted != null)
                    {
                        WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted[itemResourceID] = true;
                    }
                    WorldSaveGameManager.instance.SaveGame();
                }
            }

            // Xóa vật phẩm khỏi danh sách tương tác của người chơi ngay lập tức để ẩn UI Prompt "Pick Up Item"
            if (player.playerInteractionManager != null)
            {
                player.playerInteractionManager.RemoveInteractionFromList(this);
            }

            // 5. GIẢI PHÓNG VẬT LÝ: Hủy hoàn toàn GameObject vật phẩm lấp lánh khỏi đấu trường map thế giới
            Destroy(gameObject);
            Debug.Log($"[NHẶT ĐỒ] Người chơi đã nhặt thành công vật phẩm ID {itemResourceID}: {itemResource.itemName}");
        }
    }
}
