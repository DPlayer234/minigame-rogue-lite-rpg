namespace SAE.RoguePG.Main
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Will make the Camera that this is attached to follow the referenced GameObject
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        ///     The <seealso cref="Transform"/> of the GameObject to follow
        /// </summary>
        [HideInInspector]
        public Transform following;

        /// <summary>
        ///     How much higher should the camera be than the pivot of <see cref="following"/>
        /// </summary>
        public float preferredHeight = 1.0f;

        /// <summary>
        ///     How far away should the camera be from <see cref="following"/>
        /// </summary>
        public float preferredDistance = 4.0f;

        /// <summary> How fast the camera approaches the target position. Lower values increase the speed. </summary>
        [Range(0.0f, 1.0f)]
        public float movementSpeedBase = 0.1f;

        /// <summary> How fast the camera approaches the target rotation. Lower values increase the speed. </summary>
        [Range(0.0f, 1.0f)]
        public float rotationSpeedBase = 0.01f;

        /// <summary>
        ///     Called by Unity once every frame after all Updates and FixedUpdates have been executed.
        /// </summary>
        private void FixedUpdate()
        {
            if (this.following != null)
            {
                Vector3 currentRotation = VariousCommon.WrapDegrees(this.transform.eulerAngles);
                this.transform.LookAt(this.following);
                Vector3 targetRotation = VariousCommon.WrapDegrees(this.transform.eulerAngles);

                Vector3 thisToFollowing = this.transform.forward;
                thisToFollowing.y = 0.0f;
                thisToFollowing.Normalize();

                Vector3 newPosition = VariousCommon.ExponentialLerp(
                    this.transform.position,
                    this.following.position - thisToFollowing * this.preferredDistance,
                    this.movementSpeedBase,
                    Time.fixedDeltaTime);

                newPosition.y = this.following.position.y + this.preferredHeight;
                this.transform.position = newPosition;

                this.transform.eulerAngles = VariousCommon.ExponentialLerpRotation(
                    currentRotation,
                    targetRotation,
                    this.rotationSpeedBase,
                    Time.fixedDeltaTime);
            }
        }
    }
}