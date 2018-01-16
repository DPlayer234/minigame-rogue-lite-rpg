using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SAE.RougePG.Main
{
    /// <summary>
    ///     Will make <seealso cref="Sprite"/>s rotate towards the set <seealso cref="Camera"/>.
    ///     Also makes sure that all Sprites have the correct order within the 3D space.
    /// </summary>
    public class SpriteManager3D : MonoBehaviour
    {
        /// <summary> The camera to rotate to. Defaults to <see cref="StateManager.MainCamera"/>. </summary>
        public Camera mainCamera;

        /// <summary> The associated <see cref="SortingGroup"/>. </summary>
        private SortingGroup sortingGroup;

        /// <summary> Contains all the associated <see cref="Transform"/>s. </summary>
        private Transform[] transforms;

        /// <summary> First child of this <seealso cref="GameObject"/> </summary>
        private Transform firstChild;

        /// <summary> Whether it's facing right. </summary>
        private bool isFacingRight;

        /// <summary> -1.0..1.0; where the flipping animation currently is. </summary>
        private float flipStatus;

        /// <summary> Multiplier for sorting order </summary>
        private const float SortingOrderMultiplier = 20.0f;

        /// <summary> How fast the flip animation is played. </summary>
        private const float FlipSpeed = 10.0f;

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
                    foreach (Transform transform in this.transforms)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -transform.localPosition.z);
                    }

                    visiblyFacingRight = this.flipStatus > 0.0f;
                }

                this.firstChild.transform.localRotation = Quaternion.Euler(0.0f, 90.0f - this.flipStatus * 90.0f, 0.0f);

                yield return null;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="SpriteManager3D"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            if (mainCamera == null)
            {
                // Default to the main camera of the scene
                mainCamera = StateManager.MainCamera;
            }
            
            this.sortingGroup = GetComponent<SortingGroup>();
            this.isFacingRight = true;
            this.flipStatus = 1.0f;

            this.firstChild = this.transform.GetChild(0);
            this.transforms = this.firstChild.GetComponentsInChildren<Transform>();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                FlipToDirection(!isFacingRight);
            }
        }

        /// <summary>
        ///     Called by Unity once every frame after all Updates and FixedUpdates have been executed.
        /// </summary>
        private void LateUpdate()
        {
            // Face Camera
            this.transform.rotation = Quaternion.Euler(0.0f, mainCamera.transform.rotation.eulerAngles.y, 0.0f);

            // Set Layer Order => Get Distance to camera, add int.MinValue to increase overall range and cap the value at int.MaxValue

            Vector3 positionDifference = mainCamera.transform.position - this.transform.position;

            float sqrDistance = (positionDifference - Vector3.Dot(positionDifference, this.transform.right) * this.transform.right).sqrMagnitude;

            sortingGroup.sortingOrder = -(int)Mathf.Min(int.MaxValue,
                SortingOrderMultiplier * sqrDistance);
        }
    }
}