using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RougePG.Main.Sprite3D;

namespace SAE.RougePG.Main.Driver
{
    /// <summary>
    ///     Makes Players work.
    ///     Will automatically tag the attached GameObject as "PlayerEntity".
    /// </summary>
    [RequireComponent(typeof(EntityDriver))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SpriteAnimator))]
    public class PlayerDriver : MonoBehaviour
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

        /// <summary>
        ///     The <seealso cref="CameraController"/> whose <seealso cref="Camera"/> needs to follow the leading <see cref="PlayerDriver"/>.
        ///     Is set to the one of <see cref="StateManager.MainCamera"/>.
        /// </summary>
        private CameraController mainCameraController;

        /// <summary> The <seealso cref="SpriteAnimator"/> also attached to this <seealso cref="GameObject"/> </summary>
        private SpriteAnimator spriteAnimator;

        /// <summary> The <seealso cref="EntityDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        private EntityDriver entityDriver;

        /// <summary> The <seealso cref="Rigidbody"/> also attached to this <seealso cref="GameObject"/> </summary>
        new private Rigidbody rigidbody;

        /// <summary>
        ///     Minimum distance needed to walk towards <see cref="following"/>
        /// </summary>
        private const float MinimumFollowDistance = 1.0f;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.tag = "PlayerEntity";

            this.spriteAnimator = this.GetComponent<SpriteAnimator>();
            this.entityDriver = this.GetComponent<EntityDriver>();
            this.rigidbody = this.GetComponent<Rigidbody>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            this.mainCameraController = StateManager.MainCamera.GetComponent<CameraController>();
        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="PlayerDriver"/>
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="EntityDriver"/>
        /// </summary>
        private void FixedUpdate()
        {
            Vector3 movement;

            movement = (
                // Leading and not following
                this.leader == null && this.following == null ?
                new Vector3(
                    Input.GetAxisRaw("Horizontal"),
                    0.0f,
                    Input.GetAxisRaw("Vertical")) :

                // Following and not impeding personal space
                this.following != null && (this.following.transform.position - this.transform.position).sqrMagnitude > MinimumFollowDistance ?
                new Vector3(
                    this.following.transform.position.x - this.transform.position.x,
                    0.0f,
                    this.following.transform.position.z - this.transform.position.z) :
                
                // Not walking
                Vector3.zero).normalized * movementSpeed;

            movement.y = this.rigidbody.velocity.y;

            this.rigidbody.velocity = VariousCommon.ExponentialLerp(this.rigidbody.velocity, movement, 0.01f, Time.deltaTime);

            if (this.leader == null && this.mainCameraController.following != this.gameObject)
            {
                this.mainCameraController.following = this.gameObject;
            }
        }
    }
}
