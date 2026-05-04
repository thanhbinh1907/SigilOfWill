using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace SG { 
	public class PlayerCamera : MonoBehaviour
	{
		public static PlayerCamera instance;
		public PlayerManager player;
		public Camera cameraObject;
		[SerializeField] Transform cameraPivotTransform;

		[Header("Camera Settings")]
		private float cameraSmoothSpeed = 1;
		[SerializeField] float leftAndRightRotationSpeed = 220; 
		[SerializeField] float upAndDownRotationSpeed = 220;
		[SerializeField] float minimumPivot = -30;                     // lowest point the camera can look down to
		[SerializeField] float maximumPivot = 60; 
		[SerializeField] float cameraCollisionRadius = 0.2f;
		[SerializeField] LayerMask collisionWithLayers;

		[Header("Camera Values")]
		private Vector3 cameraVelocity;
		private Vector3 cameraObjectPosition;                          // used for camera collision (move camera to this position if there is a collision)
		[SerializeField] float leftAndRightLookAngle;			
		[SerializeField] float upAndDownLookAngle; 
		private float cameraZPosition;                                 // used for camera collision 
		private float targetCameraZPosition;                           // used for camera collision

		[Header("Lock On Settings")]
		[SerializeField] float lockOnRadius = 20;
		[SerializeField] float maximumLockOnDistance = 20f;
		[SerializeField] float minimumViewableAngle = -50f;
		[SerializeField] float maximumViewableAngle = 50f;
		[SerializeField] float lockOnRotationSpeed = 2f;
		[SerializeField] float unlockedCameraHeight = 1.75f;
		[SerializeField] float lockedCameraHeight = 2f;
		[SerializeField] float lockOnLostTimer = 0;
		[SerializeField] float maxLockOnLostTime = 2f;


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
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
			cameraZPosition = cameraObject.transform.localPosition.z;
		}

		public void HandleAllCameraActions()
		{
			if (player != null)
			{
				HandleFollowTarget();
				HandleRotation();
				HandleCollision();
				HandleCameraHeight();
			}
		}

		private void HandleFollowTarget()
		{
			Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
			transform.position = targetCameraPosition;
		}

		private void HandleRotation()
		{
			if (player.isLockOn && player.playerCombatManager.currentTarget != null)
			{
				bool canSeeTarget = !Physics.Linecast(player.lockOnTransform.position,
													player.playerCombatManager.currentTarget.lockOnTransform.position,
													WorldUtilityManager.instance.GetEnvironmentLayers());

				if (!canSeeTarget)
				{
					lockOnLostTimer += Time.deltaTime;

					if (lockOnLostTimer > maxLockOnLostTime)
					{
						player.isLockOn = false;
						player.playerCombatManager.currentTarget = null;
						lockOnLostTimer = 0;
						return;
					}
				}
				else
				{
					lockOnLostTimer = 0;
				}

				Vector3 targetDirection = player.playerCombatManager.currentTarget.lockOnTransform.position - transform.position;
				targetDirection.Normalize();
				targetDirection.y = 0;

				Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnRotationSpeed * Time.deltaTime);

				targetDirection = player.playerCombatManager.currentTarget.lockOnTransform.position - cameraPivotTransform.position;
				targetDirection.Normalize();

				Quaternion pivotRotation = Quaternion.LookRotation(targetDirection);
				cameraPivotTransform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, pivotRotation, lockOnRotationSpeed);

				leftAndRightLookAngle = transform.eulerAngles.y;
				float angle = cameraPivotTransform.localEulerAngles.x;
				if (angle > 180)
				{
					angle -= 360;
				}
				upAndDownLookAngle = angle;
			}

			else
			{
				// else rotate based on camera input
				leftAndRightLookAngle += PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed * Time.deltaTime;
				upAndDownLookAngle -= PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed * Time.deltaTime;
				upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

				Vector3 cameraRotation = Vector3.zero;
				Quaternion targetRotation;

				// rotate this game object left and right
				cameraRotation.y = leftAndRightLookAngle;
				targetRotation = Quaternion.Euler(cameraRotation);
				transform.rotation = targetRotation;

				// rotate this game object up and down
				cameraRotation = Vector3.zero;
				cameraRotation.x = upAndDownLookAngle;
				targetRotation = Quaternion.Euler(cameraRotation);
				cameraPivotTransform.localRotation = targetRotation;
			}
		}

		private void HandleCameraHeight()
		{
			float targetHeight = unlockedCameraHeight;

			if (player.isLockOn)
			{
				targetHeight = lockedCameraHeight;
			}

			Vector3 newHeight = new Vector3(0, targetHeight, 0);
			cameraPivotTransform.localPosition = Vector3.Lerp(cameraPivotTransform.localPosition, newHeight, 0.5f);
		}

		private void HandleCollision()
		{
			targetCameraZPosition = cameraZPosition;
			RaycastHit hit;

			// direction from the camera pivot to the camera
			Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
			direction.Normalize();

			// check if there is an object between the camera pivot and the camera
			if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collisionWithLayers))
			{
				// if there is a collision, set the target camera z position to the hit point minus the collision radius
				float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
				targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
			}

			// if the target camera z position is too close to the pivot, set it to the minimum distance (collision radius)
			if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
			{
				targetCameraZPosition = -cameraCollisionRadius;
			}

			// smoothly move the camera to the target z position
			cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
			cameraObject.transform.localPosition = cameraObjectPosition;
		}

		public void HandleLockOnTargets()
		{
			CharacterManager closestTarget = null;
			float shortestDistance = Mathf.Infinity;

			Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());
			for (int i=0; i < colliders.Length; i++)
			{
				CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();
				
				if (lockOnTarget != null)
				{
					float distanceFromPlayer = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
					
					Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
					float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

					if (lockOnTarget.isDead ||
						lockOnTarget.transform.root == player.transform.root ||
						distanceFromPlayer > maximumLockOnDistance || 
						viewableAngle < minimumViewableAngle || viewableAngle > maximumViewableAngle)
					{
						continue;
					}

					if (Physics.Linecast(player.lockOnTransform.position,
										 lockOnTarget.lockOnTransform.position,
										 WorldUtilityManager.instance.GetEnvironmentLayers()))
					{
						continue;
					}

					if (distanceFromPlayer < shortestDistance)
					{
						shortestDistance = distanceFromPlayer;
						closestTarget = lockOnTarget;
					}
				}
			}
			if (closestTarget != null)
			{
				player.playerCombatManager.currentTarget = closestTarget;
				player.isLockOn = true;
			}
		}

		// ===================== Test & Debug ===================== // 
		private void OnDrawGizmos()
		{
			if (player == null) return;

			// 1. Vẽ vòng tròn bán kính Lock-on (Màu xanh dương)
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(player.transform.position, lockOnRadius);

			// 2. Nếu đang Lock-on, vẽ đường thẳng nối tới mục tiêu (Màu đỏ)
			if (player.isLockOn && player.playerCombatManager.currentTarget != null)
			{
				Gizmos.color = Color.red;
				// Vẽ tia nối giữa 2 điểm Lock-on để kiểm tra Linecast
				Gizmos.DrawLine(player.lockOnTransform.position,
								player.playerCombatManager.currentTarget.lockOnTransform.position);

				// Vẽ một khối cầu nhỏ tại điểm đang bị khóa
				Gizmos.DrawWireSphere(player.playerCombatManager.currentTarget.lockOnTransform.position, 0.5f);
			}
		}
	}
}