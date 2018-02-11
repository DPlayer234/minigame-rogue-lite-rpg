namespace SAE.RoguePG.Main.BattleActions
{
    using SAE.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Base class for any action that can be taken during a turn.
    /// </summary>
    public abstract partial class BattleAction
    {
        /// <summary> See: <see cref="Name"/> </summary>
        protected string name;

        /// <summary> See: <see cref="AttackPower"/> </summary>
        protected float attackPower;

        /// <summary> See: <see cref="AttackPointCost"/> </summary>
        protected float attackPointCost;

        /// <summary> See: <see cref="TargetOption"/> </summary>
        protected ActionTargetOption targetOption;

        /// <summary> See: <see cref="Category"/> </summary>
        protected ActionCategory category;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BattleAction"/> class.
        ///     Since this is an abstract class, it only serves as a base.
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public BattleAction(BaseBattleDriver user)
        {
            this.User = user;
        }

        /// <summary>
        ///     Which options are there for using this action?
        /// </summary>
        public enum ActionTargetOption
        {
            /// <summary> Can only be used on one-self </summary>
            Self,

            /// <summary> Can be used on anybody </summary>
            Anyone,

            /// <summary> Can be used on a single ally </summary>
            OneAlly,

            /// <summary> Can be used on a single opponent </summary>
            OneOpponent,

            /// <summary> Will be used on everybody at once </summary>
            Everybody,

            /// <summary> Will be used on all allies at once </summary>
            AllAllies,

            /// <summary> Will be used on all opponents at once </summary>
            AllOpponents,
        }

        /// <summary>
        ///     Which Category does this action belong in?
        /// </summary>
        public enum ActionCategory
        {
            /// <summary> Physical attack. Uses <seealso cref="BaseBattleDriver.PhysicalDamage"/> for damage calculation. </summary>
            PhysicalAttack,

            /// <summary> Magical attack. Uses <seealso cref="BaseBattleDriver.MagicalDamage"/> for damage calculation. </summary>
            MagicalAttack,

            /// <summary> Support action, f.e. healing. </summary>
            Support,

            /// <summary> The action is not defined to be in any other category. </summary>
            Undefined
        }

        /// <summary> The name of this action </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        ///     The attack power of this attack.
        ///     Irrelevant if it deals no damage (does not call <seealso cref="DealDamage(BaseBattleDriver)"/>)
        /// </summary>
        public float AttackPower { get { return this.attackPower; } }

        /// <summary> How many attack points using this action costs </summary>
        public float AttackPointCost { get { return this.attackPointCost; } }

        /// <summary> Which target choices are there? </summary>
        public ActionTargetOption TargetOption { get { return this.targetOption; } }

        /// <summary> Which category does this action belong in? </summary>
        public ActionCategory Category { get { return this.category; } }

        /// <summary> Who is using this action? </summary>
        public BaseBattleDriver User { get; private set; }

        /// <summary>
        ///     Use this action against a set list of targets
        /// </summary>
        /// <param name="targets">A list of targets</param>
        public void Use(BaseBattleDriver[] targets)
        {
            Debug.LogFormat("{0} is using {1}!", this.User.name, this.Name);

            this.User.AttackPoints -= this.AttackPointCost;

            foreach (BaseBattleDriver target in targets)
            {
                this.Use(target);
            }
        }

        /// <summary>
        ///     Returns an array of BattleDriver-Arrays, each of which represent an option
        /// </summary>
        /// <returns>A array of choices which can be passed to <seealso cref="Use(BaseBattleDriver[])"/></returns>
        public BaseBattleDriver[][] GetTargets()
        {
            switch (this.TargetOption)
            {
                case ActionTargetOption.Anyone:
                    return VariousCommon.SplitIntoArrayOfLenghtOneArrays(this.User.AlliesAndOpponents);

                case ActionTargetOption.OneAlly:
                    return VariousCommon.SplitIntoArrayOfLenghtOneArrays(this.User.Allies);

                case ActionTargetOption.OneOpponent:
                    return VariousCommon.SplitIntoArrayOfLenghtOneArrays(this.User.Opponents);

                case ActionTargetOption.Everybody:
                    return new BaseBattleDriver[][]
                    {
                        this.User.AlliesAndOpponents
                    };

                case ActionTargetOption.AllAllies:
                    return new BaseBattleDriver[][]
                    {
                        this.User.Allies
                    };

                case ActionTargetOption.AllOpponents:
                    return new BaseBattleDriver[][]
                    {
                        this.User.Opponents
                    };

                default:
                    // No case TargetChoice.Self; therefore also called for that
                    return new BaseBattleDriver[][]
                    {
                        new BaseBattleDriver[] { this.User }
                    };
            }
        }

        /// <summary>
        ///     Deal damage respecting the damage formula.
        /// </summary>
        /// <param name="target">The defending BattleDriver</param>
        /// <returns>The amount of damage dealt</returns>
        public int DealDamage(BaseBattleDriver target)
        {
            float damageStat;

            switch (this.Category)
            {
                case ActionCategory.PhysicalAttack:
                    damageStat = this.User.PhysicalDamage;
                    break;

                case ActionCategory.MagicalAttack:
                    damageStat = this.User.MagicalDamage;
                    break;

                default:
                    Debug.LogWarning("Attempting to deal damage with BattleAction of category " + this.Category.ToString());
                    damageStat = 0.0f;
                    break;
            }
            
            int damageValue = (int)(Mathf.Max(
                0.0f,
                (BaseBattleDriver.LevelStatOffset + this.User.Level) * this.AttackPower * damageStat / target.Defense));
            target.CurrentHealth -= damageValue;
            return damageValue;
        }

        /// <summary>
        ///     Returns the label for a target selection
        /// </summary>
        /// <param name="targetChoice">The target selection</param>
        /// <returns>A fitting label</returns>
        public string GetTargetLabel(BaseBattleDriver[] targetChoice)
        {
            switch (this.targetOption)
            {
                case ActionTargetOption.Self:
                    return "Self";

                case ActionTargetOption.Anyone:
                case ActionTargetOption.OneAlly:
                case ActionTargetOption.OneOpponent:
                    return targetChoice[0].battleName;

                case ActionTargetOption.Everybody:
                    return "Everybody";

                case ActionTargetOption.AllAllies:
                    return "All Allies";

                case ActionTargetOption.AllOpponents:
                    return "All Opponents";

                default:
                    return "IDK";
            }
        }

        /// <summary>
        ///     The method to run on the target when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected abstract void Use(BaseBattleDriver target);

        /// <summary>
        ///     The method to run on the user when this action is being used.
        /// </summary>
        protected virtual void OnUse() { }
    }
}
