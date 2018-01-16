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
        /// <summary> The <seealso cref="SpriteManager3D"/> also attached to this <seealso cref="GameObject"/> </summary>
        private SpriteManager3D spriteManager;

        /// <summary> The <seealso cref="SpriteAnimator3D"/> also attached to this <seealso cref="GameObject"/> </summary>
        private SpriteAnimator3D spriteAnimator;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EntityDriver"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.spriteManager = this.GetComponent<SpriteManager3D>();
            this.spriteAnimator = this.GetComponent<SpriteAnimator3D>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EntityDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            this.spriteAnimator.Animation = IdleAnimation;
        }

        /// <summary>
        ///     Generic idle animation! Yay!
        /// </summary>
        /// <param name="spriteAnimator">The used <seealso cref="SpriteAnimator3D"/></param>
        /// <returns>More time.</returns>
        private IEnumerator IdleAnimation(SpriteAnimator3D spriteAnimator)
        {
            var lowState = new SpriteAnimationInformation3D(
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

            var highState = new SpriteAnimationInformation3D(
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
                spriteAnimator.information = lowState;

                yield return new WaitForSeconds(1.0f);

                spriteAnimator.information = highState;

                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
