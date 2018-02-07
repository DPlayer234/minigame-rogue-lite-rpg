namespace SAE.RoguePG.Main.BattleDriver
{
    using SAE.RoguePG.Dev;
    using SAE.RoguePG.Main.BattleActions;
    using SAE.RoguePG.Main.Driver;
    using SAE.RoguePG.Main.Sprite3D;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SpriteManager))]
    [RequireComponent(typeof(SpriteAnimator))]
    [DisallowMultipleComponent]
    public abstract class BaseBattleDriver : MonoBehaviour
    {
        /// <summary> Maximum amount of <seealso cref="AttackPoints"/>. Also represents the amount needed to get a turn. </summary>
        public const float MaximumAttackPoints = 10.0f;

        /// <summary> "Levels" added for each stat </summary>
        public const int LevelStatOffset = 4;

        /// <summary> The name displayed in battle </summary>
        public string battleName;

        /// <summary> An array of <seealso cref="BattleAction.ActionClass"/>es </summary>
        public BattleAction.ActionClass[] actionClasses;

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

        /// <summary> The <seealso cref="SpriteManager"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public SpriteManager spriteManager;

        /// <summary> The <seealso cref="SpriteAnimator"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public SpriteAnimator spriteAnimator;

        /// <summary> The <seealso cref="BaseDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public BaseDriver entityDriver;

        /// <summary> An array of <seealso cref="BattleAction"/>s; generated from <seealso cref="actionClasses"/> </summary>
        protected BattleAction[] actions;

        /// <summary> Whether it's this thing's turn </summary>
        private bool takingTurn;

        /// <summary> Status Display used by this BattleDriver </summary>
        protected GameObject statusDisplay;

        /// <summary> Current level; use the property <seealso cref="Level"/> instead </summary>
        [SerializeField]
        private int level = 1;

        /// <summary> Maximum Health; use the property <seealso cref="MaximumHealth"/> instead </summary>
        private int maximumHealth = -1;

        /// <summary> Current Health Value; use the property <seealso cref="CurrentHealth"/> instead </summary>
        private int currentHealth = -1;

        /// <summary> Physical Damage value; use the property <seealso cref="PhysicalDamage"/> instead </summary>
        private float physicalDamage;

        /// <summary> Magical Damage value; use the property <seealso cref="MagicalDamage"/> instead </summary>
        private float magicalDamage;

        /// <summary> Defense; resistance against damage; use the property <seealso cref="Defense"/> instead </summary>
        private float defense;

        /// <summary> How fast and often can they take a turn; use the property <seealso cref="TurnSpeed"/> instead </summary>
        private float turnSpeed;

        /// <summary> The amount of animations that this object is waiting for to complete </summary>
        private int waitingOnAnimationCount = 0;

        /// <summary> All of its allies, including itself </summary>
        public BaseBattleDriver[] Allies { get; set; }

        /// <summary> All of its opponents </summary>
        public BaseBattleDriver[] Opponents { get; set; }

        /// <summary>
        ///     All of its allies and opponents.
        ///     This is potentially slow if called repeatedly as it creates a new array everytime.
        /// </summary>
        public BaseBattleDriver[] AlliesAndOpponents
        {
            get
            {
                var alliesAndOpponents = new BaseBattleDriver[this.Allies.Length + this.Opponents.Length];

                int i = 0;

                foreach (BaseBattleDriver ally in this.Allies)
                {
                    alliesAndOpponents[i++] = ally;
                }

                foreach (BaseBattleDriver opponent in this.Opponents)
                {
                    alliesAndOpponents[i++] = opponent;
                }

                return alliesAndOpponents;
            }
        }

        /// <summary>
        ///     Gets whether it is waiting on any animation.
        ///     Setting either adds or subtracts from the total.
        /// </summary>
        public bool IsWaitingOnAnimation
        {
            get
            {
                return this.waitingOnAnimationCount > 0;
            }

            set
            {
                this.waitingOnAnimationCount += (value ? 1 : -1);
            }
        }

        /// <summary> Whether they can still fight </summary>
        public bool CanStillFight { get { return this.CurrentHealth > 0; } }

        /// <summary> Whether it's this thing's turn; updating this value will call <seealso cref="StartTurn"/> (true) or <seealso cref="EndTurn"/> (false) </summary>
        public bool TakingTurn
        {
            get
            {
                return this.takingTurn;
            }

            set
            {
                if (value)
                {
                    this.StartTurn();
                }
                else
                {
                    this.EndTurn();
                }

                this.takingTurn = value;
            }
        }

        /// <summary> Current level </summary>
        public int Level
        {
            get
            {
                return this.level;
            }

            set
            {
                this.level = Mathf.Max(1, value);
                this.RecalculateStats();
            }
        }

        /// <summary> Maximum Health </summary>
        public int MaximumHealth { get { return this.maximumHealth; } private set { this.maximumHealth = value; } }

        /// <summary> Current Health Value </summary>
        public int CurrentHealth
        {
            get
            {
                return this.currentHealth;
            }

            set
            {
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
        ///     Regenerates (updates) the <seealso cref="actions"/> from <seealso cref="actionClasses"/>
        /// </summary>
        public void RegenerateActions()
        {
            this.actions = new BattleAction[this.actionClasses.Length];

            for (int i = 0; i < this.actionClasses.Length; i++)
            {
                this.actions[i] = BattleAction.GetBattleAction(this.actionClasses[i], this);
            }
        }

        /// <summary>
        ///     To be called when a battle starts
        /// </summary>
        public virtual void OnBattleStart()
        {
            this.RegenerateActions();
            this.RecalculateStats();

            this.AttackPoints = MaximumAttackPoints;

            this.statusDisplay = Instantiate(MainManager.Instance.statusDisplayPrefab, this.spriteManager.rootTransform);
            this.statusDisplay.transform.localPosition = new Vector3(0.0f, 1.3f, 0.0f);

            this.waitingOnAnimationCount = 0;
        }

        /// <summary>
        ///     To be called when a battle ends
        /// </summary>
        public virtual void OnBattleEnd()
        {
            if (this.statusDisplay != null)
            {
                Destroy(this.statusDisplay);
            }

            this.StopAllCoroutines();
        }

        /// <summary>
        ///     Sets up everything needed for the Entity's turn
        /// </summary>
        public virtual void StartTurn()
        {
            this.LogThisAndFormat("Start Turn!");
            MainManager.CameraController.following = this.transform;
        }

        /// <summary>
        ///     Ends the Entity's turn
        /// </summary>
        public virtual void EndTurn()
        {
            this.LogThisAndFormat("End Turn!");
        }

        /// <summary>
        ///     Updates the Entity's turn once a frame
        /// </summary>
        public virtual void UpdateTurn()
        {
            
        }

        /// <summary>
        ///     Updates the Entity once a frame while nothing is taking a turn
        /// </summary>
        public virtual void UpdateIdle()
        {
            if (this.CanStillFight)
            {
                this.AttackPoints += this.turnSpeed * Time.deltaTime;
            }
        }

        /// <summary>
        ///     Called by Unity when the Behaviour is enabled
        /// </summary>
        protected virtual void OnEnable()
        {
            // Reset velocity.
            Rigidbody rigidbody = this.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="BaseBattleDriver"/> whether it is or is not active.
        /// </summary>
        protected virtual void Awake()
        {
            this.spriteManager = this.GetComponent<SpriteManager>();
            this.spriteAnimator = this.GetComponent<SpriteAnimator>();
            this.entityDriver = this.GetComponent<BaseDriver>();
        }
        
        /// <summary>
        ///     Called by Unity every frame to update the <see cref="BaseBattleDriver"/>
        /// </summary>
        protected virtual void Update()
        {

        }
    }
}
