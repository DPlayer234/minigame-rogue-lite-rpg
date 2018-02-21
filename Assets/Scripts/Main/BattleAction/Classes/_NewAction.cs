//-----------------------------------------------------------------------
// <copyright file="_NewAction.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using DPlay.RoguePG.Main.BattleDriver;

    /// <summary>
    ///     Template for any action
    /// </summary>
    public class _NewAction : BattleAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="_NewAction"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public _NewAction(BaseBattleDriver user) : base(user)
        {
            this.name = "ACTIONNAME";
            this.description = "ACTIONDESCRIPTION";

            this.attackPointCost = 10.0f;
            this.attackPower = 10.0f;
            this.category = ActionCategory.Undefined;
            this.targetOption = ActionTargetOption.OneOpponent;
        }

        /// <summary>
        ///     The method to run when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void Use(BaseBattleDriver target)
        {
            // Code here
        }
    }
}
