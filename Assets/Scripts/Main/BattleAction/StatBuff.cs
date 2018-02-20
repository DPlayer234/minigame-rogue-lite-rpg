namespace DPlay.RoguePG.Main.BattleAction
{
    using DPlay.RoguePG.Main.BattleDriver;

    /// <summary>
    ///     Any Buff modifying stats
    /// </summary>
    public class StatBuff : Buff
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StatBuff"/> class.
        /// </summary>
        /// <param name="stat">The stat to change</param>
        /// <param name="multiplier">The multiplier</param>
        /// <param name="turnDuration">The turn duration</param>
        public StatBuff(Stat stat, float multiplier = 1.1f, int turnDuration = 2) : base(turnDuration)
        {
            this.Stat = stat;
            this.Multiplier = multiplier;
            this.TurnDuration = turnDuration;
        }

        /// <summary>
        ///     The stat to modify
        /// </summary>
        public Stat Stat { get; protected set; }

        /// <summary>
        ///     The multiplier for the stat
        /// </summary>
        public float Multiplier { get; protected set; }

        /// <summary>
        ///     Called when the buff is applied
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void OnApplication(BaseBattleDriver target)
        {
            target.SetStat(this.Stat, target.GetStat(this.Stat) * this.Multiplier);
        }

        /// <summary>
        ///     Called when the buff runs out
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void OnRemoval(BaseBattleDriver target)
        {
            target.SetStat(this.Stat, target.GetStat(this.Stat) / this.Multiplier);
        }
    }
}
