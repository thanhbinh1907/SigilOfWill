using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class PlayerUICharacterMenuManager : MonoBehaviour
    {
        [Header("Menu Window")]
        [SerializeField] GameObject menu;

        public bool IsOpen => menu != null && menu.activeSelf;

        public void OpenCharacterMenu()
        {
            PlayerUIManager.instance.SetMainMenuActive(true);
            menu.SetActive(true);
        }

        public void CloseCharacterMenu()
        {
			PlayerUIManager.instance.SetMainMenuActive(false);
			menu.SetActive(false);
		}

        public void CloseCharacterMenuAfterFixedFrame()
        {
            StartCoroutine(WaitThenCloseMenu());
		}

        private IEnumerator WaitThenCloseMenu()
        {
            yield return new WaitForFixedUpdate();

			PlayerUIManager.instance.SetMainMenuActive(false);
			menu.SetActive(false);
		}

        public void ReturnToMainMenu()
        {

            if (WorldSaveGameManager.instance != null)
            {
                WorldSaveGameManager.instance.SaveGame();
            }


            CloseCharacterMenu();
            if (PlayerUIManager.instance != null)
            {
                PlayerUIManager.instance.menuWindowIsOpen = false;
                PlayerUIManager.instance.popupWindowIsOpen = false;
            }


            if (PlayerManager.instance != null) Destroy(PlayerManager.instance.gameObject);
            if (PlayerCamera.instance != null) Destroy(PlayerCamera.instance.gameObject);
            if (PlayerInputManager.instance != null) Destroy(PlayerInputManager.instance.gameObject);


            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
