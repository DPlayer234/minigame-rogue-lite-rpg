using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAE.RoguePG.Main.BattleDriver;

namespace SAE.RoguePG.Main.BattleActions
{
    /// <summary>
    ///     Simple physical attack
    /// </summary>
    public class Smash : BattleAction
    {
        /// <summary> Action Name </summary>
        public const string ActionName = "Smash";

        /// <summary>
        ///     Initializes a new instance of the <see cref="Smash"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public Smash(BaseBattleDriver user) : base(user)
        {
            this.name = ActionName;

            this.attackPointCost = 4.0f;
            this.attackPower = 1.5f;
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
        }
    }
}
