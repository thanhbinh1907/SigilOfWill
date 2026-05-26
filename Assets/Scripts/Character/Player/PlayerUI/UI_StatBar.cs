using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SG
{
	public class UI_StatBar : MonoBehaviour
	{
		protected Slider slider;
		protected RectTransform rectTransform;
		// variable to scale the slider value to the actual stat value

		[Header("Bar Options")]
		[SerializeField] protected bool scaleBarLengthWithStats = true;
		[SerializeField] protected float widthScaleMultiplier = 1;

		protected virtual void Awake()
		{
			CheckSlider();
		}

		protected void CheckSlider()
		{
			if (slider == null)
			{
				slider = GetComponent<Slider>();
				if (slider == null)
				{
					slider = GetComponentInChildren<Slider>();
				}
			}

			if (rectTransform == null)
			{
				rectTransform = GetComponent<RectTransform>();
			}
		}

		public virtual void SetStat(float newValue)
		{
			CheckSlider();
			if (slider != null)
			{
				slider.value = newValue;
			}
		}

		public virtual void SetMaxStat(float maxValue)
		{
			CheckSlider();
			if (slider != null)
			{
				slider.maxValue = maxValue;
				slider.value = maxValue;
			}

			if (scaleBarLengthWithStats && rectTransform != null)
			{
				// SCALE THE TRANSFORM OF THIS OBJECT
				rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
				// RESET THE POSITION OF THE BARS BASE ON THEIR LAYOUT GROUP'S SETTINGS
				PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
			}
		}
	}
}