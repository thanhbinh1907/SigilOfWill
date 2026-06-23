using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

    public class FogWallInteractable : Interactable
    {
        public enum DirectionAxis
        {
            Forward,
            Backward,
            Right,
            Left
        }

        [Header("Fog Wall Visuals")]
        [SerializeField] GameObject[] fogGameObjects;

        [Header("Collision Settings (Offline Context)")]
        [SerializeField] private Collider fogWallCollider;

        [Header("Fog Wall ID")]
        public int fogWallID = 0;

        [Header("Active")]
        public bool _isActive = true;

        [Header("Direction Settings (Offline Setup)")]
        [SerializeField] private DirectionAxis enterDirection = DirectionAxis.Right;

        [Header("Movement Settings (Offline Setup)")]
        [SerializeField] private float passThroughSpeed = 1.5f;
        [SerializeField] private float passThroughDuration = 3.0f;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveStatusChanged(_isActive);
            }
        }

        protected override void Awake()
        {
            base.Awake();


            if (fogWallCollider == null)
            {
                Collider[] colliders = GetComponentsInChildren<Collider>();
                foreach (var col in colliders)
                {
                    if (!col.isTrigger)
                    {
                        fogWallCollider = col;
                        break;
                    }
                }
            }
        }

        public void Start()
        {
            if (WorldObjectManager.instance != null)
            {
                WorldObjectManager.instance.AddFogWallToList(this);
            }


            if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
            {
                var saveData = WorldSaveGameManager.instance.currentCharacterData;
                if (saveData.bossesDefeated.ContainsKey(fogWallID))
                {
                    if (saveData.bossesDefeated[fogWallID])
                    {
                        _isActive = false;
                    }
                }
            }

            OnIsActiveStatusChanged(_isActive);
        }

        private void OnDestroy()
        {
            if (WorldObjectManager.instance != null)
            {
                WorldObjectManager.instance.RemoveFogWallFromList(this);
            }
        }

        private void OnIsActiveStatusChanged(bool isActive)
        {
            foreach (var fogObject in fogGameObjects)
            {
                if (fogObject != null)
                {
                    fogObject.SetActive(isActive);
                }
            }

            if (fogWallCollider != null)
            {
                fogWallCollider.enabled = isActive;
            }

            if (interactableCollider != null)
            {
                interactableCollider.enabled = isActive;
            }
        }

        // =================================================================================

        // =================================================================================
        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player == null || player.playerAnimatorManager == null) return;


            Vector3 targetDir = transform.forward;
            switch (enterDirection)
            {
                case DirectionAxis.Forward:
                    targetDir = transform.forward;
                    break;
                case DirectionAxis.Backward:
                    targetDir = -transform.forward;
                    break;
                case DirectionAxis.Right:
                    targetDir = transform.right;
                    break;
                case DirectionAxis.Left:
                    targetDir = -transform.right;
                    break;
            }


            targetDir.y = 0;
            targetDir.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            player.transform.rotation = targetRotation;


            player.playerAnimatorManager.PlayTargetAnimation("Pass_Through_Fog_01", true, false);


            if (player.playerStatsManager != null)
            {
                // player.playerStatsManager.isInvulnerable = true;
            }


            StartCoroutine(DisableCollisionsAndMovePlayer(player, targetDir));
        }


        private IEnumerator DisableCollisionsAndMovePlayer(PlayerManager player, Vector3 moveDirection)
        {
            if (player.characterController != null)
            {

                Collider[] fogColliders = GetComponentsInChildren<Collider>();
                foreach (var col in fogColliders)
                {
                    if (col != null)
                    {
                        Physics.IgnoreCollision(player.characterController, col, true);
                    }
                }

                float elapsed = 0f;


                while (elapsed < passThroughDuration)
                {
                    elapsed += Time.deltaTime;


                    Vector3 moveVelocity = moveDirection * passThroughSpeed;
                    moveVelocity.y = 0;

                    player.characterController.Move(moveVelocity * Time.deltaTime);

                    yield return null;
                }


                foreach (var col in fogColliders)
                {
                    if (col != null)
                    {
                        Physics.IgnoreCollision(player.characterController, col, false);
                    }
                }
            }
        }
    }
}
