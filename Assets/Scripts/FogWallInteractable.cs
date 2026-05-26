using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class FogWallInteractable : MonoBehaviour
    {
        [SerializeField] GameObject[] fogGameObjects;

		[Header("Fog Wall ID")]
		public int fogWallID = 0;

		[Header("Active")]
        public bool _isActive = true;

		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnIsActiveStatusChanged(_isActive);
			}
		}

		public void Start()
		{
			if (WorldObjectManager.instance != null)
			{
				WorldObjectManager.instance.AddFogWallToList(this);
			}

			// Kiểm tra xem Boss tương ứng với Fog Wall này đã bị tiêu diệt trong file save chưa
			if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
			{
				var saveData = WorldSaveGameManager.instance.currentCharacterData;
				if (saveData.bossesDefeated.ContainsKey(fogWallID))
				{
					if (saveData.bossesDefeated[fogWallID])
					{
						_isActive = false; // Tắt sương mù ban đầu nếu boss đã bị tiêu diệt
					}
				}
			}

			OnIsActiveStatusChanged(_isActive);
		}

		private void OnDestroy()
		{
			if (WorldObjectManager.instance != null)
			{
				WorldObjectManager.instance.RemoveFogWallFromList(this);
			}
		}

		private void OnIsActiveStatusChanged(bool isActive)
		{
			foreach (var fogObject in fogGameObjects)
			{
				if (fogObject != null)
				{
					fogObject.SetActive(isActive);
				}
			}
		}
	}
}
