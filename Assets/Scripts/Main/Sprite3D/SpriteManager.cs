//-----------------------------------------------------------------------
// <copyright file="SpriteManager.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.Sprite3D
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    ///     Will make <seealso cref="Sprite"/>s rotate towards the set <seealso cref="Camera"/>.
    ///     Also makes sure that all Sprites have the correct order within the 3D space.
    /// </summary>
    [RequireComponent(typeof(SortingGroup))]
    public class SpriteManager : MonoBehaviour
    {
        /// <summary> The tag used by the sprite root </summary>
        public const string SpriteRootTag = "SpriteRoot";

        /// <summary> The tag used by the sprite body </summary>
        public const string SpriteBodyTag = "SpriteBody";

        /// <summary> Contains all the associated <see cref="Transform"/>s usable in animations. </summary>
        [HideInInspector]
        public Transform[] animatedTransforms;

        /// <summary> Transform of the Sprite root </summary>
        [HideInInspector]
        public Transform rootTransform;

        /// <summary> Transform of the Sprite body </summary>
        [HideInInspector]
        public Transform bodyTransform;

        /// <summary> Multiplier for sorting order </summary>
        private const float SortingOrderMultiplier = 20.0f;

        /// <summary> How fast the flip animation is played. </summary>
        private const float FlipSpeed = 10.0f;

        /// <summary> Whether it's facing right. </summary>
        private bool isFacingRight;

        /// <summary> -1.0..1.0; where the flipping animation currently is. </summary>
        private float flipStatus;

        /// <summary> Whether the Sprite is facing right </summary>
        public bool IsFacingRight { get { return this.isFacingRight; } }

        /// <summary>
        ///     Starts the flipping animation.
        /// </summary>
        /// <param name="faceRight">Whether or not to face right.</param>
        public void FlipToDirection(bool faceRight)
        {
            if (this.isFacingRight != faceRight)
            {
                this.isFacingRight = faceRight;

                this.StartCoroutine(this.DoFlipAnimation(faceRight));
            }
        }

        /// <summary>
        ///     Similar to <seealso cref="FlipToDirection"/>, but skips the animation.
        ///     Will effectively cancel any active flip animation.
        /// </summary>
        /// <param name="faceRight">Whether or not to face right.</param>
        public void SetDirection(bool faceRight)
        {
            this.isFacingRight = faceRight;
            this.flipStatus = faceRight ? 1.0f : -1.0f;
        }
        
        /// <summary>
        ///     Coroutine to run the flipping animation.
        /// </summary>
        /// <param name="faceRight">Whether to face right after the animation</param>
        /// <returns>A routine...</returns>
        private IEnumerator DoFlipAnimation(bool faceRight)
        {
            float timeMultiplier = (faceRight ? 1.0f : -1.0f) * FlipSpeed;
            bool visiblyFacingRight = this.flipStatus > 0.0f;

            // Cancel coroutine if it flips another time during the animation
            while ((faceRight ? this.flipStatus < 1.0f : this.flipStatus > -1.0f) && faceRight == this.isFacingRight)
            {
                this.flipStatus = Mathf.Clamp(this.flipStatus + timeMultiplier * Time.deltaTime, -1.0f, 1.0f);

                if (visiblyFacingRight && this.flipStatus <= 0.0f || !visiblyFacingRight && this.flipStatus > 0.0f)
                {
                    // Invert relative Z-position
                    foreach (Transform transform in this.animatedTransforms)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -transform.localPosition.z);
                    }

                    visiblyFacingRight = this.flipStatus > 0.0f;
                }

                this.bodyTransform.localRotation = Quaternion.Euler(0.0f, 90.0f - this.flipStatus * 90.0f, 0.0f);

                yield return null;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="SpriteManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.isFacingRight = true;
            this.flipStatus = 1.0f;

            // Finding Sprite Root
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform transform = this.transform.GetChild(i);
                if (transform.CompareTag(SpriteRootTag))
                {
                    this.rootTransform = transform;
                    break;
                }
            }

            if (this.rootTransform == null) throw new RPGException(RPGException.Cause.SpriteNoRootHierarchy);

            // Finding Sprite Body
            for (int i = 0; i < this.rootTransform.childCount; i++)
            {
                Transform transform = this.rootTransform.GetChild(i);
                if (transform.CompareTag(SpriteBodyTag))
                {
                    this.bodyTransform = transform;
                    break;
                }
            }

            if (this.bodyTransform == null) throw new RPGException(RPGException.Cause.SpriteNoBody);

            this.animatedTransforms = this.bodyTransform.GetComponentsInChildren<Transform>();
        }

        /// <summary>
        ///     Called by Unity once every frame after all Updates and FixedUpdates have been executed.
        /// </summary>
        private void LateUpdate()
        {
            // Face Camera
            this.rootTransform.rotation =
                Quaternion.Euler(
                    0.0f,
                    MainManager.CameraController.transform.rotation.eulerAngles.y,
                    0.0f);
        }
    }
}