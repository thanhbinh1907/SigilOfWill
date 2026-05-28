using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class PlayerUICharacterMenuManager : MonoBehaviour
    {
        [Header("Menu Window")]
        [SerializeField] GameObject menuWindow;

        [Header("Default Selected Button")]
        [SerializeField] Button defaultSelectedButton;

        public void OpenCharacterMenu()
        {
            PlayerUIManager.instance.menuWindowIsOpen = true;
            menuWindow.SetActive(true);

            if (defaultSelectedButton != null)
            {
                defaultSelectedButton.Select();
            }
        }

        public void CloseCharacterMenu()
        {
            PlayerUIManager.instance.menuWindowIsOpen = false;
            menuWindow.SetActive(false);
        }

        // Hàm wrapper gọi từ nút bấm UI (Step 5)
        public void CloseCharacterMenuAfterFixedUpdate()
        {
            StartCoroutine(WaitThenClose());
        }

        private IEnumerator WaitThenClose()
        {
            yield return new WaitForFixedUpdate(); // Chờ xử lý vật lý/input gameplay frame này chạy xong
            CloseCharacterMenu();
        }
    }
}
