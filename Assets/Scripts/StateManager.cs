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
        [SerializeField]
        /// <summary> The main camera in the scene. To be set from the UnityEditor. </summary>
        private Camera mainCamera;

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
                Destroy(instance);
            }

            instance = this;
            DontDestroyOnLoad(this);

            // Copy relevant set fields.
            MainCamera = mainCamera;
        }
    }
}