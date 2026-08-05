using UnityEngine;

namespace SG
{
    public class CharacterUIManager : MonoBehaviour
    {
        [Header("UI References (Offline Context)")]
        public bool hasFloatingHPBar = true;
        [SerializeField] private UICharacterHPBar characterHPBar;

        private void Awake()
        {

            if (characterHPBar == null)
            {
                characterHPBar = GetComponentInChildren<UICharacterHPBar>();
            }
        }


        public void OnCharacterHPChanged(int oldValue, int newValue)
        {
            if (!hasFloatingHPBar || characterHPBar == null) return;


            characterHPBar.oldHealthValue = oldValue;


            characterHPBar.SetCharacterStat(newValue);
        }
    }
}
