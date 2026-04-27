using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
			}
		}

		private void HandleFollowTarget()
		{
			Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
			transform.position = targetCameraPosition;
		}

		private void HandleRotation()
		{
			// if locked on, rotation towards enemy
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
	}
}