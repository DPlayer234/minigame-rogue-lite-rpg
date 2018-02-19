namespace SAE.RoguePG.Main.BattleDriver
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Dev;
    using SAE.RoguePG.Main.BattleAction;
    using SAE.RoguePG.Main.Driver;
    using SAE.RoguePG.Main.Sprite3D;
    using SAE.RoguePG.Main.UI;
    using UnityEngine;

    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [RequireComponent(typeof(BaseDriver))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SpriteManager))]
    [RequireComponent(typeof(SpriteAnimator))]
    [DisallowMultipleComponent]
    public abstract partial class BaseBattleDriver : MonoBehaviour
    {
        /// <summary> "Levels" added for each stat </summary>
        public const int LevelStatOffset = 4;

        /// <summary> The multiplier for the base stat when it gets a bonus </summary>
        public const float BonusStatMultiplier = 1.15f;

        /// <summary> The duration for targetting highlights in seconds </summary>
        public const float TargetHighlightDuration = 0.5f;

        /// <summary> An array of possible battle names. </summary>
        public string[] possibleBattleNames;

        /// <summary> An array of the used <seealso cref="BattleAction.ActionClass"/>es </summary>
        public BattleAction.ActionClass[] actionClasses;

        /// <summary> The <seealso cref="SpriteManager"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public SpriteManager spriteManager;

        /// <summary> The <seealso cref="SpriteAnimator"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public SpriteAnimator spriteAnimator;

        /// <summary> The <seealso cref="BaseDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public BaseDriver entityDriver;

        /// <summary> The <seealso cref="Highlighter"/> also attached to this <seealso cref="GameObject"/> </summary>
        [HideInInspector]
        public Highlighter highlight;

        /// <summary> An array of <seealso cref="BattleAction"/>s; generated from <seealso cref="actionClasses"/> </summary>
        protected BattleAction[] actions;

        /// <summary> Status Display used by this BattleDriver </summary>
        protected StatusDisplayController statusDisplay;

        /// <summary> Whether it's this thing's turn </summary>
        private bool takingTurn;

        /// <summary> The name displayed in battle </summary>
        [SerializeField]
        private string battleName = "Fighter";

        /// <summary> The name is the #th occurence within its allies </summary>
        private int battleNameIndex = 0;

        /// <summary> The buffer for the displayed battle name. </summary>
        private string battleNameBuffer;

        /// <summary> Current level; use the property <seealso cref="Level"/> instead </summary>
        [SerializeField]
        private int level = 1;

        /// <summary> The amount of animations that this object is waiting for to complete </summary>
        private int waitingOnAnimationCount = 0;

        /// <summary>
        ///     Any function to be called when a turn starts or ends
        /// </summary>
        /// <returns>Whether to remove the action</returns>
        public delegate bool TurnAction();

        /// <summary> The name displayed in battle </summary>
        public string BattleName
        {
            get
            {
                if (this.battleNameIndex < 1)
                {
                    return this.battleName;
                }
                
                return this.battleNameBuffer ?? (this.battleNameBuffer = this.battleName + " #" + this.battleNameIndex);
            }
        }

        /// <summary> A list of actions to be called when a turn starts </summary>
        public List<TurnAction> StartTurnActions { get; private set; }

        /// <summary> A list of actions to be called when a turn ends </summary>
        public List<TurnAction> EndTurnActions { get; private set; }

        /// <summary> All of its allies, including itself </summary>
        public List<BaseBattleDriver> Allies { get; set; }

        /// <summary> All of its opponents </summary>
        public List<BaseBattleDriver> Opponents { get; set; }

        /// <summary> All of its allies and opponents. </summary>
        public List<BaseBattleDriver> AlliesAndOpponents { get; set; }

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
                this.takingTurn = value;

                if (value)
                {
                    this.StartTurn();
                }
                else
                {
                    this.EndTurn();
                }
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

        /// <summary> The number of turns that this driver has already taken </summary>
        public int TurnNumber { get; protected set; }

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

            this.AttackPoints = 0.0f;

            this.waitingOnAnimationCount = 0;

            this.TurnNumber = 0;
            this.StartTurnActions = new List<TurnAction>();
            this.EndTurnActions = new List<TurnAction>();
        }

        /// <summary>
        ///     To be called when a battle ends
        /// </summary>
        public virtual void OnBattleEnd()
        {
            this.StopAllCoroutines();
        }

        /// <summary>
        ///     Sets up everything needed for the Entity's turn
        /// </summary>
        public virtual void StartTurn()
        {
            this.LogThisAndFormat("Start Turn!");
            MainManager.CameraController.following = this.spriteManager.rootTransform;

            ++this.TurnNumber;

            this.EnableTurnHighlight();

            // Handle turn actions
            for (int i = this.StartTurnActions.Count - 1; i >= 0; i--)
            {
                if (this.StartTurnActions[i]())
                {
                    this.StartTurnActions.RemoveAt(i);
                }
            }
        }

        /// <summary>
        ///     Ends the Entity's turn
        /// </summary>
        public virtual void EndTurn()
        {
            this.LogThisAndFormat("End Turn!");

            this.DisableTurnHighlight();

            // Handle turn actions
            for (int i = this.EndTurnActions.Count - 1; i >= 0; i--)
            {
                if (this.EndTurnActions[i]())
                {
                    this.EndTurnActions.RemoveAt(i);
                }
            }
        }

        /// <summary>
        ///     Updates the Entity's turn once a frame
        /// </summary>
        public virtual void UpdateTurn() { }

        /// <summary>
        ///     Updates the Entity once a frame while nothing is taking a turn
        /// </summary>
        public virtual void UpdateIdle()
        {
            this.RegenerateAttackPoint();
        }

        /// <summary>
        ///     Leaves the party from the battle.
        ///     The last party member will never leave.
        /// </summary>
        public virtual bool LeaveParty()
        {
            if (this.Allies.Count > 1)
            {
                this.Allies.Remove(this);
                this.AlliesAndOpponents.Remove(this);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Highlights this driver as a target
        /// </summary>
        public void HighlightAsTarget()
        {
            this.highlight.Enable(Highlighter.TargetColor, BaseBattleDriver.TargetHighlightDuration);
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
            this.highlight = this.GetComponent<Highlighter>();

            this.battleName = this.possibleBattleNames != null ? this.possibleBattleNames.GetRandomItem() : this.battleName;
        }

        /// <summary>
        ///     Enables the highlight for taking a turn
        /// </summary>
        private void EnableTurnHighlight()
        {
            this.highlight.Enable(Highlighter.ActiveColor);
        }

        /// <summary>
        ///     Disables the highlight for having a turn
        /// </summary>
        private void DisableTurnHighlight()
        {
            if (this.highlight.Color == Highlighter.ActiveColor)
            {
                this.highlight.Disable();
            }
        }
    }
}
