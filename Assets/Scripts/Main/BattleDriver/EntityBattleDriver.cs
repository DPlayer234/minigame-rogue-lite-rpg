using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RoguePG.Main.Driver;
using SAE.RoguePG.Main.Sprite3D;

namespace SAE.RoguePG.Main.BattleDriver
{
    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [DisallowMultipleComponent]
    public class EntityBattleDriver : MonoBehaviour
    {
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

        /// <summary> The <seealso cref="EntityDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public EntityDriver entityDriver;

        /// <summary> All of its allies, including itself </summary>
        protected GameObject[] allies;

        /// <summary> All of its opponents </summary>
        protected GameObject[] opponents;

        /// <summary> Whether it's this thing's turn </summary>
        private bool takingTurn;

        /// <summary> Current level; use the property <seealso cref="Level"/> instead </summary>
        private int level;

        /// <summary> Maximum Health; use the property <seealso cref="MaximumHealth"/> instead </summary>
        private int maximumHealth;

        /// <summary> Current Health Value; use the property <seealso cref="CurrentHealth"/> instead </summary>
        private int currentHealth;

        /// <summary> Physical Damage value; use the property <seealso cref="PhysicalDamage"/> instead </summary>
        private float physicalDamage;

        /// <summary> Magical Damage value; use the property <seealso cref="MagicalDamage"/> instead </summary>
        private float magicalDamage;

        /// <summary> Defense; resistance against damage; use the property <seealso cref="Defense"/> instead </summary>
        private float defense;

        /// <summary> How fast and often can they take a turn; use the property <seealso cref="TurnSpeed"/> instead </summary>
        private float turnSpeed;

        /// <summary> Whether they can still fight </summary>
        public bool CanStillFight { get { return this.CurrentHealth > 0; } }

        /// <summary> Whether they are knocked out </summary>
        public bool KnockedOut { get { return !this.CanStillFight; } }

        /// <summary> Whether it's this thing's turn; updating this value will call <seealso cref="StartTurn"/> or <seealso cref="EndTurn"/> </summary>
        public bool TakingTurn
        {
            get
            {
                return this.takingTurn;
            }

            set
            {
                if (this.takingTurn != value)
                {
                    if (value)
                    {
                        this.StartTurn();
                    }
                    else
                    {
                        this.EndTurn();
                    }
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
                this.level = value;
                this.RecalculateStats();
            }
        }

        /// <summary> Maximum Health </summary>
        public int MaximumHealth { get { return this.maximumHealth; } private set { this.maximumHealth = value; } }

        /// <summary> Current Health Value </summary>
        public int CurrentHealth {
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
        ///     Recalculates all stats (Health, Physical Damage, etc...)
        /// </summary>
        public void RecalculateStats()
        {
            this.MaximumHealth = (int)(this.Level * this.healthBase * 5);
            this.PhysicalDamage = this.Level * this.physicalBase;
            this.MagicalDamage = this.Level * this.magicalBase;
            this.Defense = this.Level * this.defenseBase;
            this.TurnSpeed = this.Level * this.speedBase;

            // Make sure the health value is valid
            this.CurrentHealth = this.CurrentHealth;
        }

        /// <summary>
        ///     Sets the array of allies and opponents.
        /// </summary>
        /// <param name="allies">All allies</param>
        /// <param name="opponents">All opponents</param>
        public void SetAlliesAndOpponents(GameObject[] allies, GameObject[] opponents)
        {
            this.allies = allies;
            this.opponents = opponents;
        }

        /// <summary>
        ///     Sets up everything needed for the Entity's turn
        /// </summary>
        protected virtual void StartTurn()
        {
            Debug.LogWarning("Attempting to Start the turn of a EntityBattleDriver. Use a PlayerBattleDriver or EntityBattleDriver instead.");
        }

        /// <summary>
        ///     Ends the Entity's turn
        /// </summary>
        protected virtual void EndTurn()
        {
            Debug.LogWarning("Attempting to End the turn of a EntityBattleDriver. Use a PlayerBattleDriver or EntityBattleDriver instead.");
        }

        /// <summary>
        ///     Updates the Entity's turn once a frame
        /// </summary>
        protected virtual void UpdateTurn()
        {
            Debug.LogWarning("Attempting to Update the turn of a EntityBattleDriver. Use a PlayerBattleDriver or EntityBattleDriver instead.");
        }

        /// <summary>
        ///     Called by Unity when the Behaviour is enabled
        /// </summary>
        protected void OnEnable()
        {
            // Reset velocity.
            Rigidbody rigidbody = this.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EntityBattleDriver"/> whether it is or is not active.
        /// </summary>
        protected void Awake()
        {
            this.spriteManager = this.GetComponent<SpriteManager>();
            this.spriteAnimator = this.GetComponent<SpriteAnimator>();
            this.entityDriver = this.GetComponent<EntityDriver>();
        }
        
        /// <summary>
        ///     Called by Unity every frame to update the <see cref="EntityBattleDriver"/>
        /// </summary>
        protected void Update()
        {
            if (this.TakingTurn)
            {
                this.UpdateTurn();
            }
        }
    }
}
