//-----------------------------------------------------------------------
// <copyright file="MajorHeal.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using DPlay.RoguePG.Main.BattleDriver;

    /// <summary>
    ///     Healing action that fully recovers HP
    /// </summary>
    public class MajorHeal : HealAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MajorHeal"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public MajorHeal(BaseBattleDriver user) : base(user)
        {
            this.name = "Major Heal";
            this.description = "Fully recovers an allies health.";

            // Storing heal-potential (fraction) in attack power.
            this.attackPower = 1.00f;

            this.attackPointCost = 14.0f;
            this.category = ActionCategory.Support;
            this.targetOption = ActionTargetOption.OneAlly;
        }
    }
}
