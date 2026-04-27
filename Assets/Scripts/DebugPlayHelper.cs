using UnityEngine;
using SG;

namespace SG
{
	public class DebugPlayHelper : MonoBehaviour
	{
		void Start()
		{
			// 1. Kích hoạt Input Manager (vì bình thường nó đợi chuyển cảnh mới bật)
			if (PlayerInputManager.instance != null)
			{
				PlayerInputManager.instance.enabled = true;
			}

			// 2. Khóa chuột ngay lập tức
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			// 3. Cấp Stamina cho Player (vì logic cũ bị kẹt nếu không có UI)
			PlayerManager player = FindFirstObjectByType<PlayerManager>();
			if (player != null)
			{
				// Khởi tạo chỉ số cơ bản để nhấn được Shift (Dodge/Sprint)
				player.maxStamina = 100;
				player.currentStamina = 100;
			}
		}
	}
}