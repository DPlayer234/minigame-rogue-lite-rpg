using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RoguePG.Main.Driver;
using SAE.RoguePG.Main.BattleActions;

namespace SAE.RoguePG.Main.BattleDriver
{
    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyBattleDriver : BaseBattleDriver
    {
        /// <summary> The <seealso cref="EnemyDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        private EnemyDriver enemyDriver;

        /// <summary>
        ///     To be called when a battle starts
        /// </summary>
        public override void OnBattleStart()
        {
            this.Level = this.enemyDriver.level;
            this.CurrentHealth = this.MaximumHealth;

            base.OnBattleStart();
        }

        /// <summary>
        ///     To be called when a battle ends
        /// </summary>
        public override void OnBattleEnd()
        {
            base.OnBattleEnd();
        }

        /// <summary>
        ///     Sets up everything needed for the Enemy's turn
        /// </summary>
        public override void StartTurn()
        {
            base.StartTurn();
        }

        /// <summary>
        ///     Ends the Enemy's turn
        /// </summary>
        public override void EndTurn()
        {
            base.EndTurn();
        }

        /// <summary>
        ///     Updates the Enemy's turn once a frame
        /// </summary>
        public override void UpdateTurn()
        {
            base.UpdateTurn();
        }

        /// <summary>
        ///     Updates the Enemy once a frame while nothing is taking a turn
        /// </summary>
        public override void UpdateIdle()
        {
            base.UpdateIdle();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyBattleDriver"/> whether it is or is not active.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            this.enemyDriver = this.GetComponent<EnemyDriver>();
        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="BaseBattleDriver"/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyBattleDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {

        }
    }
}
