using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG.Main
{
    /// <summary>
    ///     Will make the Camera that this is attached to follow the referenced GameObject
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        ///     The GameObject to follow
        /// </summary>
        [HideInInspector]
        public GameObject following;

        /// <summary>
        ///     How much higher should the camera be than the pivot of <see cref="following"/>
        /// </summary>
        public float preferredHeight = 1.0f;

        /// <summary>
        ///     How far away should the camera be from <see cref="following"/>
        /// </summary>
        public float preferredDistance = 4.0f;

        /// <summary> Leniency in distance when following </summary>
        private const float FollowDistanceLeniency = 0.5f;

        /// <summary>
        ///     Called by Unity once every frame after all Updates and FixedUpdates have been executed.
        /// </summary>
        private void LateUpdate()
        {
            if (this.following != null)
            {
                this.transform.LookAt(this.following.transform);

                Vector3 thisToFollowing = this.transform.forward;

                float distanceToFollowing = (this.following.transform.position - this.transform.position).magnitude;

                
            }
        }
    }
}