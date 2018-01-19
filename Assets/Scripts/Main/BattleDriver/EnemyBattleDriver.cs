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
    public class EnemyBattleDriver : EntityBattleDriver
    {
        /// <summary> The <seealso cref="EnemyDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        private EnemyDriver enemyDriver;

        /// <summary>
        ///     Sets up everything needed for the Enemy's turn
        /// </summary>
        protected override void StartTurn()
        {

        }

        /// <summary>
        ///     Ends the Enemy's turn
        /// </summary>
        protected override void EndTurn()
        {

        }

        /// <summary>
        ///     Updates the Enemy's turn once a frame
        /// </summary>
        protected override void UpdateTurn()
        {

        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyBattleDriver"/> whether it is or is not active.
        /// </summary>
        new private void Awake()
        {
            base.Awake();

            this.enemyDriver = this.GetComponent<EnemyDriver>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyBattleDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="EntityBattleDriver"/>
        /// </summary>
        new private void Update()
        {
            base.Update();
        }
    }
}
