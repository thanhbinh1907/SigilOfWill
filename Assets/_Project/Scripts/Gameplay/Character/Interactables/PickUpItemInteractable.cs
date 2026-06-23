using UnityEngine;

namespace SG
{
    public class PickUpItemInteractable : Interactable
    {
        [Header("Item Pickup Settings")]
        public ItemPickupType pickupType = ItemPickupType.WorldSpawn;
        [SerializeField] private Item itemResource;

        [Header("World Spawn ID Data")]
        public int itemResourceID = 0;
        [SerializeField] private bool hasBeenLooted = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {

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

                    if (!saveData.worldItemsLooted.ContainsKey(itemResourceID))
                    {
                        saveData.worldItemsLooted.Add(itemResourceID, false);
                    }
                    else
                    {

                        hasBeenLooted = saveData.worldItemsLooted[itemResourceID];
                    }
                }


                if (hasBeenLooted)
                {
                    gameObject.SetActive(false);
                }
            }
        }


        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player == null || itemResource == null) return;


            if (player.characterSoundFXManager != null && WorldSoundFXManager.instance != null)
            {
                player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickupItemSFX);
            }


            if (player.playerInventoryManager != null)
            {
                player.playerInventoryManager.AddItemToInventory(itemResource);
            }


            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendItemPopup(itemResource, 1);
            }


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


            if (player.playerInteractionManager != null)
            {
                player.playerInteractionManager.RemoveInteractionFromList(this);
            }


            Destroy(gameObject);
            Debug.Log($"[NHẶT ĐỒ] Người chơi đã nhặt thành công vật phẩm ID {itemResourceID}: {itemResource.itemName}");
        }
    }
}
