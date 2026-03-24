using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG 
{
	public class CharacterManager : MonoBehaviour
	{
		public CharacterController characterController;
		protected virtual void Awake()
		{
			DontDestroyOnLoad(this);

			characterController = GetComponent<CharacterController>();
		}

		protected virtual void Update()
		{

		}
	}
}
