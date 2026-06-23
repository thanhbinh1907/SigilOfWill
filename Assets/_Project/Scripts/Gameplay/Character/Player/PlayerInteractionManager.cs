using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    public class PlayerInteractionManager : MonoBehaviour
    {
        private PlayerManager player;

        [Header("Interaction Queue")]

        private List<Interactable> currentInteractableActions;

        private void Awake()
        {
            player = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            currentInteractableActions = new List<Interactable>();
        }

        public void Update()
        {

            if (currentInteractableActions == null || currentInteractableActions.Count == 0) return;


            if (PlayerUIManager.instance != null &&
               (PlayerUIManager.instance.menuWindowIsOpen ||
                (PlayerUIManager.instance.playerUIPopUpManager != null && PlayerUIManager.instance.playerUIPopUpManager.IsItemPopupActive())))
            {
                return;
            }


            CheckForInteractable();
        }

        private void CheckForInteractable()
        {
            if (currentInteractableActions.Count == 0) return;


            if (currentInteractableActions[0] == null)
            {
                currentInteractableActions.RemoveAt(0);


                if (currentInteractableActions.Count == 0)
                {
                    if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
                    {
                        PlayerUIManager.instance.playerUIPopUpManager.ClosePlayerMessagePopup();
                    }
                }
                return;
            }


            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopup(currentInteractableActions[0].interactableText);
            }
        }

        public void Interact()
        {
            if (currentInteractableActions == null || currentInteractableActions.Count == 0) return;

            if (currentInteractableActions[0] != null)
            {

                currentInteractableActions[0].Interact(player);


                RefreshInteractionList();
            }
        }

        public void AddInteractionToList(Interactable interactableObject)
        {
            if (currentInteractableActions == null)
            {
                currentInteractableActions = new List<Interactable>();
            }

            if (!currentInteractableActions.Contains(interactableObject))
            {
                currentInteractableActions.Add(interactableObject);
            }
        }

        public void RemoveInteractionFromList(Interactable interactableObject)
        {
            if (currentInteractableActions != null && currentInteractableActions.Contains(interactableObject))
            {
                currentInteractableActions.Remove(interactableObject);
            }
            RefreshInteractionList();
        }

        private void RefreshInteractionList()
        {
            if (currentInteractableActions == null) return;


            for (int i = currentInteractableActions.Count - 1; i >= 0; i--)
            {
                if (currentInteractableActions[i] == null)
                {
                    currentInteractableActions.RemoveAt(i);
                }
            }


            if (currentInteractableActions.Count == 0)
            {
                if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
                {
                    PlayerUIManager.instance.playerUIPopUpManager.ClosePlayerMessagePopup();
                }
            }
        }
    }
}
