﻿namespace SAE.RoguePG.Main
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using SAE.RoguePG.Main.UI;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Stores and manages general game state of the Main scene.
    ///     Behaves like a singleton; any existing instance will prevent
    ///     a new one from being created.
    /// </summary>
    [DisallowMultipleComponent]
    public class MainManager : MonoBehaviour
    {
        /// <summary>
        ///     The current player party
        /// </summary>
        [Tooltip("Do not set this. It is easier to debug with something in the editor.")]
        public List<PlayerDriver> playerPartyDebug;

        /// <summary> Prefab for player health bars </summary>
        public GameObject statusDisplayPrefab;

        /// <summary> Prefab for an empty panel </summary>
        public GameObject genericPanelPrefab;

        /// <summary> Prefab for buttons </summary>
        public Button genericButtonPrefab;

        /// <summary> Prefab for 3D Text </summary>
        public Text3DController generic3DTextPrefab;

        /// <summary> The main camera in the scene. To be set from the UnityEditor. </summary>
        [SerializeField]
        private Camera mainCamera;

        /// <summary> The parent object for the exploration HUD. </summary>
        [SerializeField]
        private GameObject exploreHud;

        /// <summary> The parent object for the battle HUD. </summary>
        [SerializeField]
        private GameObject battleHud;

        /// <summary> The canvas that is in world space. </summary>
        [SerializeField]
        private Canvas worldCanvas;

        /// <summary>
        ///     The global instance of the <see cref="MainManager"/>.
        /// </summary>
        public static MainManager Instance { get; private set; }

        /// <summary>
        ///     The <seealso cref="CameraController"/> attached to the MainCamera.
        /// </summary>
        public static CameraController CameraController { get; private set; }

        /// <summary>
        ///     The parent object for the exploration HUD.
        /// </summary>
        public static GameObject ExploreHud { get { return MainManager.Instance.exploreHud; } }

        /// <summary>
        ///     The parent object for the battle HUD.
        /// </summary>
        public static GameObject BattleHud { get { return MainManager.Instance.battleHud; } }

        /// <summary>
        ///     The canvas that is in world space.
        /// </summary>
        public static Canvas WorldCanvas { get { return MainManager.Instance.worldCanvas; } }

        /// <summary>
        ///     Prefab for any UI panel.
        /// </summary>
        public static GameObject GenericPanelPrefab { get { return MainManager.Instance.genericPanelPrefab; } }

        /// <summary>
        ///     Prefab for buttons.
        /// </summary>
        public static Button GenericButtonPrefab { get { return MainManager.Instance.genericButtonPrefab; } }

        /// <summary>
        ///     Prefab for 3D Text.
        /// </summary>
        public static Text3DController Generic3DTextPrefab { get { return MainManager.Instance.generic3DTextPrefab; } }

        /// <summary>
        ///     Spawns an entity based on a prefab with a bonus
        /// </summary>
        /// <param name="prefab">The prefab to use</param>
        /// <param name="bonus1">The first bonus stat</param>
        /// <param name="bonus2">The second bonus stat</param>
        /// <returns>The new entity</returns>
        public static T SpawnEntityWithBonus<T>(T prefab, Stat? bonus1 = Stat.Random, Stat? bonus2 = Stat.Random) where T : BaseDriver
        {
            T driver = Instantiate(prefab);
            BaseBattleDriver battleDriver = driver.battleDriver;

            if (bonus1 == Stat.Random)
            {
                bonus1 = MainGeneral.GetRandomStat();
            }

            if (bonus2 == Stat.Random)
            {
                bonus2 = MainGeneral.GetRandomStat(bonus1);
            }

            if (bonus1 != null)
            {
                battleDriver.SetBaseStat(
                    (Stat)bonus1,
                    battleDriver.GetBaseStat((Stat)bonus1) * BaseBattleDriver.BonusStatMultiplier);
            }

            if (bonus2 != null)
            {
                battleDriver.SetBaseStat(
                    (Stat)bonus2,
                    battleDriver.GetBaseStat((Stat)bonus2) * BaseBattleDriver.BonusStatMultiplier);
            }

            return driver;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="MainManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            if (MainManager.Instance != null)
            {
                Debug.LogWarning("There was an additional active MainManager. The new instance was destroyed.");
                MonoBehaviour.Destroy(this);
                return;
            }

            MainManager.Instance = this;
            //// DontDestroyOnLoad(this);

            this.ValidateSetup();

            MainManager.CameraController = this.mainCamera.GetComponent<CameraController>();

            MainManager.ExploreHud.SetActive(true);
            MainManager.BattleHud.SetActive(false);

            PlayerDriver.CreateNewParty();

#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
        }

        /// <summary>
        ///     Validates that everything is correctly setup and throws an exception otherwise.
        /// </summary>
        private void ValidateSetup()
        {
            if (this.mainCamera == null) throw new Exceptions.MainManagerException("There is no MainCamera set!");
            if (MainManager.ExploreHud == null) throw new Exceptions.MainManagerException("There is no ExploreHud set!");
            if (MainManager.BattleHud == null) throw new Exceptions.MainManagerException("There is no BattleHud set!");

            if (this.mainCamera.GetComponent<CameraController>() == null)
            {
                // Add camera follow script to Main Camera
                Debug.LogWarning("There is no CameraController attached to the Main Camera. Attaching one at run-time; please set it in the Editor!");
                this.mainCamera.gameObject.AddComponent<CameraController>();
            }
        }
    }
}