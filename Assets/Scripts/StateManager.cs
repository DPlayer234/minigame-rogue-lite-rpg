using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG
{
    /// <summary>
    ///     Stores and manages general game state.
    ///     Behaves like a singleton; any new instance will override the old one.
    /// </summary>
    public class StateManager : MonoBehaviour
    {
        /// <summary> The main camera in the scene. To be set from the UnityEditor. </summary>
        [SerializeField]
        private Camera mainCamera;

        /// <summary> The layer entities are on. </summary>
        [SerializeField]
        private int entitiesLayer;

        /// <summary>
        ///     The global instance of the <see cref="StateManager"/>.
        /// </summary>
        private static StateManager instance;

        /// <summary>
        ///     The main camera in the scene.
        /// </summary>
        public static Camera MainCamera { private set; get; }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="StateManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("There is an additional active StateManager. The new instance was destroyed.");
                Destroy(this);
                return;
            }

            instance = this;
            // DontDestroyOnLoad(this);

            // Copy relevant set fields.
            MainCamera = mainCamera;

            // Entities should ignore collisions with each other
            if (entitiesLayer >= 0 && entitiesLayer <= 31)
            {
                Physics.IgnoreLayerCollision(entitiesLayer, entitiesLayer, true);
            }
            else
            {
                Debug.LogWarning("Physics Layers range from 0-31. Entities will collide until this is corrected.");
            }

            // Add camera follow script to Main Camera
            if (MainCamera.gameObject.GetComponent<Main.CameraController>() == null)
            {
                Debug.LogWarning("There is no CameraController attached to the Main Camera. Attaching one at run-time; please set it in the Editor!");
                MainCamera.gameObject.AddComponent<Main.CameraController>();
            }

#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
        }
    }
}