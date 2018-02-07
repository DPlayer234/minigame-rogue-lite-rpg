namespace SAE.RoguePG.Main.BattleActions
{
    using SAE.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Healing action
    /// </summary>
    public class MinorHeal : BattleAction
    {
        /// <summary> Action Name </summary>
        public const string ActionName = "Minor Heal";

        /// <summary>
        ///     Initializes a new instance of the <see cref="MinorHeal"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public MinorHeal(BaseBattleDriver user) : base(user)
        {
            this.name = ActionName;

            // Storing heal-potential (fraction) in attack power.
            this.attackPower = 0.3f;

            this.attackPointCost = 8.0f;
            this.category = ActionCategory.Support;
            this.targetOption = ActionTargetOption.OneAlly;
        }

        /// <summary>
        ///     The method to run when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void Use(BaseBattleDriver target)
        {
            target.CurrentHealth += (int)(target.MaximumHealth * this.attackPower);
            MinorHeal.DoSmallJump(target);
        }
        
        /// <summary>
        ///     The method to run on the user when this action is being used.
        /// </summary>
        protected override void OnUse()
        {
            MinorHeal.DoSmallJump(this.User);
        }

        /// <summary>
        ///     Does a small jump.
        /// </summary>
        /// <param name="battleDriver">The entity that needs to jump</param>
        private static void DoSmallJump(BaseBattleDriver battleDriver)
        {
            battleDriver.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.5f, 0.0f);
        }
    }
}
