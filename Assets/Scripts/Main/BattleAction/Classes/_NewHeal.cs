namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DPlay.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Template for any healing action
    /// </summary>
    public class _NewHeal : HealAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="_NewHeal"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public _NewHeal(BaseBattleDriver user) : base(user)
        {
            this.name = "ACTIONNAME";
            this.description = "ACTIONDESCRIPTION";

            // Storing heal-potential (fraction) in attack power.
            this.attackPower = 0.25f;

            this.attackPointCost = 6.0f;
            this.category = ActionCategory.Support;
            this.targetOption = ActionTargetOption.OneAlly;
        }
    }
}
