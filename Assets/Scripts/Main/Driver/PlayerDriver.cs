using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RoguePG.Main.Sprite3D;
using SAE.RoguePG.Main.BattleDriver;

namespace SAE.RoguePG.Main.Driver
{
    /// <summary>
    ///     Makes Players work.
    /// </summary>
    [RequireComponent(typeof(PlayerBattleDriver))]
    [DisallowMultipleComponent]
    public class PlayerDriver : BaseDriver
    {
        /// <summary>
        ///     The <seealso cref="CameraController"/> whose <seealso cref="Camera"/> needs to follow the leading <see cref="PlayerDriver"/>.
        ///     Is set to the one of <see cref="MainManager.MainCamera"/>.
        /// </summary>
        private CameraController mainCameraController;

        /// <summary>
        ///     Calculates and returns the top-down movement vector.
        ///     The axes are mapped X: X, Y: Z.
        /// </summary>
        /// <returns>Target Top-Down Movement Vector</returns>
        protected override Vector2 GetLeaderMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 rawMovement = MainManager.MainCamera.transform.forward * vertical + MainManager.MainCamera.transform.right * horizontal;
            return new Vector2(rawMovement.x, rawMovement.z).normalized;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> whether it is or is not active.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> when it first becomes active
        /// </summary>
        protected override void Start()
        {
            base.Start();
            this.mainCameraController = MainManager.MainCamera.GetComponent<CameraController>();
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="BaseDriver"/>
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.leader == null && this.mainCameraController.following != this.spriteManager.rootTransform)
            {
                this.mainCameraController.following = this.spriteManager.rootTransform;
            }
        }
    }
}
