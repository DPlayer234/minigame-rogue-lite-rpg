namespace SAE.RoguePG.Main.Driver
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.Sprite3D;
    using SAE.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Makes Players work.
    /// </summary>
    [RequireComponent(typeof(PlayerBattleDriver))]
    [DisallowMultipleComponent]
    public class PlayerDriver : BaseDriver
    {
        /// <summary>
        ///     The current player party
        /// </summary>
        public static List<GameObject> Party { get; set; }

        /// <summary>
        ///     Whether this player driver is a recruit
        /// </summary>
        public bool IsRecruit { get; set; }

        /// <summary> Returns whether this <see cref="PlayerDriver"/> is the leader </summary>
        public override bool IsLeader { get { return base.IsLeader && !this.IsRecruit; } }

        /// <summary>
        ///     Calculates and returns the top-down movement vector.
        ///     The axes are mapped X: X, Y: Z.
        /// </summary>
        /// <returns>Target Top-Down Movement Vector</returns>
        protected override Vector2 GetLeaderMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 rawMovement =
                MainManager.CameraController.transform.forward * vertical +
                MainManager.CameraController.transform.right * horizontal;
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
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="BaseDriver"/>
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.IsLeader && MainManager.CameraController.following != this.spriteManager.rootTransform)
            {
                MainManager.CameraController.following = this.spriteManager.rootTransform;
            }
        }
    }
}
