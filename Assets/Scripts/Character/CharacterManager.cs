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
		[HideInInspector] public CharacterAnimatorManager characterAnimatorManager;	

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
		public event Action<int, int> OnHealthChanged;
		
		public int _currentHealth = 0;
		public int currentHealth
		{
			get { return _currentHealth; }
			set
			{
				int oldValue = _currentHealth;
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
			characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
		}

		protected virtual void Update()
		{
			animator.SetBool("isGrounded", isGrounded);
			PlayerCamera.instance.HandleAllCameraActions();
		}

		public void CheckHP(int oldValue, int newValue)
		{
			if (isDead)
				return;

			if (currentHealth <= 0)
			{
				StartCoroutine(ProcessDeathEvent());
			}

			if (currentHealth > maxHealth)
			{
				currentHealth = maxHealth;
			}
		}

		public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
		{
			_currentHealth = 0;
			isDead = true;

			// RESET ANY FLAGS HERE THAT NEED TO BE RESET
			// NOTHING YET

			// IF WE ARE NOT GROUNDED,  PLAY AN AERIAL DEATH ANIMATION

			if (!manuallySelectDeathAnimation)
			{
				characterAnimatorManager.PlayTargetAnimation("Dead_01", true);
			}

			// PLAY SOME DEATH SFX

			yield return new WaitForSeconds(5);

			// AWARD PLAYER WITH RUNES

			// DISABLE CHARACTER CONTROLLER
		}

		public virtual void ReviveCharacter()
		{

		}
	}
}
