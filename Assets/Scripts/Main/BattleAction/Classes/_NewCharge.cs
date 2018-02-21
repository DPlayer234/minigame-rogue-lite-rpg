//-----------------------------------------------------------------------
// <copyright file="_NewCharge.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using DPlay.RoguePG.Main.BattleDriver;

    /// <summary>
    ///     Template for any charge action
    /// </summary>
    public class _NewCharge : ChargeAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="_NewCharge"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public _NewCharge(BaseBattleDriver user) : base(user)
        {
            this.name = "ACTIONNAME";
            this.description = "ACTIONDESCRIPTION";

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
