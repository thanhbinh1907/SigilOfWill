using UnityEngine;

namespace SG
{
    public class PlayerUIToggleHUD : MonoBehaviour
    {
        private void OnEnable()
        {
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerHUDManager != null)
                PlayerUIManager.instance.playerHUDManager.ToggleHUD(false);
        }

        private void OnDisable()
        {
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerHUDManager != null)
                PlayerUIManager.instance.playerHUDManager.ToggleHUD(true);
        }
    }
}
