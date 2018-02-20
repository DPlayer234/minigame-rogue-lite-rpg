namespace DPlay.RoguePG.Main.BattleAction
{
    using DPlay.RoguePG.Main.BattleDriver;

    /// <summary>
    ///     Any Buff
    /// </summary>
    public abstract class Buff
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Buff"/> class.
        /// </summary>
        /// <param name="turnDuration">The turn duration</param>
        public Buff(int turnDuration = 2)
        {
            this.TurnDuration = turnDuration;
        }

        /// <summary>
        ///     The duration in turns
        /// </summary>
        public int TurnDuration { get; protected set; }

        /// <summary>
        ///     Applies a buff to a battle driver
        /// </summary>
        /// <param name="target">The target battle driver</param>
        public void Apply(BaseBattleDriver target)
        {
            this.OnApplication(target);

            int turnDuration = this.TurnDuration;

            target.EndTurnActions.Add(delegate
            {
                --turnDuration;

                if (turnDuration <= 0)
                {
                    this.OnRemoval(target);

                    return true;
                }

                this.OnTurn(target);

                return false;
            });
        }

        // These functions aren't abstract because they are supposed to be optional.

        /// <summary>
        ///     Called when the buff is applied
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected virtual void OnApplication(BaseBattleDriver target) { }

        /// <summary>
        ///     Called when the buff runs out
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected virtual void OnRemoval(BaseBattleDriver target) { }

        /// <summary>
        ///     Called every turn while the buff is applied
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected virtual void OnTurn(BaseBattleDriver target) { }
    }
}
