using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RoguePG.Main.Sprite3D;
using SAE.RoguePG.Main.BattleDriver;

namespace SAE.RoguePG.Main.Driver
{
    /// <summary>
    ///     Makes Entities work.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SpriteManager))]
    [RequireComponent(typeof(SpriteAnimator))]
    [DisallowMultipleComponent]
    public abstract class BaseDriver : MonoBehaviour
    {
        /// <summary> The <seealso cref="SpriteManager"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public SpriteManager spriteManager;

        /// <summary> The <seealso cref="SpriteAnimator"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public SpriteAnimator spriteAnimator;

        /// <summary> The <seealso cref="BaseBattleDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public BaseBattleDriver battleDriver;

        /// <summary>
        ///     Movement Speed
        /// </summary>
        public float movementSpeed = 1.0f;

        /// <summary>
        ///     The <seealso cref="BaseDriver"/> leading the group.
        /// </summary>
        public BaseDriver leader;

        /// <summary>
        ///     The <seealso cref="BaseDriver"/> this one is directly following.
        /// </summary>
        public BaseDriver following;

        /// <summary> The <seealso cref="Rigidbody"/> also attached to this <seealso cref="GameObject"/> </summary>
        new protected Rigidbody rigidbody;

        /// <summary> Velocity during the last frame </summary>
        protected Vector3 lastVelocity;

        /// <summary> Minimum angle in degrees between movement velocity and forward vector required to flip </summary>
        private const float MinimumFlipAngle = 5.0f;

        /// <summary> Minimum velocity needed to flip </summary>
        private const float MinimumFlipVelocity = 0.1f;

        /// <summary>
        ///     Minimum distance needed to walk towards <see cref="following"/>
        /// </summary>
        private const float MinimumFollowDistance = 1.0f;

        /// <summary> Returns whether this <see cref="BaseDriver"/> is the leader </summary>
        public bool IsLeader { get { return this.leader == null && this.following == null; } }

        /// <summary>
        ///     Generic idle animation! Yay!
        /// </summary>
        /// <param name="informationSetter">Used to set the information</param>
        /// <returns>More time.</returns>
        public IEnumerator IdleAnimation(SpriteAnimator.StatusSetter informationSetter)
        {
            var lowState = new SpriteAnimationStatus(
                // importance
                0.8f,
                // position
                new Vector3(0.0f, -0.025f, 0.0f),
                // rotations > Body, Head, Hat, LeftArm, LeftLeg, RightArm, RightLeg
                1.0f,
                -3.5f,
                -5.5f,
                20.0f,
                10.0f,
                -20.0f,
                -10.0f);

            var highState = new SpriteAnimationStatus(
                // importance
                0.8f,
                // position
                new Vector3(0.0f, 0.025f, 0.0f),
                // rotations > Body, Head, Hat, LeftArm, LeftLeg, RightArm, RightLeg
                -1.0f,
                3.5f,
                5.5f,
                5.0f,
                1.0f,
                20.0f,
                -1.0f);

            while (true)
            {
                informationSetter(lowState);

                yield return new WaitForSeconds(1.0f);

                informationSetter(highState);

                yield return new WaitForSeconds(1.0f);
            }
        }

        /// <summary>
        ///     Calculates and returns the top-down movement vector.
        ///     The axes are mapped X: X, Y: Z.
        /// </summary>
        /// <returns>Target Top-Down Movement Vector</returns>
        protected abstract Vector2 GetLeaderMovement();

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="BaseDriver"/> whether it is or is not active.
        /// </summary>
        protected virtual void Awake()
        {
            this.spriteManager = this.GetComponent<SpriteManager>();
            this.spriteAnimator = this.GetComponent<SpriteAnimator>();
            this.battleDriver = this.GetComponent<BaseBattleDriver>();
            this.rigidbody = this.GetComponent<Rigidbody>();

            this.lastVelocity = this.transform.forward;

            this.battleDriver.enabled = false;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="BaseDriver"/> when it first becomes active
        /// </summary>
        protected virtual void Start()
        {
            this.spriteAnimator.Animation = this.IdleAnimation;
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="BaseDriver"/>
        /// </summary>
        protected virtual void FixedUpdate()
        {
            Vector2 movement = (
                // Leading and not following
                this.IsLeader ?
                this.GetLeaderMovement() :

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

            // Make sure the SpriteManager is looking in the correct direction (left/right)
            Vector3 currentVelocity = this.rigidbody.velocity;

            if (currentVelocity.sqrMagnitude > MinimumFlipVelocity * MinimumFlipVelocity)
            {
                if (currentVelocity.x != 0 || currentVelocity.z != 0)
                {
                    this.lastVelocity = currentVelocity;
                    this.lastVelocity.y = 0.0f;
                }

                Vector3 facingVector = spriteManager.rootTransform.forward;
                facingVector.y = 0.0f;

                float angle = Vector3.SignedAngle(this.lastVelocity, facingVector, new Vector3(0.0f, 1.0f, 0.0f));

                if (angle < -MinimumFlipAngle && angle > MinimumFlipAngle - 180.0f)
                {
                    spriteManager.FlipToDirection(true);
                }
                else if (angle > MinimumFlipAngle && angle < 180.0f - MinimumFlipAngle)
                {
                    spriteManager.FlipToDirection(false);
                }
            }
        }
    }
}
