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
		[HideInInspector] public CharacterSoundFXManager characterSoundFXManager;

		[Header("Flags")]
		public bool isPerformingAction = false;
		public bool isJumping = false;
		public bool isGrounded = true;
		public bool applyRootMotion = false;
		public bool canRotate = true;
		public bool canMove = true;
		public bool isSprinting = false;
		public bool isCasting = false;

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

		// ----------------------- MANA ------------------------ //
		public int maxMana = 0;
		public event Action<float, float> OnManaChanged;
		public float _currentMana = 0;
		public float currentMana
		{
			get { return _currentMana; }
			set
			{
				float oldValue = _currentMana;
				_currentMana = value;
				OnManaChanged?.Invoke(oldValue,_currentMana);
			}
		}


		[Header("Stats")]
		public int vitality = 10;
		public int endurance = 10;
		public int intelligence = 10;

		protected virtual void Awake()
		{
			DontDestroyOnLoad(this);

			characterController = GetComponent<CharacterController>();
			animator = GetComponent<Animator>();
			characterEffectsManager = GetComponent<CharacterEffectsManager>();
			characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
			characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
		}

		protected virtual void Start()
		{
			IgnoreMyOwnColliders();
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

		protected virtual void IgnoreMyOwnColliders()
		{
			Collider characterControllerCollider = GetComponent<Collider>();
			Collider[] damagealbeColliders = GetComponentsInChildren<Collider>();

			List<Collider> ignoreColliders = new List<Collider>();

			// ADD ALL OF OUR DAMAGEABLE COLLIDERS TO THE IGNORE LIST, SO WE DON'T DAMAGE OURSELVES
			foreach (var collider in damagealbeColliders)
			{
				ignoreColliders.Add(collider);
			}

			// ADD OUR CHARACTER CONTROLLER COLLIDER TO THE IGNORE LIST, SO WE DON'T DAMAGE OURSELVES
			ignoreColliders.Add(characterControllerCollider);

			// GOES THROUGH EVERY COLLIDER ON THE LIST, AND IGNORES COLLISION WITH EACH OTHER

			foreach (var collider in ignoreColliders)
			{
				foreach (var otherCollider in ignoreColliders)
				{
					Physics.IgnoreCollision(collider, otherCollider, true);
				}
			}
		}
	}
}
