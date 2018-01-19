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
    public class EntityDriver : MonoBehaviour
    {
        /// <summary> The <seealso cref="SpriteManager"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public SpriteManager spriteManager;

        /// <summary> The <seealso cref="SpriteAnimator"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public SpriteAnimator spriteAnimator;

        /// <summary> The <seealso cref="EntityBattleDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public EntityBattleDriver battleDriver;

        /// <summary> The <seealso cref="Rigidbody"/> also attached to this <seealso cref="GameObject"/> </summary>
        new protected Rigidbody rigidbody;

        /// <summary> Velocity during the last frame </summary>
        protected Vector3 lastVelocity;

        /// <summary> Minimum angle in degrees between movement velocity and forward vector required to flip </summary>
        private const float MinimumFlipAngle = 5.0f;

        /// <summary> Minimum velocity needed to flip </summary>
        private const float MinimumFlipVelocity = 0.1f;

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
                new Vector3(0.0f, 0.0f, -3.5f),
                new Vector3(0.0f, 0.0f, -5.5f),
                new Vector3(0.0f, 0.0f, 20.0f),
                new Vector3(0.0f, 0.0f, 10.0f),
                new Vector3(0.0f, 0.0f, -20.0f),
                new Vector3(0.0f, 0.0f, -10.0f));

            var highState = new SpriteAnimationStatus(
                // importance
                0.8f,
                // position
                new Vector3(0.0f, 0.025f, 0.0f),
                // rotations > Body, Head, Hat, LeftArm, LeftLeg, RightArm, RightLeg
                new Vector3(0.0f, 0.0f, 3.5f),
                new Vector3(0.0f, 0.0f, 5.5f),
                new Vector3(0.0f, 0.0f, 5.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 20.0f),
                new Vector3(0.0f, 0.0f, -1.0f));

            while (true)
            {
                informationSetter(lowState);

                yield return new WaitForSeconds(1.0f);

                informationSetter(highState);

                yield return new WaitForSeconds(1.0f);
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EntityDriver"/> whether it is or is not active.
        /// </summary>
        protected void Awake()
        {
            this.spriteManager = this.GetComponent<SpriteManager>();
            this.spriteAnimator = this.GetComponent<SpriteAnimator>();
            this.battleDriver = this.GetComponent<EntityBattleDriver>();
            this.rigidbody = this.GetComponent<Rigidbody>();

            this.lastVelocity = this.transform.forward;

            this.battleDriver.enabled = false;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EntityDriver"/> when it first becomes active
        /// </summary>
        protected void Start()
        {
            this.spriteAnimator.Animation = this.IdleAnimation;
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="EntityDriver"/>
        /// </summary>
        protected void FixedUpdate()
        {
            // Make sure the SpriteManager is looking in the correct direction
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
