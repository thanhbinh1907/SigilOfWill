using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SG
{
    public class TitleScreenLoadMenuInputManager : MonoBehaviour
    {
        PlayerControl playerControls;

		[Header("Title Screen Inputs")]
		[SerializeField] bool deleteCharacterSlot = false;

		private void Update()
		{
			if (deleteCharacterSlot)
			{
				deleteCharacterSlot	= false;
				TitleScreenManager.instance.AttemptToDeleteCharacterSlot();
			}
		}

		private void OnEnable()
		{
			if (playerControls == null)
			{
				playerControls = new PlayerControl();
				playerControls.UI.DeleteCharacterSlot.performed += i => deleteCharacterSlot = true;
			}
			playerControls.Enable();
		}

		private void OnDisable()
		{
			playerControls.Disable();
		}
	}
}