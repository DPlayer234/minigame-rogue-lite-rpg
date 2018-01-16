using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG
{
    /// <summary>
    ///     Will make a <seealso cref="Sprite"/> rotate towards the set <seealso cref="Camera"/>.
    ///     Also makes sure that all Sprites have the correct order within the 3D space.
    /// </summary>
    public class SpriteRotator3D : MonoBehaviour
    {
        /// <summary> The camera to rotate to. Defaults to <see cref="StateManager.MainCamera"/>. </summary>
        public Camera mainCamera;

        /// <summary> Contains all sprite renderers in this <seealso cref="GameObject"/> children. </summary>
        private SpriteRenderer[] spriteRenderers;

        /// <summary> Multiplier for sorting order </summary>
        private const float SortingOrderMultiplier = 1000.0f;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="SpriteRotator3D"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            if (mainCamera == null)
            {
                // Default to the main camera of the scene
                mainCamera = StateManager.MainCamera;
            }

            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        /// <summary>
        ///     Called by Unity once every frame after all Updates and FixedUpdates have been executed.
        /// </summary>
        private void LateUpdate()
        {
            // Face Camera
            this.transform.rotation = Quaternion.Euler(0.0f, mainCamera.transform.rotation.eulerAngles.y, 0.0f);

            // Set Layer Order => Get Distance to camera, add int.MinValue to increase overall range and cap the value at int.MaxValue
            Vector3 distances = this.transform.position - mainCamera.transform.position;

            int sortingOrder = -(int)Mathf.Min(int.MaxValue,
                SortingOrderMultiplier * (distances.x * distances.x + distances.y * distances.y + distances.z * distances.z) + int.MinValue);

            foreach (var renderer in spriteRenderers)
            {
                renderer.sortingOrder = sortingOrder;
            }
        }
    }
}