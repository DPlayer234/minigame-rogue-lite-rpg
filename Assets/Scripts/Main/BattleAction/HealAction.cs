namespace DPlay.RoguePG.Main.BattleAction
{
    using DPlay.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Base class for any action that heals an ally.
    /// </summary>
    public abstract class HealAction : BattleAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HealAction"/> class.
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public HealAction(BaseBattleDriver user) : base(user) { }
        
        /// <summary>
        ///     Does a small jump.
        /// </summary>
        /// <param name="battleDriver">The entity that needs to jump</param>
        protected static void DoSmallJump(BaseBattleDriver battleDriver)
        {
            battleDriver.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.5f, 0.0f);
        }

        /// <summary>
        ///     The method to run when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void Use(BaseBattleDriver target)
        {
            target.CurrentHealth += (int)(target.MaximumHealth * this.attackPower);
            HealAction.DoSmallJump(target);
        }

        /// <summary>
        ///     The method to run on the user when this action is being used.
        /// </summary>
        protected override void OnUse()
        {
            HealAction.DoSmallJump(this.User);
        }
    }
}
