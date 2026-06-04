using UnityEngine;

namespace SG
{
    public class PlayerUIToggleHUD : MonoBehaviour
    {
        private void OnEnable()
        {
            PlayerUIManager.instance.playerUIHudManager.ToggleHUD(false);
		}

        private void OnDisable()
        {
            PlayerUIManager.instance.playerUIHudManager.ToggleHUD(true);
        }
    }
}
