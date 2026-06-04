using UnityEngine;

namespace SG
{
    public class TeleportPortalInteractable : Interactable
    {
        [Header("Teleport Destination Settings")]
        [Tooltip("The build index of the target scene in Build Settings.")]
        [SerializeField] private int targetSceneIndex = 2;

        [Tooltip("The spawn coordinates where the player will start in the target scene.")]
        [SerializeField] private Vector3 spawnPosition = Vector3.zero;

        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player == null) return;

            if (WorldSaveGameManager.instance != null && !WorldSaveGameManager.instance.isSceneLoading)
            {
                if (WorldSaveGameManager.instance.currentCharacterData != null)
                {
                    // Clean up interaction list first to avoid persistent prompt popups during scene load
                    if (player.playerInteractionManager != null)
                    {
                        player.playerInteractionManager.RemoveInteractionFromList(this);
                    }

                    // Update scene and coordinate settings in current save data
                    var saveData = WorldSaveGameManager.instance.currentCharacterData;
                    saveData.sceneIndex = targetSceneIndex;
                    saveData.xPosition = spawnPosition.x;
                    saveData.yPosition = spawnPosition.y;
                    saveData.zPosition = spawnPosition.z;

                    Debug.Log($"[TELEPORT] Bắt đầu dịch chuyển sang Scene Index: {targetSceneIndex}, Tọa độ: {spawnPosition}");

                    // Prevent inputs and autosaves during scene load
                    WorldSaveGameManager.instance.isSceneLoading = true;

                    // Save progress directly to file (avoiding player coords overwriting it)
                    string saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBaseOnCharacterSlotBeingUsed(WorldSaveGameManager.instance.currentCharacterSlotBeingUsed);
                    SaveFileDataWriter saveFileDataWriter = new SaveFileDataWriter();
                    saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
                    saveFileDataWriter.saveFileName = saveFileName;
                    saveFileDataWriter.CreateNewCharacterSaveFile(saveData);

                    // Load the target scene asynchronously
                    WorldSaveGameManager.instance.StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
                }
            }
        }
    }
}
