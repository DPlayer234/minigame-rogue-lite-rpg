using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG.Main
{
    /// <summary>
    ///     Allows for animating sprites using a <see cref="SpriteManager3D"/>
    /// </summary>
    public class SpriteAnimator3D : MonoBehaviour
    {
        /// <summary> The current animation information </summary>
        public SpriteAnimationInformation3D information;

        /// <summary> The <seealso cref="SpriteManager3D"/> also attached to this <seealso cref="GameObject"/> </summary>
        private SpriteManager3D spriteManager;

        /// <summary> The active animation coroutine </summary>
        private Coroutine animationCoroutine;

        /// <summary> [ Use Field <see cref="Animation"/> ] </summary>
        new private SpriteAnimation animation;

        /// <summary> Set or get the Animation delegate </summary>
        public SpriteAnimation Animation
        {
            set
            {
                if (this.animationCoroutine != null)
                {
                    StopCoroutine(this.animationCoroutine);
                }

                if (value != null)
                {
                    this.animationCoroutine = StartCoroutine(value(this));
                }

                this.animation = value;
            }

            get
            {
                return this.animation;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="SpriteAnimator3D"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.spriteManager = this.GetComponent<SpriteManager3D>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="SpriteAnimator3D"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            // Might need this later
        }

        /// <summary>
        ///     Called by Unity to update the current animation, if any.
        /// </summary>
        private void Update()
        {
            if (this.information != null)
            {
#if UNITY_EDITOR
                if (this.information.rotations.Length + 1 != this.spriteManager.animatedTransforms.Length)
                    Debug.LogWarning("The Amount of Sprites and Rotations does not match up.");
#endif
                float deltaTime = Time.deltaTime;
                float @base = 1.0f - this.information.importance;

                // Move Body
                this.spriteManager.bodyTransform.localPosition =
                    VariousCommon.ExponentialLerp(
                        this.spriteManager.bodyTransform.localPosition,
                        this.information.position,
                        @base,
                        deltaTime);

                // Rotate Absolutely Everything Else
                int length = Mathf.Min(this.information.rotations.Length + 1, this.spriteManager.animatedTransforms.Length);
                for (int i = 1; i < length; i++)
                {
                    this.spriteManager.animatedTransforms[i].localEulerAngles =
                        VariousCommon.ExponentialLerpEuler(
                            this.spriteManager.animatedTransforms[i].localEulerAngles,
                            this.information.rotations[i - 1],
                            @base,
                            deltaTime);
                }
            }
        }

        /// <summary>
        ///     Allows animating a sprite by replacing the referenced <seealso cref="SpriteAnimationInformation3D"/>.
        ///     It's run as a <see cref="Coroutine"/>, just in case the return type didn't make that obvious enough.
        /// </summary>
        public delegate IEnumerator SpriteAnimation(SpriteAnimator3D spriteAnimator);
    }
}