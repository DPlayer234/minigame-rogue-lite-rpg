using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        ///     The global instance of the <see cref="StateManager"/>.
        /// </summary>
        private static StateManager instance;

        /// <summary>
        ///     All GameObjects currently fighting
        /// </summary>
        private static List<GameObject> fightingGameObjects;

        /// <summary>
        ///     All GameObjects that were disabled to make room for the fighters
        /// </summary>
        private static List<GameObject> deactivatedGameObjects;

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

            // Add camera follow script to Main Camera
            if (MainCamera.gameObject.GetComponent<CameraController>() == null)
            {
                Debug.LogWarning("There is no CameraController attached to the Main Camera. Attaching one at run-time; please set it in the Editor!");
                MainCamera.gameObject.AddComponent<CameraController>();
            }

#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
        }

        /// <summary>
        ///     Properly arranges all entities for a battle
        /// </summary>
        /// <param name="leader">The leader</param>
        /// <param name="group">The entire group (may or may not include leader)</param>
        private static void ArrangeEntities(GameObject leader, params GameObject[] group)
        {
            Vector3 flatCameraForward = MainCamera.transform.forward;
            flatCameraForward.y = 0.0f;
            flatCameraForward.Normalize();

            int placementOffset = 0;
            bool arrangeAbove = false;
            foreach (GameObject member in group)
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
        ///     Removes or adds a Component
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="gameObject">The <seealso cref="GameObject"/> to add or remove the component from</param>
        /// <param name="add">Whether to add (true) or remove (false)</param>
        private static void AddOrRemoveComponent<T>(GameObject gameObject, bool add) where T : Component
        {
            if (add)
            {
                gameObject.AddComponent<T>();
            }
            else
            {
                T component = gameObject.GetComponent<T>();

                if (component != null)
                {
                    Destroy(component);
                }
            }
        }

        /// <summary>
        ///     Sets up entities to either start or end a fight
        /// </summary>
        /// <param name="gameObjects">The List (or whatever) of <seealso cref="GameObject"/>s</param>
        /// <param name="start">Whether the fight starts (true) or ends (false)</param>
        private static void SetupEntities(IEnumerable<GameObject> gameObjects, bool start)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                // Battle Driver
                AddOrRemoveComponent<BattleDriver.EntityBattleDriver>(gameObject, start);

                gameObject.GetComponent<Driver.EntityDriver>().enabled = !start;

                // Is Player
                var playerDriver = gameObject.GetComponent<Driver.PlayerDriver>();
                if (playerDriver != null)
                {
                    AddOrRemoveComponent<BattleDriver.PlayerBattleDriver>(gameObject, start);

                    playerDriver.enabled = !start;
                    continue;
                }

                // Is Enemy
                var enemyDriver = gameObject.GetComponent<Driver.EnemyDriver>();
                if (enemyDriver != null)
                {
                    AddOrRemoveComponent<BattleDriver.EnemyBattleDriver>(gameObject, start);

                    enemyDriver.enabled = !start;
                    continue;
                }
            }
        }

        /// <summary>
        ///     Starts a turn-based battle
        /// </summary>
        public static void StartBattleMode(GameObject leaderPlayer, GameObject leaderEnemy)
        {
            fightingGameObjects = new List<GameObject>();
            deactivatedGameObjects = new List<GameObject>();

            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag(PlayerEntityTag);
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(EnemyEntityTag);

            ArrangeEntities(leaderPlayer, allPlayers);
            //ArrangeEntities(leaderEnemy, allEnemies); // Incorrect... for now

            fightingGameObjects.AddRange(allPlayers);
            fightingGameObjects.Add(leaderEnemy);

            SetupEntities(fightingGameObjects, true);

            // Deactivate irrelevant GameObjects
            foreach (GameObject enemy in allEnemies)
            {
                if (enemy != leaderEnemy)
                {
                    enemy.SetActive(false);
                    deactivatedGameObjects.Add(enemy);
                }
            }
        }

        /// <summary>
        ///     Exits a battle
        /// </summary>
        public static void ExitBattleMode()
        {
            if (fightingGameObjects != null)
            {
                SetupEntities(fightingGameObjects, false);
            }
            else
            {
                Debug.LogWarning("A battle may have just been exited without ever being started...?");
            }

            if (deactivatedGameObjects != null)
            {
                // Reactivate deactivated GameObjects
                foreach (GameObject gameObject in deactivatedGameObjects)
                {
                    gameObject.SetActive(true);
                }

                deactivatedGameObjects = null;
            }
        }
    }
}