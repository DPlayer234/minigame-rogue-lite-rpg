namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using DPlay.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Fire magic which will burn the target
    /// </summary>
    public class FireMagic : BattleAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FireMagic"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public FireMagic(BaseBattleDriver user) : base(user)
        {
            this.name = "Fire Magic";
            this.description = "Magic which deals damage to an opponent and has them take damage for a few turns.";

            this.attackPointCost = 5.0f;
            this.attackPower = 5.0f;
            this.category = ActionCategory.MagicalAttack;
            this.targetOption = ActionTargetOption.OneOpponent;
        }

        /// <summary>
        ///     The method to run when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void Use(BaseBattleDriver target)
        {
            this.DealDamage(target);

            new FireDebuff().Apply(target);
        }

        /// <summary>
        ///     Debuff that this attack can cause
        /// </summary>
        public class FireDebuff : Buff
        {
            /// <summary>
            ///     The minimum amount of turns the fire may last
            /// </summary>
            private const int MinimumTurnDuration = 2;

            /// <summary>
            ///     The maximum amount of turn the fire may last
            /// </summary>
            private const int MaximumTurnDuration = 4;

            /// <summary>
            ///     How much damage, in relation to their max. health should
            ///     a target take per turn.
            /// </summary>
            private const float DamagePerTurn = 0.075f;

            /// <summary>
            ///     Initializes a new instance of the <see cref="FireDebuff"/> class.
            ///     It lasts for <seealso cref="DebuffTurnDuration"/> turns.
            /// </summary>
            public FireDebuff() : base(UnityEngine.Random.Range(FireDebuff.MinimumTurnDuration, FireDebuff.MaximumTurnDuration + 1)) { }

            /// <summary>
            ///     Called every turn while the buff is applied
            /// </summary>
            /// <param name="target">The target battle driver</param>
            protected override void OnTurn(BaseBattleDriver target)
            {
                target.CurrentHealth -= (int)(target.MaximumHealth * FireDebuff.DamagePerTurn);
            }
        }
    }
}
