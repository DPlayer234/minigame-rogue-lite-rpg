namespace DPlay.RoguePG.Main.BattleDriver
{
    using System.Collections;
    using System.Collections.Generic;
    using DPlay.RoguePG.Main.BattleAction;
    using DPlay.RoguePG.Main.Driver;
    using DPlay.RoguePG.Main.UI;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerBattleDriver : BaseBattleDriver
    {
        /// <summary>
        ///     The turn taking place.
        /// </summary>
        private PlayerTurn turn;

        /// <summary>
        ///     To be called when a battle starts
        /// </summary>
        public override void OnBattleStart()
        {
            base.OnBattleStart();
        }

        /// <summary>
        ///     To be called when a battle ends
        /// </summary>
        public override void OnBattleEnd()
        {
            base.OnBattleEnd();
            this.ClearTurn();
        }

        /// <summary>
        ///     Sets up everything needed for the Player's turn
        /// </summary>
        public override void StartTurn()
        {
            base.StartTurn();

            if (!this.CanStillFight) return;

            this.turn = new PlayerTurn(this.actions);
        }

        /// <summary>
        ///     Ends the Player's turn
        /// </summary>
        public override void EndTurn()
        {
            base.EndTurn();

            this.ClearTurn();
        }

        /// <summary>
        ///     Updates the Player's turn once a frame
        /// </summary>
        public override void UpdateTurn()
        {
            base.UpdateTurn();

            if (this.AttackPoints <= 0)
            {
                this.ClearTurn();
                this.TakingTurn = false;
            }
        }

        /// <summary>
        ///     Updates the Player once a frame while nothing is taking a turn
        /// </summary>
        public override void UpdateIdle()
        {
            base.UpdateIdle();

            if (!this.CanStillFight) return;
        }

        /// <summary>
        ///     Clears actions taken by the active <seealso cref="PlayerTurn"/>
        /// </summary>
        private void ClearTurn()
        {
            if (this.turn != null)
            {
                this.turn.DestroyActionButtons();
            }
        }
    }
}
