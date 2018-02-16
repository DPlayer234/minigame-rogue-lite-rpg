namespace SAE.RoguePG.Main.BattleAction.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SAE.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Healing action that fully recovers HP
    /// </summary>
    public class MajorHeal : HealAction
    {
        /// <summary> Action Name </summary>
        public const string ActionName = "Major Heal";

        /// <summary>
        ///     Initializes a new instance of the <see cref="MajorHeal"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public MajorHeal(BaseBattleDriver user) : base(user)
        {
            this.name = ActionName;

            // Storing heal-potential (fraction) in attack power.
            this.attackPower = 1.00f;

            this.attackPointCost = 14.0f;
            this.category = ActionCategory.Support;
            this.targetOption = ActionTargetOption.OneAlly;
        }
    }
}
