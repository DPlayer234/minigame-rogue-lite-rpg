namespace SAE.RoguePG.Main
{
    using System;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Manages the flow of a battle.
    /// </summary>
    public class BattleFlow
    {
        /// <summary> The <seealso cref="BattleManager"/> depending on this instance </summary>
        private BattleManager battleManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BattleFlow"/> class.
        /// </summary>
        /// <param name="battleManager">The <seealso cref="BattleManager"/> depending on this instance</param>
        public BattleFlow(BattleManager battleManager, BattleStatus battleStatus)
        {
            this.battleManager = battleManager;
            this.BattleStatus = battleStatus;
        }

        /// <summary> The current battle status </summary>
        public BattleStatus BattleStatus { get; private set; }

        /// <summary>
        ///     Updates the battle once a frame.
        /// </summary>
        public void Update()
        {
            if (this.BattleStatus.CurrentTurnOf == null)
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
            float overshoot = this.UpdateIdlingFighters();
            this.FindNextTurnOf(overshoot);
        }

        /// <summary>
        ///     Updates the idling fighters and returns the AP overshoot.
        /// </summary>
        /// <returns>The AP overshoot</returns>
        private float UpdateIdlingFighters()
        {
            float overshoot = 0.0f;

            foreach (BaseBattleDriver battleDriver in this.BattleStatus.StillFightingEntities)
            {
                try
                {
                    battleDriver.UpdateIdle();
                }
                catch (Exception e)
                {
                    // We don't want any errors to interupt the program flow any more,
                    // but we don't want them to be ignored either.
                    Debug.LogError(e);
                }

                if (battleDriver.CanStillFight)
                {
                    overshoot = Mathf.Max(overshoot, battleDriver.AttackPoints - BaseBattleDriver.MaximumAttackPoints);
                }
                else
                {
                    battleDriver.AttackPoints = Mathf.Min(battleDriver.AttackPoints, 0.0f);
                }
            }

            return overshoot;
        }

        /// <summary>
        ///     Finds and assigns the entity to take a turn next.
        ///     Will do nothing if <paramref name="apOvershoot"/> is 0 or less.
        /// </summary>
        /// <param name="apOvershoot">How much too high did the Attack Points go?</param>
        private void FindNextTurnOf(float apOvershoot)
        {
            if (apOvershoot < 0.0f) return;

            // Makes sure that not multiple Entities are initialized as taking a turn
            bool foundNextTurn = false;

            foreach (BaseBattleDriver battleDriver in this.BattleStatus.StillFightingEntities)
            {
                // Reduce
                battleDriver.AttackPoints -= apOvershoot;

                if (!foundNextTurn && battleDriver.AttackPoints == BaseBattleDriver.MaximumAttackPoints)
                {
                    // This was it: Next Turn of this thing here...!
                    battleDriver.TakingTurn = true;
                    this.BattleStatus.CurrentTurnOf = battleDriver;

                    foundNextTurn = true;
                }
            }
        }

        /// <summary>
        ///     Updates the battle's turn-phase once a frame while something is taking a turn
        /// </summary>
        private void UpdateBattleTurn()
        {
            if (this.BattleStatus.CurrentTurnOf != null)
            {
                // Ended turn
                if (!this.BattleStatus.CurrentTurnOf.TakingTurn)
                {
                    this.BattleStatus.CurrentTurnOf = null;

                    this.CheckAndUpdateBattleStatus();
                    return;
                }

                try
                {
                    if (this.BattleStatus.CurrentTurnOf.CanStillFight)
                    {
                        this.BattleStatus.CurrentTurnOf.UpdateTurn();
                    }
                    else
                    {
                        this.BattleStatus.CurrentTurnOf.TakingTurn = false;
                    }
                }
                catch (System.Exception e)
                {
                    // We don't want any errors to interupt the program flow any more,
                    // but we don't want the error to be ignored completely either.
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
            foreach (BaseBattleDriver playerEntity in this.BattleStatus.FightingPlayers)
            {
                playersAlive |= playerEntity.CanStillFight;
            }

            bool enemiesAlive = false;
            foreach (BaseBattleDriver enemyEntity in this.BattleStatus.FightingEnemies)
            {
                enemiesAlive |= enemyEntity.CanStillFight;
            }
            
            if (!playersAlive || !enemiesAlive)
            {
                // Either party has been knocked entirely
                this.battleManager.EndBattleMode();
            }

            if (!playersAlive)
            {
                // Players have been defeated. Loss.
                this.TriggerGameOver();
            }
        }

        /// <summary>
        ///     Sets the game over status.
        /// </summary>
        private void TriggerGameOver()
        {
            Debug.Log("<b>!! GAME OVER !!</b>");

            GameOverHandler.SetGameOver();
        }
    }
}