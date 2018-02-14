namespace SAE.RoguePG.Main.BattleAction.Actions
{
    using SAE.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Simple physical attack
    /// </summary>
    public class Smash : ChargeAction
    {
        /// <summary> Action Name </summary>
        public const string ActionName = "Smash";

        /// <summary> Multiplier for the attack velocity </summary>
        public const float VelocityMultiplier = 10.0f;

        /// <summary> Amount of Velocity added in Y-direction for attack </summary>
        public const float YVelocity = 2.0f;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Smash"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public Smash(BaseBattleDriver user) : base(user)
        {
            this.name = ActionName;

            this.attackPointCost = 4.0f;
            this.attackPower = 10.0f;
            this.category = ActionCategory.PhysicalAttack;
            this.targetOption = ActionTargetOption.OneOpponent;
        }

        /// <summary>
        ///     The method to run when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void Use(BaseBattleDriver target)
        {
            this.DealDamage(target);

            this.User.StartCoroutine(this.DoCharge(target));
        }
    }
}
