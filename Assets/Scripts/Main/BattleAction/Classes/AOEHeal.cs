namespace SAE.RoguePG.Main.BattleAction.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SAE.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Healing action which heals all allies
    /// </summary>
    public class AOEHeal : HealAction
    {
        /// <summary> Action Name </summary>
        public const string ActionName = "Area Heal";

        /// <summary>
        ///     Initializes a new instance of the <see cref="AOEHeal"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public AOEHeal(BaseBattleDriver user) : base(user)
        {
            this.name = ActionName;

            // Storing heal-potential (fraction) in attack power.
            this.attackPower = 0.25f;

            this.attackPointCost = 10.0f;
            this.category = ActionCategory.Support;
            this.targetOption = ActionTargetOption.AllAllies;
        }
    }
}
