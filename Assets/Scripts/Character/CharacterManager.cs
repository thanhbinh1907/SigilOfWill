using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.AI;

namespace SG 
{
	public class CharacterManager : MonoBehaviour
	{
		[Header("Status")]
		public bool isDead = false;

		[HideInInspector] public CharacterController characterController;
		[HideInInspector] public Animator animator;

		[HideInInspector] public CharacterEffectsManager characterEffectsManager;

		[Header("Flags")]
		public bool isPerformingAction = false;
		public bool isJumping = false;
		public bool isGrounded = true;
		public bool applyRootMotion = false;
		public bool canRotate = true;
		public bool canMove = true;
		public bool isSprinting = false;

		[Header("Resources")]
		// ----------------------- HEALTH ------------------------ //
		public int maxHealth = 0;
		public event Action<float, float> OnHealthChanged;
		
		public float _currentHealth = 0;
		public float currentHealth
		{
			get { return _currentHealth; }
			set
			{
				float oldValue = _currentHealth;
				_currentHealth = value;
				OnHealthChanged?.Invoke(oldValue,_currentHealth);
			}
		}

		// ----------------------- STAMINA ------------------------ //
		public int maxStamina = 0;

		public event Action<float, float> OnStaminaChanged;

		public float _currentStamina = 0;
		public float currentStamina
		{
			get { return _currentStamina; }
			set
			{
				float oldValue = _currentStamina;
				_currentStamina = value;
				OnStaminaChanged?.Invoke(oldValue,_currentStamina);
			}
		}

		[Header("Stats")]
		public int vitality = 10;
		public int endurance = 10;

		protected virtual void Awake()
		{
			DontDestroyOnLoad(this);

			characterController = GetComponent<CharacterController>();
			animator = GetComponent<Animator>();
			characterEffectsManager = GetComponent<CharacterEffectsManager>();
		}

		protected virtual void Update()
		{
			animator.SetBool("isGrounded", isGrounded);
			PlayerCamera.instance.HandleAllCameraActions();
		}

	}
}
