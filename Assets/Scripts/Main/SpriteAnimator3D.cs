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
        /// <summary> How far the current animation state has progressed </summary>
        private float progress;
        
        /// <summary> The current animation information </summary>
        private SpriteAnimationStatus3D endStatus;
        
        /// <summary> Reference information for interpolation </summary>
        private SpriteAnimationStatus3D startStatus;

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
                    this.animationCoroutine = StartCoroutine(value(this.SetAnimationTarget));
                }

                this.animation = value;
            }

            get
            {
                return this.animation;
            }
        }

        /// <summary>
        ///     Sets the <seealso cref="SpriteAnimationStatus3D"/>.
        /// </summary>
        /// <param name="status">The new status to use</param>
        private void SetAnimationTarget(SpriteAnimationStatus3D status)
        {
            this.progress = 0.0f;
            this.endStatus = status;

            Vector3[] currentRotations = new Vector3[this.spriteManager.animatedTransforms.Length];
            for (int i = 0; i < this.spriteManager.animatedTransforms.Length; i++)
            {
                currentRotations[i] = VariousCommon.WrapAngle(this.spriteManager.animatedTransforms[i].localEulerAngles);
            }

            this.startStatus = new SpriteAnimationStatus3D(
                0.0f,
                this.spriteManager.bodyTransform.localPosition,
                currentRotations);
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
        ///     Called by Unity every frame to update the <see cref="SpriteAnimator3D"/>
        /// </summary>
        private void Update()
        {
            this.UpdateAnimation();
        }

        /// <summary>
        ///     Updates the current animation if there is one.
        /// </summary>
        private void UpdateAnimation()
        {
            if (this.endStatus != null)
            {
                this.progress += Time.deltaTime * this.endStatus.speed;

#if UNITY_EDITOR
                if (this.endStatus.rotations.Length != this.startStatus.rotations.Length)
                    Debug.LogWarning("The Amount of Sprites and Rotations does not match up.");
#endif

                // Move Body
                this.spriteManager.bodyTransform.localPosition =
                    VariousCommon.SmootherStep(this.startStatus.position, this.endStatus.position, this.progress);

                // Rotate Absolutely Everything Else
                int length = Mathf.Min(this.endStatus.rotations.Length, this.startStatus.rotations.Length);
                for (int i = 0; i < length; i++)
                {
                    this.spriteManager.animatedTransforms[i].localEulerAngles =
                        VariousCommon.SmootherStep(this.startStatus.rotations[i], this.endStatus.rotations[i], this.progress);
                }
            }
        }

        /// <summary>
        ///     Allows animating a sprite by calling the <seealso cref="StatusSetter"/> with the new <seealso cref="SpriteAnimationStatus3D"/>.
        ///     It's run as a <see cref="Coroutine"/>, just in case the return type didn't make that obvious enough.
        /// </summary>
        public delegate IEnumerator SpriteAnimation(StatusSetter statusSetter);

        /// <summary>
        ///     Used to set information in a <see cref="SpriteAnimator3D"/>
        /// </summary>
        public delegate void StatusSetter(SpriteAnimationStatus3D status);
    }
}