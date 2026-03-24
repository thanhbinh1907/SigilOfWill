using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        PlayerLocomotionManager playerLocomotionManager;
        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        }

        protected override void Update()
        {
            base.Update();
            playerLocomotionManager.HandelAllMovement();
        }
    }
}