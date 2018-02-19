namespace SAE.RoguePG.Main
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Stores and manages general game state of the Main scene.
    ///     Behaves like a singleton; any new instance will override the old one.
    /// </summary>
    [DisallowMultipleComponent]
    public class BattleManager : MonoBehaviour
    {
        /// <summary> Tag used by Player Entities </summary>
        public const string PlayerEntityTag = "PlayerEntity";

        /// <summary> Tag used by Enemy Entities </summary>
        public const string EnemyEntityTag = "EnemyEntity";

        /// <summary> Tag used by the exploration HUD root </summary>
        public const string ExploreHudTag = "ExploreHud";

        /// <summary> Tag used by the battle HUD root </summary>
        public const string BattleHudTag = "BattleHud";

        /// <summary> Whether there is a fight going on right now. Probably. </summary>
        private bool initialized = false;

        /// <summary> The Entity whose turn it currently is. </summary>
        private BaseBattleDriver currentTurnOf;

        /// <summary> All Entities taking part in the fight </summary>
        private List<BaseBattleDriver> allFightingEntities;

        /// <summary> All Entities which are still fighting </summary>
        private List<BaseBattleDriver> stillFightingEntities;

        /// <summary> All Players currently fighting </summary>
        private List<BaseBattleDriver> fightingPlayers;

        /// <summary> All Enemies currently fighting </summary>
        private List<BaseBattleDriver> fightingEnemies;

        /// <summary> All GameObjects that were disabled to make room for the fighters </summary>
        private List<GameObject> deactivatedGameObjects;

        /// <summary> The HUD Parent for anything this adds to the HUD </summary>
        private Transform hudParent;

        /// <summary>
        ///     The global instance of the <see cref="BattleManager"/>.
        /// </summary>
        public static BattleManager Instance { get; private set; }

        /// <summary>
        ///     The Entity whose turn it currently is.
        /// </summary>
        public static BaseBattleDriver CurrentTurnOf { get { return Instance.currentTurnOf; } }

        /// <summary>
        ///     Whether a battle is active
        /// </summary>
        public static bool IsBattleActive
        {
            get
            {
                return BattleManager.Instance != null;
            }
        }

        /// <summary>
        ///     Starts a turn-based battle
        /// </summary>
        /// <param name="leaderPlayer">The leading player</param>
        /// <param name="leaderEnemy">The leading enemy</param>
        public static void StartBattleMode(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            if (MainManager.Instance == null) throw new RPGException(RPGException.Cause.MainManagerNoActiveInstance);
            if (BattleManager.Instance != null) return;

            BattleManager.Instance = MainManager.Instance.gameObject.AddComponent<BattleManager>();

            BattleManager.Instance.StartCoroutine(BattleManager.Instance.StartBattleNextFrame(leaderPlayer, leaderEnemy));
        }

        /// <summary>
        ///     Starts a turn-based battle
        /// </summary>
        public static void EndBattleMode()
        {
            if (BattleManager.Instance == null) throw new RPGException(RPGException.Cause.BattleManagerNoActiveInstance);

            BattleManager.Instance.EndBattleModeInstanced();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="BattleManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
        }

        /// <summary>
        ///     Called by Unity once per frame to update the <see cref="BattleManager"/>
        /// </summary>
        private void Update()
        {
            if (!this.initialized) return;

            this.UpdateBattle();
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
            foreach (BaseBattleDriver battleDriver in this.stillFightingEntities)
            {
                try
                {
                    battleDriver.UpdateIdle();
                }
                catch (System.Exception e)
                {
                    // We don't want any errors to interupt the program flow any more,
                    // but we don't want them to be ignored either.
                    Debug.LogError(e);
                }

                if (battleDriver.CanStillFight)
                {
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

                foreach (BaseBattleDriver battleDriver in this.stillFightingEntities)
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
                // Ended turn
                if (!this.currentTurnOf.TakingTurn)
                {
                    this.currentTurnOf = null;

                    this.CheckAndUpdateBattleStatus();
                    return;
                }

                try
                {
                    if (this.currentTurnOf.CanStillFight)
                    {
                        this.currentTurnOf.UpdateTurn();
                    }
                    else
                    {
                        this.currentTurnOf.TakingTurn = false;
                    }
                }
                catch (System.Exception e)
                {
                    // We don't want any errors to interupt the program flow any more,
                    // but we don't want them to be ignored either.
                    Debug.LogError(e);
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
                BattleManager.EndBattleMode();

                if (!playersAlive)
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

        /// <summary>
        ///     Sets the game over status.
        /// </summary>
        private void DoGameOver()
        {
            Debug.LogError("<b>!! GAME OVER !!</b>");

            foreach (var playerEntity in this.fightingPlayers)
            {
                MonoBehaviour.Destroy(playerEntity.gameObject);
            }
        }

        /// <summary>
        ///     Starts a turn-based battle on the instance
        /// </summary>
        /// <param name="leaderPlayer">The leading player driver</param>
        /// <param name="leaderEnemy">The leading enemy driver</param>
        private void StartBattleModeInstanced(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            this.deactivatedGameObjects = new List<GameObject>();
            
            this.FindFightingPlayers(leaderPlayer);
            this.FindFightingEnemies(leaderEnemy);
            this.AssignFightingEntities();

            this.SetupEntities(battleStarts: true);

            BaseBattleDriver.HighestTurnSpeed = this.GetHighestTurnSpeed();

            foreach (BaseBattleDriver battleDriver in this.allFightingEntities)
            {
                battleDriver.OnBattleStart();
            }

            this.hudParent = MonoBehaviour.Instantiate(GenericPrefab.Panel, HudManager.BattleHud.transform).transform;

            leaderPlayer.CreateStatusBars(this.hudParent, GenericPrefab.StatusDisplayPlayer);
            leaderEnemy.CreateStatusBars(this.hudParent, GenericPrefab.StatusDisplayEnemy);

            leaderPlayer.DeduplicateBattleNamesInAllies();
            leaderEnemy.DeduplicateBattleNamesInAllies();

            this.initialized = true;
        }

        /// <summary>
        ///     Ends a battle on the instance
        /// </summary>
        private void EndBattleModeInstanced()
        {
            if (this.allFightingEntities != null)
            {
                foreach (BaseBattleDriver battleDriver in this.allFightingEntities)
                {
                    battleDriver.OnBattleEnd();
                }

                this.SetupEntities(battleStarts: false);
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

            MonoBehaviour.Destroy(this.hudParent.gameObject);
            MonoBehaviour.Destroy(this);
        }

        /// <summary>
        ///     Gets all player fighters and deactivates the ones not participating.
        ///     Assigns <seealso cref="fightingPlayers"/>.
        /// </summary>
        /// <param name="leaderPlayer">The leader player</param>
        private void FindFightingPlayers(PlayerBattleDriver leaderPlayer)
        {
            this.fightingPlayers = this.FindFighters<PlayerBattleDriver, PlayerDriver>(leaderPlayer, BattleManager.PlayerEntityTag);
        }

        /// <summary>
        ///     Gets all enemy fighters and deactivates the ones not participating.
        ///     Assigns <seealso cref="fightingEnemies"/>.
        /// </summary>
        /// <param name="leaderEnemy">The leader enemy</param>
        private void FindFightingEnemies(EnemyBattleDriver leaderEnemy)
        {
            this.fightingEnemies = this.FindFighters<EnemyBattleDriver, EnemyDriver>(leaderEnemy, BattleManager.EnemyEntityTag);
        }

        /// <summary>
        ///     Finds all fighters of type <typeparamref name="TBattleDriver"/> and deactivates the ones not participating
        /// </summary>
        /// <typeparam name="TBattleDriver">The battle driver type</typeparam>
        /// <typeparam name="TDriver">The regular driver type</typeparam>
        /// <param name="leaderBattleDriver">The leader of the bunch</param>
        /// <param name="tag">The entity tag associated</param>
        /// <returns>A list of <seealso cref="BaseBattleDriver"/>s</returns>
        private List<BaseBattleDriver> FindFighters<TBattleDriver, TDriver>(TBattleDriver leaderBattleDriver, string tag)
            where TBattleDriver : BaseBattleDriver
            where TDriver : BaseDriver
        {
            // Find all GameObjects with that tag
            GameObject[] allFighters = GameObject.FindGameObjectsWithTag(tag);

            List<BaseBattleDriver> listOfFighters = new List<BaseBattleDriver>();

            TDriver leaderDriver = leaderBattleDriver.GetComponent<TDriver>();

            // Check each of them
            foreach (GameObject fighter in allFighters)
            {
                TDriver driver = fighter.GetComponent<TDriver>();

                if (driver != null && (driver.Leader == leaderDriver || driver == leaderDriver))
                {
                    // Required component; should never be missing
                    listOfFighters.Add(fighter.GetComponent<TBattleDriver>());
                }
                else
                {
                    fighter.SetActive(false);
                    this.deactivatedGameObjects.Add(fighter);
                }
            }

            return listOfFighters;
        }

        /// <summary>
        ///     Assigns <seealso cref="allFightingEntities"/> using <seealso cref="fightingPlayers"/> and <seealso cref="fightingEnemies"/>
        /// </summary>
        private void AssignFightingEntities()
        {
            this.allFightingEntities = new List<BaseBattleDriver>();
            this.stillFightingEntities = new List<BaseBattleDriver>();

            // Setup for battle
            // For some reason, conversion of 'compatible' lists won't work, but it works with arrays.
            this.allFightingEntities.AddRange(this.fightingPlayers);
            this.allFightingEntities.AddRange(this.fightingEnemies);

            // Copy
            this.stillFightingEntities.AddRange(this.allFightingEntities);
        }

        /// <summary>
        ///     Sets up entities to either start or end a fight
        /// </summary>
        /// <param name="battleStarts">Whether the fight starts (true) or ends (false)</param>
        private void SetupEntities(bool battleStarts)
        {
            foreach (BaseBattleDriver battleDriver in this.allFightingEntities)
            {
                var entityDriver = battleDriver.entityDriver;

                entityDriver.enabled = !battleStarts;
                battleDriver.enabled = battleStarts;

                if (battleStarts)
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

                    battleDriver.AlliesAndOpponents = this.stillFightingEntities;
                }
            }
        }

        /// <summary>
        ///     Returns the highest turn speed within all fighting entities.
        /// </summary>
        /// <returns>The highest turn speed</returns>
        private float GetHighestTurnSpeed()
        {
            float turnSpeed = 0;
            foreach (BaseBattleDriver battleDriver in this.allFightingEntities)
            {
                turnSpeed = Mathf.Max(battleDriver.TurnSpeed, turnSpeed);
            }

            return turnSpeed;
        }

        /// <summary>
        ///     Calls <seealso cref="StartBattleModeInstanced(PlayerBattleDriver, EnemyBattleDriver)"/> next frame
        /// </summary>
        /// <param name="leaderPlayer">The leading player</param>
        /// <param name="leaderEnemy">The leading enemy</param>
        /// <returns>An iterator</returns>
        private IEnumerator StartBattleNextFrame(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            yield return null;

            this.StartBattleModeInstanced(leaderPlayer, leaderEnemy);
        }
    }
}
