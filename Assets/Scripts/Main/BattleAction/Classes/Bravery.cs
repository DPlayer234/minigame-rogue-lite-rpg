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
    ///     Increases the stats of all allies
    /// </summary>
    public class Bravery : BattleAction
    {
        /// <summary> How long the buffs last </summary>
        private const int TurnDuration = 1;

        /// <summary> The stat increase of the bravery buffs. </summary>
        private const float StatIncrease = 1.20f;

        /// <summary>
        ///     All the buffs applied by <see cref="Bravery"/>
        /// </summary>
        private static Buff[] buffs = new Buff[]
        {
            new StatBuff(Stat.PhysicalDamage, Bravery.StatIncrease, Bravery.TurnDuration),
            new StatBuff(Stat.MagicalDamage, Bravery.StatIncrease, Bravery.TurnDuration),
            new StatBuff(Stat.Defense, Bravery.StatIncrease, Bravery.TurnDuration)
        };

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bravery"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public Bravery(BaseBattleDriver user) : base(user)
        {
            this.name = "Bravery";
            this.description = "Increases Damage and Defense of all allies for 1 turn.";

            this.attackPointCost = 12.0f;
            this.attackPower = 0.0f;
            this.category = ActionCategory.Support;
            this.targetOption = ActionTargetOption.AllAllies;
        }

        /// <summary>
        ///     The method to run when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void Use(BaseBattleDriver target)
        {
            foreach (Buff buff in Bravery.buffs)
            {
                buff.Apply(target);
            }
        }
    }
}
