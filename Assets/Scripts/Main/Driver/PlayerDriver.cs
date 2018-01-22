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
        ///     Player Movement Speed
        /// </summary>
        public float movementSpeed = 1.5f;

        /// <summary>
        ///     The <seealso cref="PlayerDriver"/> leading the group (Main Player).
        ///     If 'null', this is the leader.
        /// </summary>
        public PlayerDriver leader;

        /// <summary>
        ///     The <seealso cref="PlayerDriver"/> this one is directly following.
        /// </summary>
        public PlayerDriver following;

        /// <summary> Current Health value. If less than 0: Full health. </summary>
        [HideInInspector]
        public int currentHealth = -1;

        /// <summary>
        ///     The <seealso cref="CameraController"/> whose <seealso cref="Camera"/> needs to follow the leading <see cref="PlayerDriver"/>.
        ///     Is set to the one of <see cref="MainManager.MainCamera"/>.
        /// </summary>
        private CameraController mainCameraController;

        /// <summary>
        ///     Minimum distance needed to walk towards <see cref="following"/>
        /// </summary>
        private const float MinimumFollowDistance = 1.0f;

        /// <summary> Returns whether this <see cref="PlayerDriver"/> is the leader </summary>
        public bool IsLeader { get { return this.leader == null && this.following == null; } }

        /// <summary>
        ///     Calculates and returns the top-down movement vector.
        ///     The axes are mapped X: X, Y: Z.
        /// </summary>
        /// <returns>Target Top-Down Movement Vector</returns>
        private Vector2 GetMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 rawMovement = MainManager.MainCamera.transform.forward * vertical + MainManager.MainCamera.transform.right * horizontal;
            return new Vector2(rawMovement.x, rawMovement.z).normalized;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> whether it is or is not active.
        /// </summary>
        new private void Awake()
        {
            base.Awake();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> when it first becomes active
        /// </summary>
        new private void Start()
        {
            base.Start();
            this.mainCameraController = MainManager.MainCamera.GetComponent<CameraController>();
        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="PlayerDriver"/>
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="BaseDriver"/>
        /// </summary>
        new private void FixedUpdate()
        {
            Vector2 movement = (
                // Leading and not following
                this.IsLeader ?
                this.GetMovementInput() :

                // Following and not impeding personal space
                this.following != null && (this.following.transform.position - this.transform.position).sqrMagnitude > MinimumFollowDistance ?
                new Vector2(
                    this.following.transform.position.x - this.transform.position.x,
                    this.following.transform.position.z - this.transform.position.z) :
                
                // Not walking
                Vector2.zero).normalized * this.movementSpeed;

            Vector3 velocity = this.rigidbody.velocity;

            this.rigidbody.velocity = new Vector3(
                VariousCommon.ExponentialLerp(velocity.x, movement.x, 0.01f, Time.deltaTime),
                velocity.y,
                VariousCommon.ExponentialLerp(velocity.z, movement.y, 0.01f, Time.deltaTime));

            if (this.leader == null && this.mainCameraController.following != this.spriteManager.rootTransform)
            {
                this.mainCameraController.following = this.spriteManager.rootTransform;
            }

            base.FixedUpdate();
        }
    }
}
