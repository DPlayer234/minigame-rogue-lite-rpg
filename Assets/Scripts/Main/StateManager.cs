using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RoguePG.Main.BattleDriver;

namespace SAE.RoguePG.Main
{
    /// <summary>
    ///     Stores and manages general game state.
    ///     Behaves like a singleton; any new instance will override the old one.
    /// </summary>
    public class StateManager : MonoBehaviour
    {
        /// <summary> Tag used by Player Entities </summary>
        public const string PlayerEntityTag = "PlayerEntity";

        /// <summary> Tag used by Enemy Entities </summary>
        public const string EnemyEntityTag = "EnemyEntity";

        /// <summary> The main camera in the scene. To be set from the UnityEditor. </summary>
        [SerializeField]
        private Camera mainCamera;

        /// <summary>
        ///     The main camera in the scene.
        /// </summary>
        public static Camera MainCamera { get; private set; }

        /// <summary>
        ///     The global instance of the <see cref="StateManager"/>.
        /// </summary>
        private static StateManager Instance { get; set; }

        /// <summary>
        ///     Whether there is currently a battle
        /// </summary>
        private static bool IsBattleActive { get; set; }

        /// <summary>
        ///     The Entity whose turn it currently is.
        /// </summary>
        private static BaseBattleDriver CurrentTurnOf { get; set; }

        /// <summary>
        ///     All Entities currently fighting
        /// </summary>
        private static List<BaseBattleDriver> FightingEntities { get; set; }

        /// <summary>
        ///     All Players currently fighting
        /// </summary>
        private static PlayerBattleDriver[] FightingPlayers { get; set; }

        /// <summary>
        ///     All Enemies currently fighting
        /// </summary>
        private static EnemyBattleDriver[] FightingEnemies { get; set; }

        /// <summary>
        ///     All GameObjects that were disabled to make room for the fighters
        /// </summary>
        private static List<GameObject> DeactivatedGameObjects { get; set; }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="StateManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There is an additional active StateManager. The new instance was destroyed.");
                Destroy(this);
                return;
            }

            Instance = this;
            // DontDestroyOnLoad(this);

            // Copy relevant set fields.
            MainCamera = mainCamera;

            // Add camera follow script to Main Camera
            if (MainCamera.GetComponent<CameraController>() == null)
            {
                Debug.LogWarning("There is no CameraController attached to the Main Camera. Attaching one at run-time; please set it in the Editor!");
                MainCamera.gameObject.AddComponent<CameraController>();
            }

            // Resetting static properties and fields
            IsBattleActive = false;
            FightingEntities = null;
            FightingPlayers = null;
            FightingEnemies = null;
            DeactivatedGameObjects = null;

