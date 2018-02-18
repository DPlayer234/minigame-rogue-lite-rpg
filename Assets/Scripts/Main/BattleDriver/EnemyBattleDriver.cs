namespace SAE.RoguePG.Main.BattleDriver
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleAction;
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

        /// <summary> Current Health Value </summary>
        public override int CurrentHealth
        {
            get
            {
                return this.currentHealth;
            }

            set
            {
                this.currentHealth = Mathf.Clamp(value, 0, this.MaximumHealth);

                if (this.currentHealth == 0 && !this.grantedExperience)
                {
                    foreach (BaseBattleDriver battleDriver in this.Opponents)
                    {
                        ++battleDriver.Level;
                    }

                    this.grantedExperience = true;
                }
            }
        }

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
                if (this.isBoss)
                {
                    this.OpenNextFloorEntrance();
                }

                MonoBehaviour.Destroy(this.gameObject);
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
                    var action = this.actions.GetRandomItem();

                    var targets = action.GetTargets();

                    action.Use(targets.GetRandomItem());

                    this.waitTime = 1.0f;
                }
                else
                {
                    this.TakingTurn = false;
                }
            }
        }

        /// <summary>
        ///     Opens the entrance to the next floor
        /// </summary>
        private void OpenNextFloorEntrance()
        {
            Dungeon.DungeonGenerator.CreateFloorTransition();
        }
    }
}
