namespace SAE.RoguePG.Main
{
    using SAE.RoguePG.Main.BattleDriver;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Stores and manages general game state of the Main scene.
    ///     Behaves like a singleton; any new instance will override the old one.
    /// </summary>
    public class MainManager : MonoBehaviour
    {
        /// <summary> Prefab for player health bars </summary>
        public GameObject statusDisplayPrefab;

        /// <summary> Prefab for an empty panel </summary>
        public GameObject genericPanelPrefab;

        /// <summary> The main camera in the scene. To be set from the UnityEditor. </summary>
        [SerializeField]
        private Camera mainCamera;

        /// <summary> The parent object for the exploration HUD. </summary>
        [SerializeField]
        private GameObject exploreHud;

        /// <summary> The parent object for the battle HUD. </summary>
        [SerializeField]
        private GameObject battleHud;

        /// <summary>
        ///     The global instance of the <see cref="MainManager"/>.
        /// </summary>
        public static MainManager Instance { get; private set; }

        /// <summary>
        ///     The main camera in the scene.
        /// </summary>
        public static Camera MainCamera { get { return Instance.mainCamera; } }

        /// <summary>
        ///     The parent object for the exploration HUD.
        /// </summary>
        public static GameObject ExploreHud { get { return Instance.exploreHud; } }

        /// <summary>
        ///     The parent object for the battle HUD.
        /// </summary>
        public static GameObject BattleHud { get { return Instance.battleHud; } }

        /// <summary>
        ///     Prefab for any UI panel.
        /// </summary>
        public static GameObject GenericPanelPrefab { get { return Instance.genericPanelPrefab; } }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="MainManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There was an additional active MainManager. The new instance was destroyed.");
                Destroy(this);
                return;
            }
            
            Instance = this;
            //// DontDestroyOnLoad(this);

            if (MainCamera == null) throw new Exceptions.MainManagerException("There is no MainCamera set!");
            if (ExploreHud == null) throw new Exceptions.MainManagerException("There is no ExploreHud set!");
            if (BattleHud == null) throw new Exceptions.MainManagerException("There is no BattleHud set!");

            if (MainCamera.GetComponent<CameraController>() == null)
            {
                // Add camera follow script to Main Camera
                Debug.LogWarning("There is no CameraController attached to the Main Camera. Attaching one at run-time; please set it in the Editor!");
                MainCamera.gameObject.AddComponent<CameraController>();
            }

            ExploreHud.SetActive(true);
            BattleHud.SetActive(false);

#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
        }
    }
}