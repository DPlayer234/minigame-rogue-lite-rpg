namespace SAE.RoguePG.Main.BattleDriver
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleAction;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerBattleDriver : BaseBattleDriver
    {
        /// <summary>
        ///     Distance between buttons
        /// </summary>
        public const float ButtonDistance = 0.2f;

        /// <summary>
        ///     Base Height for buttons
        /// </summary>
        public const float ButtonBaseHeight = 0.45f;

        /// <summary> Prefab for action buttons </summary>
        [SerializeField]
        private Button actionButtonPrefab;

        /// <summary> The parent object for action buttons </summary>
        private GameObject actionButtonHolder;

        /// <summary> The parent object for target buttons </summary>
        private GameObject targetButtonHolder;

        /// <summary>
        ///     To be called when a battle starts
        /// </summary>
        public override void OnBattleStart()
        {
            base.OnBattleStart();
        }

        /// <summary>
        ///     To be called when a battle ends
        /// </summary>
        public override void OnBattleEnd()
        {
            base.OnBattleEnd();
        }

        /// <summary>
        ///     Sets up everything needed for the Player's turn
        /// </summary>
        public override void StartTurn()
        {
            base.StartTurn();

            if (!this.CanStillFight) return;

            this.CreateActionButtons();
        }

        /// <summary>
        ///     Ends the Player's turn
        /// </summary>
        public override void EndTurn()
        {
            base.EndTurn();

            if (this.actionButtonHolder != null) MonoBehaviour.Destroy(this.actionButtonHolder);
            if (this.targetButtonHolder != null) MonoBehaviour.Destroy(this.targetButtonHolder);
        }

        /// <summary>
        ///     Updates the Player's turn once a frame
        /// </summary>
        public override void UpdateTurn()
        {
            base.UpdateTurn();

            if (this.AttackPoints <= 0)
            {
                if (this.actionButtonHolder != null) MonoBehaviour.Destroy(this.actionButtonHolder);
                if (this.targetButtonHolder != null) MonoBehaviour.Destroy(this.targetButtonHolder);

                this.TakingTurn = false;
            }
        }

        /// <summary>
        ///     Updates the Player once a frame while nothing is taking a turn
        /// </summary>
        public override void UpdateIdle()
        {
            base.UpdateIdle();

            if (!this.CanStillFight) return;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> whether it is or is not active.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="PlayerBattleDriver"/>
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        ///     Creates the the buttons for all actions
        /// </summary>
        private void CreateActionButtons()
        {
            this.actionButtonHolder = MonoBehaviour.Instantiate(MainManager.GenericPanelPrefab, MainManager.WorldCanvas.transform);

            for (int actionIndex = 0; actionIndex < this.actions.Length; actionIndex++)
            {
                BattleAction action = this.actions[actionIndex];

                Button actionButton = MonoBehaviour.Instantiate(this.actionButtonPrefab, this.actionButtonHolder.transform);
                actionButton.GetComponentInChildren<Text>().text = string.Format("{0} [{1} AP]", action.Name, action.AttackPointCost);

                var actionButtonController = actionButton.GetComponent<UI.ButtonController>();
                actionButtonController.reference = this.transform;
                actionButtonController.positionOffset = new Vector3(
                    0.0f,
                    actionIndex * PlayerBattleDriver.ButtonDistance + PlayerBattleDriver.ButtonBaseHeight,
                    0.0f);

                // Action Selection
                actionButton.onClick.AddListener(delegate
                {
                    this.CreateTargetButtons(action);
                });
            }
        }

        /// <summary>
        ///     Creates the target buttons, given an action
        /// </summary>
        /// <param name="action">The action to create the target buttons for</param>
        private void CreateTargetButtons(BattleAction action)
        {
            if (this.targetButtonHolder != null) MonoBehaviour.Destroy(this.targetButtonHolder);

            this.targetButtonHolder = MonoBehaviour.Instantiate(MainManager.GenericPanelPrefab, MainManager.WorldCanvas.transform);

            BaseBattleDriver[][] targetChoices = action.GetTargets();

            for (int targetIndex = 0; targetIndex < targetChoices.Length; targetIndex++)
            {
                BaseBattleDriver[] targetChoice = targetChoices[targetIndex];

                this.CreateTargetButton(action, targetChoice);
            }
        }

        /// <summary>
        ///     Creates the button for a single target choice
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="targetChoice">The target choice</param>
        private void CreateTargetButton(BattleAction action, BaseBattleDriver[] targetChoice)
        {
            foreach (BaseBattleDriver target in targetChoice)
            {
                Button targetButton = MonoBehaviour.Instantiate(this.actionButtonPrefab, this.targetButtonHolder.transform);
                targetButton.GetComponentInChildren<Text>().text = action.GetTargetLabel();

                var targetButtonController = targetButton.GetComponent<UI.ButtonController>();
                targetButtonController.reference = target.transform;
                targetButtonController.positionOffset = new Vector3(
                    0.0f,
                    0.5f,
                    0.0f);

                // Target Selection
                targetButton.onClick.AddListener(delegate
                {
                    MonoBehaviour.Destroy(this.targetButtonHolder);

                    action.Use(targetChoice);
                });
            }
        }
    }
}
