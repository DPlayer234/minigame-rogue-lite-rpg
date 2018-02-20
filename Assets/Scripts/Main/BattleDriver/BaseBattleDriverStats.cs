namespace DPlay.RoguePG.Main.BattleDriver
{
    using System.Collections;
    using System.Collections.Generic;
    using DPlay.RoguePG.Dev;
    using DPlay.RoguePG.Main.BattleAction;
    using DPlay.RoguePG.Main.Driver;
    using DPlay.RoguePG.Main.Sprite3D;
    using UnityEngine;

    /// <summary>
    ///     Makes battles work.
    ///     (Stats)
    /// </summary>
    public abstract partial class BaseBattleDriver
    {
        // Integer overflow for health HIGHLY unlikely until floor ~10,000.
        // It's safe to assume, that this is never going to happen.
        // If it does, enjoy negative BOSS HP.
        // Negative regular HP shouldn't occur until floor ~30,000.

        /// <summary> Maximum amount of <seealso cref="AttackPoints"/>. Also represents the amount needed to get a turn. </summary>
        public const float MaximumAttackPoints = 10.0f;

        /// <summary> The minimum wait in seconds for the next turn </summary>
        public const float MinimumTurnWait = 1.0f;
        
        /// <summary> The base value for the <seealso cref="MaximumHealth"/> stat</summary>
        public float healthBase = 10.0f;

        /// <summary> The base value for the <seealso cref="PhysicalDamage"/> stat</summary>
        public float physicalBase = 10.0f;

        /// <summary> The base value for the <seealso cref="MagicalDamage"/> stat</summary>
        public float magicalBase = 10.0f;

        /// <summary> The base value for the <seealso cref="Defense"/> stat</summary>
        public float defenseBase = 10.0f;

        /// <summary> The base value for the <seealso cref="TurnSpeed"/> stat</summary>
        public float speedBase = 10.0f;

        /// <summary> Maximum Health; use the property <seealso cref="MaximumHealth"/> instead </summary>
        protected int maximumHealth = -1;

        /// <summary> Current Health Value; use the property <seealso cref="CurrentHealth"/> instead </summary>
        protected int currentHealth = -1;

        /// <summary> Physical Damage value; use the property <seealso cref="PhysicalDamage"/> instead </summary>
        protected float physicalDamage;

        /// <summary> Magical Damage value; use the property <seealso cref="MagicalDamage"/> instead </summary>
        protected float magicalDamage;

        /// <summary> Defense; resistance against damage; use the property <seealso cref="Defense"/> instead </summary>
        protected float defense;

        /// <summary> How fast and often can they take a turn; use the property <seealso cref="TurnSpeed"/> instead </summary>
        protected float turnSpeed;

        /// <summary> Maximum Health </summary>
        public int MaximumHealth { get { return this.maximumHealth; } private set { this.maximumHealth = value; } }

        /// <summary> Current Health Value </summary>
        public virtual int CurrentHealth
        {
            get
            {
                return this.currentHealth;
            }

            set
            {
                if (this.currentHealth > value)
                {
                    SFXManager.PlayClip("Death", this.transform.position);
                }

                this.currentHealth = Mathf.Clamp(value, 0, this.MaximumHealth);
            }
        }

        /// <summary> Physical Damage value </summary>
        public float PhysicalDamage { get { return this.physicalDamage; } private set { this.physicalDamage = value; } }

        /// <summary> Magical Damage value </summary>
        public float MagicalDamage { get { return this.magicalDamage; } private set { this.magicalDamage = value; } }

        /// <summary> Defense; resistance against damage </summary>
        public float Defense { get { return this.defense; } private set { this.defense = value; } }

        /// <summary> How fast and often can they take a turn </summary>
        public float TurnSpeed { get { return this.turnSpeed; } private set { this.turnSpeed = value; } }

        /// <summary>
        ///     Represents the cost that attacks for the current turn can still take.
        ///     They will regenerate during the idle phase and, once they reach <seealso cref="MaximumAttackPoints"/>, it will be this Entity's turn.
        /// </summary>
        public float AttackPoints { get; set; }

        /// <summary>
        ///     The highest turn speed in the current battle.
        ///     Set during battle initialization.
        /// </summary>
        public static float HighestTurnSpeed { get; set; }

        /// <summary>
        ///     Gets a stat by the enumator
        /// </summary>
        /// <param name="stat">Which stat</param>
        /// <returns>A value of the given stat</returns>
        public float GetStat(Stat stat)
        {
            switch (stat)
            {
                case Stat.MaximumHealth:
                    return this.MaximumHealth;
                case Stat.PhysicalDamage:
                    return this.PhysicalDamage;
                case Stat.MagicalDamage:
                    return this.MagicalDamage;
                case Stat.Defense:
                    return this.Defense;
                case Stat.TurnSpeed:
                    return this.TurnSpeed;
                default:
                    throw new RPGException(RPGException.Cause.StatInvalid);
            }
        }

        /// <summary>
        ///     Sets a stat by the enumator
        /// </summary>
        /// <param name="stat">Which stat</param>
        public float SetStat(Stat stat, float value)
        {
            switch (stat)
            {
                case Stat.MaximumHealth:
                    return this.MaximumHealth = (int)value;
                case Stat.PhysicalDamage:
                    return this.PhysicalDamage = value;
                case Stat.MagicalDamage:
                    return this.MagicalDamage = value;
                case Stat.Defense:
                    return this.Defense = value;
                case Stat.TurnSpeed:
                    return this.TurnSpeed = value;
                default:
                    throw new RPGException(RPGException.Cause.StatInvalid);
            }
        }

        /// <summary>
        ///     Gets the base value of a given stat
        /// </summary>
        /// <param name="stat">Which stat</param>
        /// <returns>The value</returns>
        public float GetBaseStat(Stat stat)
        {
            switch (stat)
            {
                case Stat.MaximumHealth:
                    return this.healthBase;
                case Stat.PhysicalDamage:
                    return this.physicalBase;
                case Stat.MagicalDamage:
                    return this.magicalBase;
                case Stat.Defense:
                    return this.defenseBase;
                case Stat.TurnSpeed:
                    return this.speedBase;
                default:
                    throw new RPGException(RPGException.Cause.StatInvalid);
            }
        }

        /// <summary>
        ///     Sets the base value of a given stat.
        ///     Does not recalculate the stats.
        /// </summary>
        /// <param name="stat">Which stat</param>
        /// <param name="value">The value to set it to</param>
        public float SetBaseStat(Stat stat, float value)
        {
            switch (stat)
            {
                case Stat.MaximumHealth:
                    return this.healthBase = value;
                case Stat.PhysicalDamage:
                    return this.physicalBase = value;
                case Stat.MagicalDamage:
                    return this.magicalBase = value;
                case Stat.Defense:
                    return this.defenseBase = value;
                case Stat.TurnSpeed:
                    return this.speedBase = value;
                default:
                    throw new RPGException(RPGException.Cause.StatInvalid);
            }
        }

        /// <summary>
        ///     Calculates a given stat.
        /// </summary>
        /// <param name="base">The base stat</param>
        /// <returns>The stat adjusted to level</returns>
        public float CalculateStat(float @base)
        {
            return @base * (this.Level + BaseBattleDriver.LevelStatOffset);
        }

        /// <summary>
        ///     Recalculates all stats (Health, Physical Damage, etc...)
        /// </summary>
        public void RecalculateStats()
        {
            int oldMaximumHealth = this.MaximumHealth;

            this.MaximumHealth = (int)(this.CalculateStat(this.healthBase) * 5);
            this.PhysicalDamage = this.CalculateStat(this.physicalBase);
            this.MagicalDamage = this.CalculateStat(this.magicalBase);
            this.Defense = this.CalculateStat(this.defenseBase);
            this.TurnSpeed = this.CalculateStat(this.speedBase);

            // Make sure the health value is valid
            this.CurrentHealth = Mathf.Max(1, this.CurrentHealth + this.MaximumHealth - oldMaximumHealth);
        }

        /// <summary>
        ///     Updates and regenerates attack points
        /// </summary>
        private void RegenerateAttackPoint()
        {
            if (this.CanStillFight)
            {
                this.AttackPoints +=
                    (this.turnSpeed / BaseBattleDriver.HighestTurnSpeed) *
                    (Time.deltaTime / BaseBattleDriver.MinimumTurnWait) * BaseBattleDriver.MaximumAttackPoints;
            }
        }
    }
}
