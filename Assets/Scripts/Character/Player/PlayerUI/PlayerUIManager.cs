using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG { 
	public class PlayerUIManager : MonoBehaviour
	{
		public static PlayerUIManager instance;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this; 
			}
			else
			{
				Destroy(gameObject); 
			}
			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}
	} 
	
}