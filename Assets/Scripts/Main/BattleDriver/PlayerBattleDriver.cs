using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RoguePG.Main.Driver;

namespace SAE.RoguePG.Main.BattleDriver
{
    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerBattleDriver : BaseBattleDriver
    {
        /// <summary> The <seealso cref="PlayerDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        private PlayerDriver playerDriver;

        /// <summary>
        ///     To be called when a battle starts
        /// </summary>
        public override void OnBattleStart()
        {
            this.Level = this.playerDriver.level;
            this.CurrentHealth = this.playerDriver.currentHealth;

            base.OnBattleStart();
        }

        /// <summary>
        ///     To be called when a battle ends
        /// </summary>
        public override void OnBattleEnd()
        {
            base.OnBattleEnd();

            this.playerDriver.level = this.Level;
            this.playerDriver.currentHealth = this.CurrentHealth;
        }

        /// <summary>
        ///     Sets up everything needed for the Player's turn
        /// </summary>
        public override void StartTurn()
        {
            base.StartTurn();
        }

        /// <summary>
        ///     Ends the Player's turn
        /// </summary>
        public override void EndTurn()
        {
            base.EndTurn();
        }

        /// <summary>
        ///     Updates the Player's turn once a frame
        /// </summary>
        public override void UpdateTurn()
        {
            base.UpdateTurn();
        }

        /// <summary>
        ///     Updates the Player once a frame while nothing is taking a turn
        /// </summary>
        public override void UpdateIdle()
        {
            base.UpdateIdle();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> whether it is or is not active.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            this.playerDriver = this.GetComponent<PlayerDriver>();
        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="PlayerBattleDriver"/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {

        }
    }
}
