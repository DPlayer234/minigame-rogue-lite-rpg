namespace SAE.RoguePG.Main.BattleDriver
{
    using SAE.RoguePG.Main.BattleActions;
    using SAE.RoguePG.Main.Driver;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

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

            if (!this.waitingForAnimation)
            {
                // Random moves for now
                if (this.AttackPoints > 0.0f)
                {
                    var action = this.actions[Random.Range(0, this.actions.Length)];

                    var targets = action.GetTargets();

                    action.Use(targets[Random.Range(0, targets.Length)]);

                    StartCoroutine(this.JumpForward());
                }
                else
                {
                    this.TakingTurn = false;
                }
            }
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
