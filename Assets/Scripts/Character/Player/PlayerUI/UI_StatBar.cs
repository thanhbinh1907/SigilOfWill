using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SG
{
	public class UI_StatBar : MonoBehaviour
	{
		private Slider slider;
		private RectTransform rectTransform;
		// variable to scale the slider value to the actual stat value

		[Header("Bar Options")]
		[SerializeField] protected bool scaleBarLengthWithStats = true;
		[SerializeField] protected float widthScaleMultiplier = 1;

		protected virtual void Awake()
		{
			if (slider == null)
			{
				slider = GetComponent<Slider>();
			}
			rectTransform = GetComponent<RectTransform>();
		}

		public virtual void SetStat(float newValue)
		{
			slider.value = newValue;
		}

		public virtual void SetMaxStat(float maxValue)
		{
			slider.maxValue = maxValue;
			slider.value = maxValue;

			if (scaleBarLengthWithStats)
			{
				// SCALE THE TRANSFORM OF THIS OBJECT
				rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
				// RESET THE POSITION OF THE BARS BASE ON THEIR LAYOUT GROUP'S SETTINGS
				PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
			}
		}
	}
}