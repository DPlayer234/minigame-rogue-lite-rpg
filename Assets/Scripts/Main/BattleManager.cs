//-----------------------------------------------------------------------
// <copyright file="BattleManager.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main
{
    using System.Collections;
    using System.Collections.Generic;
    using DPlay.RoguePG.Main.BattleDriver;
    using DPlay.RoguePG.Main.Camera;
    using DPlay.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Stores and manages general game state of the battle.
    /// </summary>
    [DisallowMultipleComponent]
    public class BattleManager : Singleton<BattleManager>
    {
        /// <summary> Tag used by Player Entities </summary>
        public const string PlayerEntityTag = "PlayerEntity";

        /// <summary> Tag used by Enemy Entities </summary>
        public const string EnemyEntityTag = "EnemyEntity";

        /// <summary> Tag used by the exploration HUD root </summary>
        public const string ExploreHudTag = "ExploreHud";

        /// <summary> Tag used by the battle HUD root </summary>
        public const string BattleHudTag = "BattleHud";

        /// <summary> The HUD Parent for anything this adds to the HUD </summary>
        private Transform hudParent;

        /// <summary> The current battle status </summary>
        private BattleStatus battleStatus;

        /// <summary> The <seealso cref="BattleFlow"/> for the current battle </summary>
        private BattleFlow battleFlow;

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

        /// <summary> Whether there is a fight going on right now. Probably. </summary>
        public bool Initialized { get; private set; }

        /// <summary>
        ///     Starts a turn-based battle
        /// </summary>
        /// <param name="leaderPlayer">The leading player</param>
        /// <param name="leaderEnemy">The leading enemy</param>
        public static void StartNewBattle(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            if (MainManager.Instance == null) throw new RPGException(RPGException.Cause.MainManagerNoActiveInstance);
            if (BattleManager.Instance != null) return;

            MainManager.Instance.gameObject.AddComponent<BattleManager>().NewPreferThis();

            BattleManager.Instance.StartBattleMode(leaderPlayer, leaderEnemy);
        }

        /// <summary>
        ///     Starts a turn-based battle in this instance
        /// </summary>
        /// <param name="leaderPlayer">The leading player driver</param>
        /// <param name="leaderEnemy">The leading enemy driver</param>
        public void StartBattleMode(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            if (this.Initialized) throw new RPGException(RPGException.Cause.BattleManagerCantStartInitialized);

            this.battleStatus = new BattleStatus(leaderPlayer, leaderEnemy);
            this.battleFlow = new BattleFlow(this, this.battleStatus);

            // Disable activity handling
            ActivityHandler.Enabled = false;

            // Deactivate unneeded GameObjects
            foreach (GameObject gameObject in this.battleStatus.DeactivatableGameObjects)
            {
                gameObject.SetActive(false);
            }

            this.SetupEntities(battleStarts: true);

            BaseBattleDriver.HighestTurnSpeed = this.battleStatus.HighestTurnSpeed;

            this.hudParent = MonoBehaviour.Instantiate(GenericPrefab.Panel, HudManager.BattleHud.transform).transform;

            // Create the status bars (HUD)
            leaderPlayer.CreateStatusBars(this.hudParent, GenericPrefab.StatusDisplayPlayer);
            leaderEnemy.CreateStatusBars(this.hudParent, GenericPrefab.StatusDisplayEnemy);
            
            // Deduplicate names to make them easier to tell apart
            leaderPlayer.DeduplicateBattleNamesInAllies();
            leaderEnemy.DeduplicateBattleNamesInAllies();
            
            // Start the battle on all entities
            foreach (BaseBattleDriver battleDriver in this.battleStatus.AllFightingEntities)
            {
                battleDriver.OnBattleStart();
            }

            this.Initialized = true;
        }

        /// <summary>
        ///     Ends a battle of this instance
        /// </summary>
        public void EndBattleMode()
        {
            if (!this.Initialized) throw new RPGException(RPGException.Cause.BattleManagerCantEndUninitialized);

            foreach (BaseBattleDriver battleDriver in this.battleStatus.AllFightingEntities)
            {
                battleDriver.OnBattleEnd();
            }

            this.SetupEntities(battleStarts: false);

            // Disable activity handling
            ActivityHandler.Enabled = true;

            // Reactivate deactivated GameObjects
            foreach (GameObject gameObject in this.battleStatus.DeactivatableGameObjects)
            {
                gameObject.SetActive(true);
            }

            MonoBehaviour.Destroy(this.hudParent.gameObject);
            MonoBehaviour.Destroy(this);
        }

        /// <summary>
        ///     Called by Unity once per frame to update the <see cref="BattleManager"/>
        /// </summary>
        private void Update()
        {
            if (!this.Initialized) return;

            this.battleFlow.Update();
        }

        /// <summary>
        ///     Sets up entities to either start or end a fight
        /// </summary>
        /// <param name="battleStarts">Whether the fight starts (true) or ends (false)</param>
        private void SetupEntities(bool battleStarts)
        {
            foreach (BaseBattleDriver battleDriver in this.battleStatus.AllFightingEntities)
            {
                var entityDriver = battleDriver.entityDriver;

                entityDriver.enabled = !battleStarts;
                battleDriver.enabled = battleStarts;

                if (battleStarts)
                {
                    if (battleDriver is PlayerBattleDriver)
                    {
                        battleDriver.Allies = this.battleStatus.FightingPlayers;
                        battleDriver.Opponents = this.battleStatus.FightingEnemies;
                    }
                    else if (battleDriver is EnemyBattleDriver)
                    {
                        battleDriver.Allies = this.battleStatus.FightingEnemies;
                        battleDriver.Opponents = this.battleStatus.FightingPlayers;
                    }

                    battleDriver.AlliesAndOpponents = this.battleStatus.StillFightingEntities;
                }
            }
        }

        /// <summary>
        ///     Calls <seealso cref="StartBattleMode(PlayerBattleDriver, EnemyBattleDriver)"/> next frame
        /// </summary>
        /// <param name="leaderPlayer">The leading player</param>
        /// <param name="leaderEnemy">The leading enemy</param>
        /// <returns>An iterator</returns>
        private IEnumerator StartBattleNextFrame(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            yield return null;

            this.StartBattleMode(leaderPlayer, leaderEnemy);
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="BattleManager"/>
        /// </summary>
        private void Awake()
        {
            if (BattleManager.Instance != null && BattleManager.Instance != this)
            {
                Debug.LogWarning("What. Why are there two BattleManagers?");
                MonoBehaviour.Destroy(this);
            }
        }
    }
}
