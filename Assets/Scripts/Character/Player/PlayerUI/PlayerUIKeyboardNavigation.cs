using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SG
{
    public class PlayerUIKeyboardNavigation : MonoBehaviour
    {
        [SerializeField] private Button[] buttons;
        private int currentIndex = 0;

        private void OnEnable()
        {
            currentIndex = 0;
            StartCoroutine(SelectButtonDelayed(currentIndex));
        }

        private System.Collections.IEnumerator SelectButtonDelayed(int index)
        {
            yield return null; // Chờ 1 frame để EventSystem thiết lập xong
            SelectButton(index);
        }

        private void Update()
        {
            if (buttons == null || buttons.Length == 0) return;

            // Di chuyển xuống bằng phím Down Arrow hoặc S
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                currentIndex = (currentIndex + 1) % buttons.Length;
                int start = currentIndex;
                // Bỏ qua các nút không tương tác được (interactable = false)
                while (!buttons[currentIndex].interactable || !buttons[currentIndex].gameObject.activeInHierarchy)
                {
                    currentIndex = (currentIndex + 1) % buttons.Length;
                    if (currentIndex == start) break;
                }
                SelectButton(currentIndex);
            }
            // Di chuyển lên bằng phím Up Arrow hoặc W
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
                int start = currentIndex;
                while (!buttons[currentIndex].interactable || !buttons[currentIndex].gameObject.activeInHierarchy)
                {
                    currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
                    if (currentIndex == start) break;
                }
                SelectButton(currentIndex);
            }

            // Nhấn Space hoặc Enter để click chọn
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (buttons[currentIndex] != null && buttons[currentIndex].interactable && buttons[currentIndex].gameObject.activeInHierarchy)
                {
                    // Giả lập click chuột
                    buttons[currentIndex].onClick.Invoke();
                }
            }
        }

        private void SelectButton(int index)
        {
            if (index >= 0 && index < buttons.Length && buttons[index] != null)
            {
                buttons[index].Select();
                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
                    Debug.Log($"[KeyboardNavigation] Đã di chuyển chọn nút index {index}: {buttons[index].name}. Đối tượng được chọn hiện tại trên EventSystem: {EventSystem.current.currentSelectedGameObject?.name}");
                }
                else
                {
                    Debug.LogWarning("[KeyboardNavigation] Scene hiện tại thiếu Object EventSystem!");
                }
            }
        }
    }
}
