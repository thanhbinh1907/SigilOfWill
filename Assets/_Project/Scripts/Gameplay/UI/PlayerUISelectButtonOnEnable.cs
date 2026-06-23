using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SG
{
    public class PlayerUISelectButtonOnEnable : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            StartCoroutine(SelectButtonDelayed());
		}

        private System.Collections.IEnumerator SelectButtonDelayed()
        {
            yield return null;
            if (button != null)
            {
                if (EventSystem.current == null)
                {
                    Debug.LogWarning("[SelectButtonOnEnable] KHÔNG thể tự động chọn nút vì Scene hiện tại thiếu Object EventSystem!");
                }
                else
                {
                    button.Select();
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                    Debug.Log($"[SelectButtonOnEnable] Đã thực hiện Select() và SetSelectedGameObject cho: {button.name}. Đối tượng được chọn hiện tại trên EventSystem: {EventSystem.current.currentSelectedGameObject?.name}");
                }
            }
            else
            {
                Debug.LogError("[SelectButtonOnEnable] Không tìm thấy Component Button trên GameObject này!");
            }
        }
    }
}
