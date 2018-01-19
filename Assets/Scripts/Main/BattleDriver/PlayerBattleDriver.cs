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
    public class PlayerBattleDriver : EntityBattleDriver
    {
        /// <summary> The <seealso cref="PlayerDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        private PlayerDriver playerDriver;

        /// <summary>
        ///     Sets up everything needed for the Player's turn
        /// </summary>
        protected override void StartTurn()
        {

        }

        /// <summary>
        ///     Ends the Player's turn
        /// </summary>
        protected override void EndTurn()
        {

        }

        /// <summary>
        ///     Updates the Player's turn once a frame
        /// </summary>
        protected override void UpdateTurn()
        {

        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> whether it is or is not active.
        /// </summary>
        new private void Awake()
        {
            base.Awake();

            this.playerDriver = this.GetComponent<PlayerDriver>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="PlayerBattleDriver"/>
        /// </summary>
        new private void Update()
        {
            base.Update();
        }
    }
}
