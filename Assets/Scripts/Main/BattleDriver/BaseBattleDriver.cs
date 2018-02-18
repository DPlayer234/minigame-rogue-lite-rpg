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

        /// <summary> An array of possible battle names. </summary>
        public string[] possibleBattleNames;

        /// <summary> The name displayed in battle </summary>
        public string battleName;

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

        /// <summary> An array of <seealso cref="BattleAction"/>s; generated from <seealso cref="actionClasses"/> </summary>
        protected BattleAction[] actions;

        /// <summary> Status Display used by this BattleDriver </summary>
        protected StatusDisplayController statusDisplay;

        /// <summary> Whether it's this thing's turn </summary>
        private bool takingTurn;

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

        /// <summary> A list of actions to be called when a turn starts </summary>
        public List<TurnAction> StartTurnActions { get; private set; }

        /// <summary> A list of actions to be called when a turn ends </summary>
        public List<TurnAction> EndTurnActions { get; private set; }

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
        ///     Creates status bars for a set of battle drivers.
        /// </summary>
        /// <param name="battleDrivers">The battle drivers to generate them for</param>
        /// <param name="parent">The transform to parent them to</param>
        public static void CreateStatusBars(IList<BaseBattleDriver> battleDrivers, Transform parent)
        {
            int playerIndex = 0, enemyIndex = 0;

            foreach (BaseBattleDriver battleDriver in battleDrivers)
            {
                StatusDisplayController statusDisplay = MonoBehaviour.Instantiate(
                    battleDriver is PlayerBattleDriver ? GenericPrefab.StatusDisplayPlayer : GenericPrefab.StatusDisplayEnemy,
                    parent);

                print(battleDriver);
                statusDisplay.battleDriver = battleDriver;

                RectTransform rectTransform = statusDisplay.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition3D = new Vector3(
                        0.0f,
                        -StatusDisplayController.Height * (battleDriver is PlayerBattleDriver ? playerIndex++ : enemyIndex++),
                        0.0f);
                }
            }
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

            this.AttackPoints = BaseBattleDriver.MaximumAttackPoints;

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
        public virtual void UpdateTurn()
        {

        }

        /// <summary>
        ///     Updates the Entity once a frame while nothing is taking a turn
        /// </summary>
        public virtual void UpdateIdle()
        {
            this.RegenerateAttackPoint();
        }

        /// <summary>
        ///     Deduplicates battle names by suffixing #Number
        /// </summary>
        public void DeduplicateBattleNamesInAllies()
        {
            // Count every occurence
            Dictionary<string, int> names = new Dictionary<string, int>();

            foreach (BaseBattleDriver ally in this.Allies)
            {
                if (names.ContainsKey(ally.battleName))
                {
                    names[ally.battleName] += 1;
                    continue;
                }

                names.Add(ally.battleName, 1);
            }

            // Add #N where needed
            Dictionary<string, int> namesYet = new Dictionary<string, int>();

            foreach (BaseBattleDriver ally in this.Allies)
            {
                if (names[ally.battleName] > 1)
                {
                    if (!namesYet.ContainsKey(ally.battleName))
                    {
                        namesYet.Add(ally.battleName, 0);
                    }

                    ally.battleName = ally.battleName + " #" + (++namesYet[ally.battleName]);
                }
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

            this.battleName = this.possibleBattleNames != null ? this.possibleBattleNames.GetRandomItem() : this.battleName;
        }
    }
}
