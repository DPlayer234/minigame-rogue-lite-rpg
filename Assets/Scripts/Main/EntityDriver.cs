using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG.Main
{
    /// <summary>
    ///     Makes Entities work.
    /// </summary>
    public class EntityDriver : MonoBehaviour
    {
        /// <summary> The <seealso cref="SpriteAnimator3D"/> also attached to this <seealso cref="GameObject"/> </summary>
        private SpriteAnimator3D spriteAnimator;

        /// <summary> The <seealso cref="Rigidbody"/> also attached to this <seealso cref="GameObject"/> </summary>
        new private Rigidbody rigidbody;

        /// <summary> Rotation in RADIANS (not degrees) from the top </summary>
        public float Rotation { set; get; }

        /// <summary>
        ///     Generic idle animation! Yay!
        /// </summary>
        /// <param name="informationSetter">Used to set the information</param>
        /// <returns>More time.</returns>
        public IEnumerator IdleAnimation(SpriteAnimator3D.StatusSetter informationSetter)
        {
            var lowState = new SpriteAnimationStatus3D(
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

            var highState = new SpriteAnimationStatus3D(
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
        private void Awake()
        {
            this.spriteAnimator = this.GetComponent<SpriteAnimator3D>();
            if (this.spriteAnimator == null) throw new Exceptions.EntityDriverException("There is no SpriteAnimator3D attached to this GameObject.");

            this.rigidbody = this.GetComponent<Rigidbody>();
            if (this.rigidbody == null) throw new Exceptions.EntityDriverException("There is no Rigidbody attached to this GameObject.");

            this.Rotation = 0.0f;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EntityDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            this.spriteAnimator.Animation = this.IdleAnimation;
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="EntityDriver"/>
        /// </summary>
        private void FixedUpdate()
        {
            
        }
    }
}