#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
        }

        /// <summary>
        ///     Called by Unity once per frame to update the <see cref="StateManager"/>
        /// </summary>
        private void Update()
        {
            if (IsBattleActive)
            {
                UpdateBattle();
            }
        }

        /// <summary>
        ///     Starts a turn-based battle
        /// </summary>
        public static void StartBattleMode(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            if (!IsBattleActive)
            {
                FightingEntities = new List<BaseBattleDriver>();
                DeactivatedGameObjects = new List<GameObject>();

                FightingPlayers = GetComponentsInCollection<PlayerBattleDriver>(GameObject.FindGameObjectsWithTag(PlayerEntityTag));
                GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(EnemyEntityTag);

                FightingEnemies = new EnemyBattleDriver[] { leaderEnemy };

                ArrangeEntities(leaderPlayer, FightingPlayers);
                //ArrangeEntities(leaderEnemy, allEnemies); // Incorrect... for now

                FightingEntities.AddRange(FightingPlayers);
                FightingEntities.Add(leaderEnemy);

                SetupEntities(FightingEntities, true);

                // Deactivate irrelevant GameObjects
                foreach (GameObject enemy in allEnemies)
                {
                    if (enemy != leaderEnemy.gameObject)
                    {
                        enemy.SetActive(false);
                        DeactivatedGameObjects.Add(enemy);
                    }
                }

                IsBattleActive = true;
                Instance.StartCoroutine(SendBattleStart());

                foreach (var item in FightingEntities)
                {
                    print(item);
                }
            }
        }

        /// <summary>
        ///     Ends a battle
        /// </summary>
        public static void EndBattleMode()
        {
            if (IsBattleActive)
            {
                if (FightingEntities != null)
                {
                    SetupEntities(FightingEntities, false);

                    FightingPlayers = null;
                    FightingEnemies = null;
                    FightingEntities = null;
                }

                if (DeactivatedGameObjects != null)
                {
                    // Reactivate deactivated GameObjects
                    foreach (GameObject gameObject in DeactivatedGameObjects)
                    {
                        gameObject.SetActive(true);
                    }

                    DeactivatedGameObjects = null;
                }

                IsBattleActive = false;
                Instance.StartCoroutine(SendBattleEnd());
            }
        }

        /// <summary>
        ///     Updates the battle once a frame.
        /// </summary>
        private static void UpdateBattle()
        {
            if (CurrentTurnOf == null)
            {
                float overflow = 0.0f;

                // Update drivers
                foreach (BaseBattleDriver battleDriver in FightingEntities)
                {
                    battleDriver.UpdateIdle();

                    overflow = Mathf.Max(overflow, battleDriver.AttackPoints - BaseBattleDriver.MaximumAttackPoints);
                }

                // Something went over the maximum attack points
                if (overflow > 0.0f)
                {
                    // Makes sure that not multiple Entities are initialized as taking a turn
                    bool foundNextTurn = false;

                    foreach (BaseBattleDriver battleDriver in FightingEntities)
                    {
                        // Reduce
                        battleDriver.AttackPoints -= overflow;

                        // This was it: Next Turn of this thing here...!
                        if (!foundNextTurn && battleDriver.AttackPoints == BaseBattleDriver.MaximumAttackPoints)
                        {
                            // Starting turn
                            battleDriver.TakingTurn = true;

                            CurrentTurnOf = battleDriver;

                            foundNextTurn = true;
                        }
                    }
                }
            }
            else
            {
                CurrentTurnOf.UpdateTurn();

                // Ended turn
                if (!CurrentTurnOf.TakingTurn)
                {
                    CurrentTurnOf = null;
                }
            }
        }

        /// <summary>
        ///     Properly arranges all entities for a battle
        /// </summary>
        /// <param name="leader">The leader</param>
        /// <param name="group">The entire group (may or may not include leader)</param>
        private static void ArrangeEntities(Component leader, params Component[] group)
        {
            Vector3 flatCameraForward = MainCamera.transform.forward;
            flatCameraForward.y = 0.0f;
            flatCameraForward.Normalize();

            int placementOffset = 0;
            bool arrangeAbove = false;
            foreach (Component member in group)
            {
                if (member != leader)
                {
                    member.transform.position = leader.transform.position + flatCameraForward * placementOffset * (arrangeAbove ? 1.0f : -1.0f);

                    arrangeAbove = !arrangeAbove;
                    if (arrangeAbove) placementOffset++;
                }
            }
        }

        /// <summary>
        ///     Sets up entities to either start or end a fight
        /// </summary>
        /// <param name="components">The List (or whatever) of <seealso cref="Component"/>s</param>
        /// <param name="start">Whether the fight starts (true) or ends (false)</param>
        private static void SetupEntities(IEnumerable<BaseBattleDriver> components, bool start)
        {
            foreach (Component component in components)
            {
                var entityDriver = component.GetComponent<Driver.BaseDriver>();
                var battleDriver = component.GetComponent<BaseBattleDriver>();

                entityDriver.enabled = !start;
                battleDriver.enabled = start;

                if (start)
                {
                    if (battleDriver is PlayerBattleDriver)
                    {
                        battleDriver.SetAlliesAndOpponents(FightingPlayers, FightingEnemies);
                    }
                    else if (battleDriver is EnemyBattleDriver)
                    {
                        battleDriver.SetAlliesAndOpponents(FightingEnemies, FightingPlayers);
                    }
                }
            }
        }

        /// <summary>
        ///     Returns an array of all components in a given collection of <seealso cref="GameObject"/>s
        /// </summary>
        /// <typeparam name="T">The type of component to get</typeparam>
        /// <param name="gameObjects">The collection of <seealso cref="GameObject"/>s</param>
        /// <returns>An array of all components</returns>
        private static T[] GetComponentsInCollection<T>(ICollection<GameObject> gameObjects)
        {
            T[] components = new T[gameObjects.Count];

            int i = 0;
            foreach (GameObject gameObject in gameObjects)
            {
                components[i++] = gameObject.GetComponent<T>();
            }

            return components;
        }

        /// <summary>
        ///     Calls <seealso cref="BaseBattleDriver.OnBattleStart"/> for all <seealso cref="BaseBattleDriver"/>s
        /// </summary>
        /// <returns>A routine</returns>
        private static IEnumerator SendBattleStart()
        {
            yield return null;

            foreach (BaseBattleDriver battleDriver in FightingEntities)
            {
                battleDriver.OnBattleStart();
            }
        }

        /// <summary>
        ///     Calls <seealso cref="BaseBattleDriver.OnBattleEnd"/> for all <seealso cref="BaseBattleDriver"/>s
        /// </summary>
        /// <returns>A routine</returns>
        private static IEnumerator SendBattleEnd()
        {
            yield return null;

            foreach (BaseBattleDriver battleDriver in FightingEntities)
            {
                battleDriver.OnBattleEnd();
            }
        }
    }
}