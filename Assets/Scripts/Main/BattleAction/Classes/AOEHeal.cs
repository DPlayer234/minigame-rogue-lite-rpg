namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DPlay.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Healing action which heals all allies
    /// </summary>
    public class AOEHeal : HealAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AOEHeal"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public AOEHeal(BaseBattleDriver user) : base(user)
        {
            this.name = "Area Heal";
            this.description = "Heals all allies for 25% of their Maximum Health.";

            // Storing heal-potential (fraction) in attack power.
            this.attackPower = 0.25f;

            this.attackPointCost = 10.0f;
            this.category = ActionCategory.Support;
            this.targetOption = ActionTargetOption.AllAllies;
        }
    }
}
