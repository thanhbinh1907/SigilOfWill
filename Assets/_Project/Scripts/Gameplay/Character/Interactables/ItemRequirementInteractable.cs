using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    public class ItemRequirementInteractable : Interactable
    {
        [Header("Requirement Settings")]
        [Tooltip("Unique ID for this interactable to save its state in character save data.")]
        public int interactableID = 0;

        [Tooltip("List of items required to activate this interactable.")]
        public List<Item> requiredItems = new List<Item>();

        [Header("Activation Effects")]
        [Tooltip("GameObjects to activate upon successful interaction.")]
        public List<GameObject> objectsToActivate = new List<GameObject>();

        [Tooltip("GameObjects to deactivate upon successful interaction.")]
        public List<GameObject> objectsToDeactivate = new List<GameObject>();

        [Header("Pop-up Messages")]
        [TextArea] public string successMessage = "The portal has been opened!";
        [TextArea] public string failureMessage = "You do not have the required items.";

        [Header("Interaction behavior")]
        [Tooltip("If true, this interactable will disable its collider after successful activation.")]
        public bool disableInteractionOnSuccess = true;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // Sync activation state from the save data when the scene loads
            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                var saveData = WorldSaveGameManager.instance.currentCharacterData;

                if (saveData.activatedInteractables != null)
                {
                    // Initialize key if not present in the dictionary
                    if (!saveData.activatedInteractables.ContainsKey(interactableID))
                    {
                        saveData.activatedInteractables.Add(interactableID, false);
                    }

                    // Apply the activated state to the objects
                    bool isAlreadyActivated = saveData.activatedInteractables[interactableID];
                    if (isAlreadyActivated)
                    {
                        ApplyActivationEffects();

                        if (disableInteractionOnSuccess && interactableCollider != null)
                        {
                            interactableCollider.enabled = false;
                        }
                    }
                }
            }
        }

        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player == null) return;

            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                var saveData = WorldSaveGameManager.instance.currentCharacterData;

                // If already activated, do nothing
                if (saveData.activatedInteractables != null && saveData.activatedInteractables.ContainsKey(interactableID))
                {
                    if (saveData.activatedInteractables[interactableID])
                    {
                        return;
                    }
                }

                // Check if the player possesses all required items
                bool hasAllRequiredItems = CheckPlayerInventory(player);

                if (hasAllRequiredItems)
                {
                    // Update state in save game
                    if (saveData.activatedInteractables != null)
                    {
                        saveData.activatedInteractables[interactableID] = true;
                    }

                    // Apply active and inactive objects states
                    ApplyActivationEffects();

                    // Save game progress
                    WorldSaveGameManager.instance.SaveGame();

                    // Send success message popup to Player UI
                    if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
                    {
                        PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopup(successMessage);
                    }

                    // Remove from interaction queue to clean up prompt UI
                    if (player.playerInteractionManager != null)
                    {
                        player.playerInteractionManager.RemoveInteractionFromList(this);
                    }

                    // Disable interaction collider if configured
                    if (disableInteractionOnSuccess && interactableCollider != null)
                    {
                        interactableCollider.enabled = false;
                    }

                    Debug.Log($"[TƯƠNG TÁC] Đã kích hoạt thành công bục tương tác ID {interactableID}!");
                }
                else
                {
                    // Send failure message popup to Player UI
                    if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
                    {
                        PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopup(failureMessage);
                    }

                    Debug.Log($"[TƯƠNG TÁC] Kích hoạt thất bại bục tương tác ID {interactableID}. Người chơi thiếu vật phẩm yêu cầu!");
                }
            }
        }

        private bool CheckPlayerInventory(PlayerManager player)
        {
            if (player.playerInventoryManager == null || player.playerInventoryManager.itemsInventory == null)
            {
                return false;
            }

            foreach (var requiredItem in requiredItems)
            {
                if (requiredItem == null) continue;

                bool found = false;
                foreach (var inventoryItem in player.playerInventoryManager.itemsInventory)
                {
                    if (inventoryItem != null && inventoryItem.itemID == requiredItem.itemID)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }

        private void ApplyActivationEffects()
        {
            // Activate target objects
            if (objectsToActivate != null)
            {
                foreach (var obj in objectsToActivate)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }
            }

            // Deactivate target objects
            if (objectsToDeactivate != null)
            {
                foreach (var obj in objectsToDeactivate)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }
}
