using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG.Main
{
    /// <summary>
    ///     Makes Players work.
    /// </summary>
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

        /// <summary> The <seealso cref="SpriteAnimator3D"/> also attached to this <seealso cref="GameObject"/> </summary>
        private SpriteAnimator3D spriteAnimator;

        /// <summary> The <seealso cref="EntityDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        private EntityDriver entityDriver;

        /// <summary> The <seealso cref="Rigidbody"/> also attached to this <seealso cref="GameObject"/> </summary>
        new private Rigidbody rigidbody;

        /// <summary>
        ///     Minimum distance needed to walk towards <see cref="following"/>
        /// </summary>
        private const float MinimumFollowDistance = 1.0f;

        /// <summary> Rotation in RADIANS (not degrees) from the top </summary>
        public float Rotation { set { this.entityDriver.Rotation = value; } get { return this.entityDriver.Rotation; } }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.entityDriver = this.GetComponent<EntityDriver>();
            if (this.entityDriver == null) throw new Exceptions.EntityDriverException("There is no EntityDriver attached to this GameObject. A PlayerDriver requires an EntityDriver.");

            this.spriteAnimator = this.GetComponent<SpriteAnimator3D>();
            if (this.spriteAnimator == null) throw new Exceptions.EntityDriverException("There is no SpriteAnimator3D attached to this GameObject.");

            this.rigidbody = this.GetComponent<Rigidbody>();
            if (this.rigidbody == null) throw new Exceptions.EntityDriverException("There is no Rigidbody attached to this GameObject.");
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            
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
        }
    }
}
