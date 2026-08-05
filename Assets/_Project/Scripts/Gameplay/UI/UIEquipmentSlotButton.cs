using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class UIEquipmentSlotButton : MonoBehaviour
    {
        [Header("Slot Configuration")]
        public EquipmentSlotType slotType;

        private void Start()
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnSlotClicked);
            }
            else
            {
                Debug.LogWarning($">> [UI EQUIPMENT SLOT] GameObject '{gameObject.name}' does not have a Button component!");
            }
        }

        private void OnSlotClicked()
        {
            PlayerUIEquipmentManager equipmentManager = PlayerUIManager.instance.playerUIEquipmentManager;

            // Load the inventory for this slot type and handle selection/un-equipping
            equipmentManager.LoadEquipmenInventory(slotType);
        }
    }
}
