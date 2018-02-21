//-----------------------------------------------------------------------
// <copyright file="BaseDriver.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.Driver
{
    using System.Collections;
    using DPlay.RoguePG.Extension;
    using DPlay.RoguePG.Main.BattleDriver;
    using DPlay.RoguePG.Main.Sprite3D;
    using UnityEngine;

    /// <summary>
    ///     Makes Entities work.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SpriteManager))]
    [RequireComponent(typeof(SpriteAnimator))]
    [RequireComponent(typeof(Highlighter))]
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

        /// <summary> The <seealso cref="Highlighter"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public Highlighter highlight;

        /// <summary>
        ///     Movement Speed
        /// </summary>
        public float movementSpeed = 1.0f;

        /// <summary> The <seealso cref="BaseDriver"/> leading the group. </summary>
        protected BaseDriver leader;

        /// <summary> The <seealso cref="BaseDriver"/> this one is directly following. </summary>
        protected BaseDriver following;

        /// <summary> The <seealso cref="Rigidbody"/> also attached to this <seealso cref="GameObject"/> </summary>
        protected new Rigidbody rigidbody;

        /// <summary> Velocity during the last frame </summary>
        protected Vector3 lastVelocity;

        /// <summary> Minimum angle in degrees between movement velocity and forward vector required to flip </summary>
        private const float MinimumFlipAngle = 5.0f;

        /// <summary> Minimum velocity needed to flip </summary>
        private const float MinimumFlipVelocity = 0.1f;

        /// <summary>
        ///     Minimum distance needed to walk towards <see cref="Following"/>
        /// </summary>
        private const float MinimumFollowDistance = 1.0f;

        /// <summary> Whether this entity is moving </summary>
        private bool isMoving = false;

        /// <summary>
        ///     The <seealso cref="BaseDriver"/> leading the group.
        /// </summary>
        public virtual BaseDriver Leader { get { return this.leader; } }

        /// <summary>
        ///     The <seealso cref="BaseDriver"/> this one is directly following.
        /// </summary>
        public virtual BaseDriver Following { get { return this.following; } }

        /// <summary> Returns whether this <see cref="BaseDriver"/> is the leader </summary>
        public virtual bool IsLeader { get { return this.Leader == null && this.Following == null; } }

        /// <summary>
        ///     Generic idle animation! Yay!
        /// </summary>
        /// <param name="informationSetter">Used to set the information</param>
        /// <returns>More time.</returns>
        public IEnumerator IdleAnimation(SpriteAnimator.StatusSetter informationSetter)
        {
            var lowState = new SpriteAnimationStatus(
            // speed
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
            // speed
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
        ///     The walking animation
        /// </summary>
        /// <param name="informationSetter">Used to set the information</param>
        /// <returns>More time.</returns>
        public IEnumerator WalkingAnimation(SpriteAnimator.StatusSetter informationSetter)
        {
            var lowState = new SpriteAnimationStatus(
            // speed
                3.0f,
            // position
                new Vector3(0.0f, -0.025f, 0.0f),
            // rotations > Body, Head, Hat, LeftArm, LeftLeg, RightArm, RightLeg
                1.0f,
                -3.5f,
                -5.5f,
                -70.0f,
                60.0f,
                70.0f,
                -60.0f);

            var highState = new SpriteAnimationStatus(
            // speed
                3.0f,
            // position
                new Vector3(0.0f, 0.025f, 0.0f),
            // rotations > Body, Head, Hat, LeftArm, LeftLeg, RightArm, RightLeg
                -1.0f,
                3.5f,
                5.5f,
                70.0f,
                -60.0f,
                -70.0f,
                60.0f);

            while (true)
            {
                informationSetter(lowState);

                yield return new WaitForSeconds(1f / 3f);

                informationSetter(highState);

                yield return new WaitForSeconds(1f / 3f);
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
            this.highlight = this.GetComponent<Highlighter>();
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
            if (GameOverHandler.IsGameOver)
            {
                this.rigidbody.velocity = Vector3.zero;
            }

            BaseDriver following;

            Vector2 movement = (
                // Leading and not following
                this.IsLeader ?
                this.GetLeaderMovement() :

                // Following and not impeding personal space
                (following = this.Following) != null && (following.transform.position - this.transform.position).sqrMagnitude > MinimumFollowDistance ?
                new Vector2(
                    following.transform.position.x - this.transform.position.x,
                    following.transform.position.z - this.transform.position.z) :

                // Not walking
                Vector2.zero).normalized * this.movementSpeed;

            Vector3 velocity = this.rigidbody.velocity;

            this.rigidbody.velocity = new Vector3(
                MathExtension.ExponentialLerp(velocity.x, movement.x, 0.01f, Time.deltaTime),
                velocity.y,
                MathExtension.ExponentialLerp(velocity.z, movement.y, 0.01f, Time.deltaTime));

            // Update animation
            bool isMoving = this.rigidbody.velocity.magnitude > this.movementSpeed * 0.5f;
            if (this.isMoving != isMoving)
            {
                if (isMoving)
                {
                    this.spriteAnimator.Animation = this.WalkingAnimation;
                }
                else
                {
                    this.spriteAnimator.Animation = this.IdleAnimation;
                }

                this.isMoving = isMoving;
            }

            // Make sure the SpriteManager is looking in the correct direction (left/right)
            Vector3 currentVelocity = this.rigidbody.velocity;

            if (currentVelocity.sqrMagnitude > MinimumFlipVelocity * MinimumFlipVelocity)
            {
                if (currentVelocity.x != 0 || currentVelocity.z != 0)
                {
                    this.lastVelocity = currentVelocity;
                    this.lastVelocity.y = 0.0f;
                }

                Vector3 facingVector = this.spriteManager.rootTransform.forward;
                facingVector.y = 0.0f;

                float angle = Vector3.SignedAngle(this.lastVelocity, facingVector, new Vector3(0.0f, 1.0f, 0.0f));

                if (angle < -BaseDriver.MinimumFlipAngle && angle > BaseDriver.MinimumFlipAngle - 180.0f)
                {
                    this.spriteManager.FlipToDirection(true);
                }
                else if (angle > BaseDriver.MinimumFlipAngle && angle < 180.0f - BaseDriver.MinimumFlipAngle)
                {
                    this.spriteManager.FlipToDirection(false);
                }
            }
        }

        /// <summary>
        ///     Called by Unity when this behaviour is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (this.spriteAnimator.Animation == null) return;

            this.isMoving = false;
            this.spriteAnimator.Animation = this.IdleAnimation;
        }
    }
}
