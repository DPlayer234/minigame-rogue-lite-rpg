using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SAE.RoguePG.Main.Sprite3D
{
    /// <summary>
    ///     Will make <seealso cref="Sprite"/>s rotate towards the set <seealso cref="Camera"/>.
    ///     Also makes sure that all Sprites have the correct order within the 3D space.
    /// </summary>
    [RequireComponent(typeof(SortingGroup))]
    public class SpriteManager : MonoBehaviour
    {
        [HideInInspector]
        /// <summary> Contains all the associated <see cref="Transform"/>s usable in animations. </summary>
        public Transform[] animatedTransforms;

        [HideInInspector]
        /// <summary> Transform of the Sprite root </summary>
        public Transform rootTransform;

        [HideInInspector]
        /// <summary> Transform of the Sprite body </summary>
        public Transform bodyTransform;

        /// <summary> Whether it's facing right. </summary>
        private bool isFacingRight;

        /// <summary> -1.0..1.0; where the flipping animation currently is. </summary>
        private float flipStatus;

        /// <summary> Multiplier for sorting order </summary>
        private const float SortingOrderMultiplier = 20.0f;

        /// <summary> How fast the flip animation is played. </summary>
        private const float FlipSpeed = 10.0f;

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

                StartCoroutine(this.DoFlipAnimation(faceRight));
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

            //if (this.transform.childCount < 1) throw new Exceptions.SpriteManagerException("This GameObject is lacking a Sprite Root/Hierarchy.");
            //this.rootTransform = this.transform.GetChild(0);

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform transform = this.transform.GetChild(i);
                if (transform.CompareTag("SpriteRoot"))
                {
                    this.rootTransform = transform;
                    break;
                }
            }
            if (this.rootTransform == null) throw new Exceptions.SpriteManagerException("This GameObject is lacking a Sprite Root/Hierarchy.");

            for (int i = 0; i < this.rootTransform.childCount; i++)
            {
                Transform transform = this.rootTransform.GetChild(i);
                if (transform.CompareTag("SpriteBody"))
                {
                    this.bodyTransform = transform;
                    break;
                }
            }
            if (this.bodyTransform == null) throw new Exceptions.SpriteManagerException("This GameObject is lacking a Sprite Body.");
            
            this.animatedTransforms = this.bodyTransform.GetComponentsInChildren<Transform>();
        }

        /// <summary>
        ///     Called by Unity once every frame after all Updates and FixedUpdates have been executed.
        /// </summary>
        private void LateUpdate()
        {
            // Face Camera
            this.rootTransform.rotation = Quaternion.Euler(0.0f, StateManager.MainCamera.transform.rotation.eulerAngles.y, 0.0f);
        }
    }
}