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

            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                var saveData = WorldSaveGameManager.instance.currentCharacterData;


                if (saveData.sitesOfGrace != null)
                {
                    if (!saveData.sitesOfGrace.ContainsKey(siteOfGraceID))
                    {
                        saveData.sitesOfGrace.Add(siteOfGraceID, false);
                    }
                    else
                    {

                        isActivated = saveData.sitesOfGrace[siteOfGraceID];
                    }
                }


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


        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player == null) return;


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


        private void RestoreSightOfGrace(PlayerManager player)
        {
            isActivated = true;


            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                if (WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace != null)
                {
                    WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace[siteOfGraceID] = true;
                }


                var saveData = WorldSaveGameManager.instance.currentCharacterData;
                saveData.hasGraceSaved = true;
                saveData.lastGraceSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                saveData.lastGraceXPosition = player.transform.position.x;
                saveData.lastGraceYPosition = player.transform.position.y;
                saveData.lastGraceZPosition = player.transform.position.z;

                WorldSaveGameManager.instance.SaveGame();
            }


            Vector3 targetDirection = transform.position - player.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            if (targetDirection != Vector3.zero)
            {
                player.transform.rotation = Quaternion.LookRotation(targetDirection);
            }


            interactableText = activatedInteractionText;


            if (player.playerAnimatorManager != null)
            {
                player.playerAnimatorManager.PlayTargetAnimation(activateGraceAnimation, true);
            }


            if (activatedParticles != null) activatedParticles.SetActive(true);


            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendGraceRestoredPopUp("GRACE RESTORED");
            }


            if (interactableCollider != null) interactableCollider.enabled = false;


            if (player.playerInteractionManager != null)
            {
                player.playerInteractionManager.RemoveInteractionFromList(this);
            }

            StartCoroutine(WaitForAnimationAndPopupThenRestoreCollider(2f));
        }


        private void RestAtSightOfGrace(PlayerManager player)
        {
            Debug.Log("[TRẠM NGHỈ] Người chơi đang ngồi nghỉ chân tại Trạm!");

            restingPlayer = player;


            Vector3 targetDirection = transform.position - player.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            if (targetDirection != Vector3.zero)
            {
                player.transform.rotation = Quaternion.LookRotation(targetDirection);
            }


            if (player.playerAnimatorManager != null)
            {
                player.playerAnimatorManager.PlayTargetAnimation(sitDownAnimation, true);
            }


            player.currentHealth = player.maxHealth;
            player.currentStamina = player.maxStamina;
            player.currentMana = player.maxMana;


            if (WorldAIManager.instance != null)
            {
                WorldAIManager.instance.ResetAllCharacters();
            }


            interactableText = restingInteractionText;


            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopup(interactableText);
            }


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


        private void StandUpFromSightOfGrace(PlayerManager player)
        {
            Debug.Log("[TRẠM NGHỈ] Người chơi đứng dậy khỏi Trạm!");


            if (player.playerAnimatorManager != null)
            {
                player.playerAnimatorManager.PlayTargetAnimation(standUpAnimation, true);
            }

            restingPlayer = null;


            interactableText = activatedInteractionText;


            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopup(interactableText);
            }
        }

        private IEnumerator WaitForAnimationAndPopupThenRestoreCollider(float delay)
        {

            yield return new WaitForSeconds(delay);


            if (interactableCollider != null) interactableCollider.enabled = true;
        }
    }
}
