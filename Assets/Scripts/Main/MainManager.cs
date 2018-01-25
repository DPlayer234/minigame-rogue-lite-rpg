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
        /// <summary> Tag used by Player Entities </summary>
        public const string PlayerEntityTag = "PlayerEntity";

        /// <summary> Tag used by Enemy Entities </summary>
        public const string EnemyEntityTag = "EnemyEntity";

        /// <summary> Tag used by the exploration HUD root </summary>
        public const string ExploreHudTag = "ExploreHud";

        /// <summary> Tag used by the battle HUD root </summary>
        public const string BattleHudTag = "BattleHud";

        /// <summary> Prefab for player health bars </summary>
        public GameObject playerHealthBarPrefab;

        /// <summary> Prefab for enemy health bars </summary>
        public GameObject enemyHealthBarPrefab;

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

        /// <summary> Whether there is currently a battle </summary>
        private bool isBattleActive;

        /// <summary> The Entity whose turn it currently is. </summary>
        private BaseBattleDriver currentTurnOf;

        /// <summary> All Entities currently fighting </summary>
        private List<BaseBattleDriver> fightingEntities;

        /// <summary> All Players currently fighting </summary>
        private PlayerBattleDriver[] fightingPlayers;

        /// <summary> All Enemies currently fighting </summary>
        private EnemyBattleDriver[] fightingEnemies;

        /// <summary> All GameObjects that were disabled to make room for the fighters </summary>
        private List<GameObject> deactivatedGameObjects;

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
        ///     Whether there is currently a battle
        /// </summary>
        public static bool IsBattleActive { get { return Instance.isBattleActive; } }

        /// <summary>
        ///     The Entity whose turn it currently is.
        /// </summary>
        public static BaseBattleDriver CurrentTurnOf { get { return Instance.currentTurnOf; } }

        /// <summary>
        ///     Starts a turn-based battle
        /// </summary>
        /// <param name="leaderPlayer">The leading player</param>
        /// <param name="leaderEnemy">The leading enemy</param>
        public static void StartBattleMode(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            if (Instance == null) throw new Exceptions.MainManagerException("Cannot start battle mode without an Instance of MainManager!");

            Instance.StartBattleModeInstanced(leaderPlayer, leaderEnemy);
        }

        /// <summary>
        ///     Starts a turn-based battle
        /// </summary>
        public static void EndBattleMode()
        {
            if (Instance == null) throw new Exceptions.MainManagerException("Cannot end battle mode without an Instance of MainManager!");

            Instance.EndBattleModeInstanced();
        }

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

            this.isBattleActive = false;
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

        /// <summary>
        ///     Called by Unity once per frame to update the <see cref="MainManager"/>
        /// </summary>
        private void Update()
        {
            if (this.isBattleActive)
            {
                this.UpdateBattle();
            }
        }

        /// <summary>
        ///     Updates the battle once a frame.
        /// </summary>
        private void UpdateBattle()
        {
            if (this.currentTurnOf == null)
            {
                this.UpdateBattleIdle();
            }
            else
            {
                this.UpdateBattleTurn();
            }
        }

        /// <summary>
        ///     Updates the battle's idle-phase once a frame while nothing is taking a turn
        /// </summary>
        private void UpdateBattleIdle()
        {
            float overflow = 0.0f;

            // Update drivers
            foreach (BaseBattleDriver battleDriver in this.fightingEntities)
            {
                if (battleDriver.CanStillFight)
                {
                    battleDriver.UpdateIdle();

                    overflow = Mathf.Max(overflow, battleDriver.AttackPoints - BaseBattleDriver.MaximumAttackPoints);
                }
                else
                {
                    battleDriver.AttackPoints = 0.0f;
                }
            }

            // Something went over the maximum attack points
            if (overflow > 0.0f)
            {
                // Makes sure that not multiple Entities are initialized as taking a turn
                bool foundNextTurn = false;

                foreach (BaseBattleDriver battleDriver in this.fightingEntities)
                {
                    // Reduce
                    battleDriver.AttackPoints -= overflow;

                    // This was it: Next Turn of this thing here...!
                    if (!foundNextTurn && battleDriver.AttackPoints == BaseBattleDriver.MaximumAttackPoints)
                    {
                        // Starting turn
                        battleDriver.TakingTurn = true;

                        this.currentTurnOf = battleDriver;

                        foundNextTurn = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the battle's turn-phase once a frame while something is taking a turn
        /// </summary>
        private void UpdateBattleTurn()
        {
            if (this.currentTurnOf != null)
            {
                this.currentTurnOf.UpdateTurn();

                // Ended turn
                if (!this.currentTurnOf.TakingTurn)
                {
                    this.currentTurnOf = null;

                    this.CheckAndUpdateBattleStatus();
                }
            }
            else
            {
                Debug.LogError("Cannot update the battle turn for value 'null'.");
            }
        }

        /// <summary>
        ///     Checks and updates battle status. (Triggers game overs or battle ends when appropriate)
        /// </summary>
        private void CheckAndUpdateBattleStatus()
        {
            bool playersAlive = false;
            foreach (var playerEntity in this.fightingPlayers)
            {
                playersAlive |= playerEntity.CanStillFight;
            }

            bool enemiesAlive = false;
            foreach (var enemyEntity in this.fightingEnemies)
            {
                enemiesAlive |= enemyEntity.CanStillFight;
            }

            if (playersAlive != enemiesAlive)
            {
                EndBattleMode();

                if (playersAlive)
                {
                    foreach (var enemyEntity in this.fightingEnemies)
                    {
                        Destroy(enemyEntity.gameObject);
                    }
                }
                else
                {
                    this.DoGameOver();
                }
            }
            else
            {
                if (!playersAlive)
                {
                    this.DoGameOver();
                }
            }
        }

        private void DoGameOver()
        {
            Debug.LogError("<b>!! GAME OVER !!</b>");

            foreach (var playerEntity in this.fightingPlayers)
            {
                Destroy(playerEntity.gameObject);
            }
        }

        /// <summary>
        ///     Starts a turn-based battle on the instance
        /// </summary>
        private void StartBattleModeInstanced(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            if (!this.isBattleActive)
            {
                this.fightingEntities = new List<BaseBattleDriver>();
                this.deactivatedGameObjects = new List<GameObject>();
                
                // Deactivate irrelevant GameObjects and get fighting enemies
                {
                    GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(EnemyEntityTag);

                    List<EnemyBattleDriver> listOfFightingEnemies = new List<EnemyBattleDriver>();

                    Driver.EnemyDriver leaderEnemyDriver = leaderEnemy.GetComponent<Driver.EnemyDriver>();

                    foreach (GameObject enemy in allEnemies)
                    {
                        Driver.EnemyDriver enemyDriver = enemy.GetComponent<Driver.EnemyDriver>();

                        if (enemyDriver != null && (enemyDriver.leader == leaderEnemyDriver || enemyDriver == leaderEnemyDriver))
                        {
                            // Required component; cannot be missing unless extremely messed with
                            listOfFightingEnemies.Add(enemy.GetComponent<EnemyBattleDriver>());
                        }
                        else
                        {
                            enemy.SetActive(false);
                            this.deactivatedGameObjects.Add(enemy);
                        }
                    }

                    this.fightingPlayers = VariousCommon.GetComponentsInCollection<PlayerBattleDriver>(GameObject.FindGameObjectsWithTag(PlayerEntityTag));
                    this.fightingEnemies = listOfFightingEnemies.ToArray();
                }

                // Adjust positions and rotations
                {
                    Vector3 enemyToPlayer = (leaderPlayer.transform.position - leaderEnemy.transform.position).normalized;
                    this.transform.position = (leaderPlayer.transform.position + leaderEnemy.transform.position) * 0.5f;
                    this.transform.right = -enemyToPlayer;

                    leaderPlayer.transform.position = this.transform.position + enemyToPlayer;
                    leaderEnemy.transform.position = this.transform.position - enemyToPlayer;

                    CameraController cameraController = MainCamera.GetComponent<CameraController>();
                    cameraController.following = this.transform;
                    cameraController.transform.position = this.transform.position - this.transform.forward * cameraController.preferredDistance;
                    cameraController.transform.forward = this.transform.forward;

                    this.ArrangeEntities(leaderPlayer, this.fightingPlayers, this.transform.forward);
                    this.ArrangeEntities(leaderEnemy, this.fightingEnemies, this.transform.forward);
                }

                // Setup for battle
                this.fightingEntities.AddRange(this.fightingPlayers);
                this.fightingEntities.AddRange(this.fightingEnemies);

                this.SetupEntities(this.fightingEntities, true);

                // HUD
                ExploreHud.SetActive(false);
                BattleHud.SetActive(true);

                int playerHealthBarCount = 0;
                int enemyHealthBarCount = 0;

                foreach (BaseBattleDriver battleDriver in this.fightingEntities)
                {
                    battleDriver.OnBattleStart();

                    GameObject healthBar = Instantiate(
                        battleDriver is PlayerBattleDriver ? this.playerHealthBarPrefab : this.enemyHealthBarPrefab,
                        BattleHud.transform);

                    healthBar.GetComponent<UI.HealthController>().battleDriver = battleDriver;

                    healthBar.transform.localPosition += new Vector3(
                        0.0f,
                        (battleDriver is PlayerBattleDriver ? playerHealthBarCount++ : enemyHealthBarCount++) * -30,
                        0.0f);
                }

                this.isBattleActive = true;
            }
        }

        /// <summary>
        ///     Ends a battle on the instance
        /// </summary>
        private void EndBattleModeInstanced()
        {
            if (this.isBattleActive)
            {
                if (this.fightingEntities != null)
                {
                    foreach (BaseBattleDriver battleDriver in this.fightingEntities)
                    {
                        battleDriver.OnBattleEnd();
                    }

                    this.SetupEntities(this.fightingEntities, false);
                }

                if (this.deactivatedGameObjects != null)
                {
                    // Reactivate deactivated GameObjects
                    foreach (GameObject gameObject in this.deactivatedGameObjects)
                    {
                        gameObject.SetActive(true);
                    }

                    this.deactivatedGameObjects = null;
                }

                ExploreHud.SetActive(true);
                BattleHud.SetActive(false);

                for (int i = 0; i < BattleHud.transform.childCount; i++)
                {
                    Transform transform = BattleHud.transform.GetChild(i);

                    if (transform.GetComponent<UI.HealthController>())
                    {
                        Destroy(transform.gameObject);
                    }
                }

                this.isBattleActive = false;
            }
        }

        /// <summary>
        ///     Properly arranges all entities for a battle
        /// </summary>
        /// <param name="leader">The leader</param>
        /// <param name="group">The entire group (may or may not include leader)</param>
        /// <param name="entityForward">Forward vector for the entities</param>
        private void ArrangeEntities(Component leader, Component[] group, Vector3 entityForward)
        {
            Vector3 flatCameraForward = MainCamera.transform.forward;
            flatCameraForward.y = 0.0f;
            flatCameraForward.Normalize();

            bool faceRight = Vector3.SignedAngle(flatCameraForward, entityForward, new Vector3(0.0f, 1.0f, 0.0f)) > 0.0f;

            {
                leader.transform.forward = entityForward;

                Sprite3D.SpriteManager spriteManager = leader.GetComponent<Sprite3D.SpriteManager>();
                if (spriteManager) spriteManager.SetDirection(faceRight);
            }

            int placementOffset = 1;
            bool arrangeAbove = true;
            foreach (Component member in group)
            {
                if (member != leader)
                {
                    member.transform.forward = entityForward;

                    member.transform.position = leader.transform.position + flatCameraForward * placementOffset * (arrangeAbove ? 1.0f : -1.0f);

                    Sprite3D.SpriteManager spriteManager = member.GetComponent<Sprite3D.SpriteManager>();
                    if (spriteManager) spriteManager.SetDirection(faceRight);

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
        private void SetupEntities(IEnumerable<BaseBattleDriver> components, bool start)
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
                        battleDriver.Allies = this.fightingPlayers;
                        battleDriver.Opponents = this.fightingEnemies;
                    }
                    else if (battleDriver is EnemyBattleDriver)
                    {
                        battleDriver.Allies = this.fightingEnemies;
                        battleDriver.Opponents = this.fightingPlayers;
                    }
                }
            }
        }
    }
}