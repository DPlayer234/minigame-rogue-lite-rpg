//-----------------------------------------------------------------------
// <copyright file="Smash.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using DPlay.RoguePG.Main.BattleDriver;

    /// <summary>
    ///     Simple physical attack
    /// </summary>
    public class Smash : ChargeAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Smash"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public Smash(BaseBattleDriver user) : base(user)
        {
            this.name = "Smash";
            this.description = "Ram into an enemy to deal some damage.";

            this.attackPointCost = 4.0f;
            this.attackPower = 10.0f;
            this.category = ActionCategory.PhysicalAttack;
            this.targetOption = ActionTargetOption.OneOpponent;
        }
    }
}
