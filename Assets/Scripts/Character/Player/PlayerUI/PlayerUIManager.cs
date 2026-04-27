using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG { 
	public class PlayerUIManager : MonoBehaviour
	{
		public static PlayerUIManager instance;
		[HideInInspector] public PlayerUIHudManager playerUIHudManager;

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

			playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}
	} 
	
}