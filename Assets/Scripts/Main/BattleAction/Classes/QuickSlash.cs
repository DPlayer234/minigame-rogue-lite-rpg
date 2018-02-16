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
    ///     Stronger physical attack
    /// </summary>
    public class QuickSlash : ChargeAction
    {
        /// <summary> Action Name </summary>
        public const string ActionName = "Quick Slash";

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuickSlash"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public QuickSlash(BaseBattleDriver user) : base(user)
        {
            this.name = ActionName;

            this.attackPointCost = 7.0f;
            this.attackPower = 14.0f;
            this.category = ActionCategory.PhysicalAttack;
            this.targetOption = ActionTargetOption.OneOpponent;
        }
    }
}
