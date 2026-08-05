using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace SG
{
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("You DIED Pop Up")]
        [SerializeField] GameObject youDiedPopUpGameObject;
        [SerializeField] TextMeshProUGUI youDiedPopUpBackGroundText;
        [SerializeField] TextMeshProUGUI youDiedPopUpText;
        [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;

        [Header("BOSS DEFEATED Pop Up")]
        [SerializeField] GameObject bossDefeatedPopUpGameObject;
        [SerializeField] TextMeshProUGUI bossDefeatedPopUpBackGroundText;
        [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
        [SerializeField] CanvasGroup bossDefeatedPopUpCanvasGroup;

        [Header("GRACE RESTORED Pop Up")]
        [SerializeField] GameObject graceRestoredPopUpGameObject;
        [SerializeField] TextMeshProUGUI graceRestoredPopUpBackgroundText;
        [SerializeField] TextMeshProUGUI graceRestoredPopUpText;
        [SerializeField] CanvasGroup graceRestoredPopUpCanvasGroup; // Allows us to set the alpha to fade over time

        [Header("Demo Completion Pop Up Settings")]
        [SerializeField] GameObject demoCompletionPopUpGameObject;
        [SerializeField] CanvasGroup demoCompletionPopUpCanvasGroup;

        [Header("Player Message Pop Up")]
        [SerializeField] GameObject playerMessagePopUpGameObject;
        [SerializeField] TextMeshProUGUI playerMessageText;

        [Header("Item Loot Popup Settings (Offline Context)")]
        [SerializeField] private GameObject itemPopupGameObject;
        [SerializeField] private UnityEngine.UI.Image itemIconImage;
        [SerializeField] private TMPro.TextMeshProUGUI itemNameText;
        [SerializeField] private TMPro.TextMeshProUGUI itemAmountText;


        public void SendYouDiedPopUp()
        {
            // ACTIVATE POST PROCESSING EFFECT

            youDiedPopUpGameObject.SetActive(true);
            youDiedPopUpBackGroundText.characterSpacing = 0;

            // STRETCH OUT THE POP UP
            StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackGroundText, 8, 19));
            // FADE IN THE POP UP
            StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
            // WAIT, THEN FADE OUT THE POP UP
            StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));
        }

        public void SendBossDefeatedPopUp(string bossDefeatedMessage)
        {
            bossDefeatedPopUpText.text = bossDefeatedMessage;
            bossDefeatedPopUpBackGroundText.text = bossDefeatedMessage;

            bossDefeatedPopUpGameObject.SetActive(true);
            bossDefeatedPopUpBackGroundText.characterSpacing = 0;

            // STRETCH OUT THE POP UP
            StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpBackGroundText, 8, 19));
            // FADE IN THE POP UP
            StartCoroutine(FadeInPopUpOverTime(bossDefeatedPopUpCanvasGroup, 5));
            // WAIT, THEN FADE OUT THE POP UP
            StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedPopUpCanvasGroup, 2, 5));
        }

        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
        {
            if (duration > 0)
            {
                text.characterSpacing = 0;          // RESET OUR CHARACTER SPACING
                float timer = 0;

                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                    yield return null;
                }
            }
        }

        private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
        {
            if (duration > 0)
            {
                canvas.alpha = 0;
                float timer = 0;

                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 1;

            yield return null;
        }

        private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
        {
            if (duration > 0)
            {
                while (delay > 0)
                {
                    delay = delay - Time.deltaTime;
                    yield return null;
                }

                canvas.alpha = 1;
                float timer = 0;

                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 0;

            yield return null;
        }

        public void SendPlayerMessagePopup(string message)
        {
            if (playerMessageText != null)
            {
                playerMessageText.text = message;
            }
            if (playerMessagePopUpGameObject != null)
            {
                playerMessagePopUpGameObject.SetActive(true);
            }
            PlayerUIManager.instance.popupWindowIsOpen = true;
        }

        public void CloseAllPopupWindows()
        {
            if (playerMessagePopUpGameObject != null)
            {
                playerMessagePopUpGameObject.SetActive(false);
            }
            if (itemPopupGameObject != null)
            {
                itemPopupGameObject.SetActive(false);
            }
            PlayerUIManager.instance.popupWindowIsOpen = false;
        }

        public bool IsItemPopupActive()
        {
            return itemPopupGameObject != null && itemPopupGameObject.activeSelf;
        }

        public void ClosePlayerMessagePopup()
        {
            if (playerMessagePopUpGameObject != null)
            {
                playerMessagePopUpGameObject.SetActive(false);
            }


            if (itemPopupGameObject != null && itemPopupGameObject.activeSelf)
            {
                PlayerUIManager.instance.popupWindowIsOpen = true;
            }
            else
            {
                PlayerUIManager.instance.popupWindowIsOpen = false;
            }
        }

        public void SendItemPopup(Item item, int itemAmount)
        {
            if (itemPopupGameObject == null) return;


            PlayerUIManager.instance.popupWindowIsOpen = true;


            if (itemIconImage != null) itemIconImage.sprite = item.itemIcon;
            if (itemNameText != null) itemNameText.text = item.itemName;

            if (itemAmountText != null)
            {
                itemAmountText.gameObject.SetActive(true);
                itemAmountText.text = "x" + itemAmount.ToString();
            }


            itemPopupGameObject.SetActive(true);
        }

        public void SendGraceRestoredPopUp(string graceRestoredMessage)
        {
            if (graceRestoredPopUpText != null)
            {
                graceRestoredPopUpText.text = graceRestoredMessage;
            }
            if (graceRestoredPopUpBackgroundText != null)
            {
                graceRestoredPopUpBackgroundText.text = graceRestoredMessage;
            }
            if (graceRestoredPopUpGameObject != null)
            {
                graceRestoredPopUpGameObject.SetActive(true);
            }
            if (graceRestoredPopUpBackgroundText != null)
            {
                graceRestoredPopUpBackgroundText.characterSpacing = 0;
                StartCoroutine(StretchPopUpTextOverTime(graceRestoredPopUpBackgroundText, 8, 19));
            }
            if (graceRestoredPopUpCanvasGroup != null)
            {
                StartCoroutine(FadeInPopUpOverTime(graceRestoredPopUpCanvasGroup, 5));
                StartCoroutine(WaitThenFadeOutPopUpOverTime(graceRestoredPopUpCanvasGroup, 2, 5));
            }
        }

        public void ReturnToMainMenu()
        {
            // Restore timeScale before returning to Main Menu
            Time.timeScale = 1f;

            CloseDemoCompletionPopup();

            if (PlayerManager.instance != null) Destroy(PlayerManager.instance.gameObject);
            if (PlayerCamera.instance != null) Destroy(PlayerCamera.instance.gameObject);
            if (PlayerInputManager.instance != null) Destroy(PlayerInputManager.instance.gameObject);


            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public void QuitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        public void DisplayDemoCompletionPopup()
        {
            if (demoCompletionPopUpGameObject != null)
            {
                demoCompletionPopUpGameObject.SetActive(true);
            }

            if (demoCompletionPopUpCanvasGroup != null)
            {
                demoCompletionPopUpCanvasGroup.alpha = 1f;
                demoCompletionPopUpCanvasGroup.interactable = true;
                demoCompletionPopUpCanvasGroup.blocksRaycasts = true;
            }


            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;


            if (PlayerInputManager.instance != null)
            {
                PlayerInputManager.instance.enabled = false;
            }


            Time.timeScale = 0f;
        }

        public void CloseDemoCompletionPopup()
        {
            if (demoCompletionPopUpGameObject != null)
            {
                demoCompletionPopUpGameObject.SetActive(false);
            }

            if (demoCompletionPopUpCanvasGroup != null)
            {
                demoCompletionPopUpCanvasGroup.alpha = 0f;
                demoCompletionPopUpCanvasGroup.interactable = false;
                demoCompletionPopUpCanvasGroup.blocksRaycasts = false;
            }
        }
    }
}