namespace SAE.RoguePG.Main.BattleDriver
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleActions;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyBattleDriver : BaseBattleDriver
    {
        /// <summary> Whether this enemy is a boss mob </summary>
        [SerializeField]
        private bool isBoss = false;

        /// <summary> Whether this has already granted experience/levels </summary>
        private bool grantedExperience = false;

        /// <summary> How long to wait before taking the next action </summary>
        private float waitTime = 0.0f;

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

            if (!this.CanStillFight)
            {
                MonoBehaviour.Destroy(this.gameObject);

                if (this.isBoss)
                {
                    this.OpenNextFloorEntrance();
                }
            }
        }

        /// <summary>
        ///     Sets up everything needed for the Enemy's turn
        /// </summary>
        public override void StartTurn()
        {
            base.StartTurn();

            this.waitTime = 1.0f;
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

            this.waitTime -= Time.deltaTime;

            if (this.waitTime < 0.0f && !this.IsWaitingOnAnimation)
            {
                // Random moves for now
                if (this.AttackPoints > 0.0f)
                {
                    var action = this.actions[Random.Range(0, this.actions.Length)];

                    var targets = action.GetTargets();

                    action.Use(targets[Random.Range(0, targets.Length)]);

                    this.waitTime = 1.0f;
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
            if (this.CanStillFight)
            {
                base.UpdateIdle();
            }
            else
            {
                // Grants XP/Levels upon defeat
                if (!this.grantedExperience)
                {
                    this.gameObject.SetActive(false);

                    foreach (BaseBattleDriver battleDriver in this.Opponents)
                    {
                        ++battleDriver.Level;
                    }

                    this.grantedExperience = true;
                }
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyBattleDriver"/> whether it is or is not active.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="BaseBattleDriver"/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        ///     Opens the entrance to the next floor
        /// </summary>
        private void OpenNextFloorEntrance()
        {

        }
    }
}
