using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SG
{
	public class UI_StatBar : MonoBehaviour
	{
		private Slider slider;
		// variable to scale the slider value to the actual stat value

		protected virtual void Awake()
		{
			if (slider == null)
			{
				slider = GetComponent<Slider>();
			}
		}

		public virtual void SetStat(float newValue)
		{
			slider.value = newValue;
		}

		public virtual void SetMaxStat(float maxValue)
		{
			slider.maxValue = maxValue;
			slider.value = maxValue;
		}
	}
}