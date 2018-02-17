namespace SAE.RoguePG.Main.BattleDriver
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleAction;
    using SAE.RoguePG.Main.Driver;
    using SAE.RoguePG.Main.UI;
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

            this.DestroyActionButtons();
        }

        /// <summary>
        ///     Updates the Player's turn once a frame
        /// </summary>
        public override void UpdateTurn()
        {
            base.UpdateTurn();

            if (this.AttackPoints <= 0)
            {
                this.DestroyActionButtons();

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
        ///     Destory all action and target buttons.
        /// </summary>
        private void DestroyActionButtons()
        {
            if (this.actionButtonHolder != null) MonoBehaviour.Destroy(this.actionButtonHolder);
            if (this.targetButtonHolder != null) MonoBehaviour.Destroy(this.targetButtonHolder);
        }

        /// <summary>
        ///     Creates the the buttons for all actions
        /// </summary>
        private void CreateActionButtons()
        {
            this.DestroyActionButtons();

            this.actionButtonHolder = MonoBehaviour.Instantiate(MainManager.GenericPanelPrefab, MainManager.WorldCanvas.transform);

            for (int actionIndex = 0; actionIndex < this.actions.Length; actionIndex++)
            {
                BattleAction action = this.actions[actionIndex];

                Button actionButton = MonoBehaviour.Instantiate(MainManager.GenericButtonPrefab, this.actionButtonHolder.transform);
                actionButton.SetText(string.Format("{0} [{1} AP]", action.Name, action.AttackPointCost));
                
                actionButton.SetupButtonController(this.transform, actionIndex * PlayerBattleDriver.ButtonDistance + PlayerBattleDriver.ButtonBaseHeight);

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
            
            foreach (BaseBattleDriver[] targetChoice in action.GetTargets())
            {
                this.CreateTargetChoiceButtons(action, targetChoice);
            }
        }

        /// <summary>
        ///     Creates the button for a single target choice
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="targetChoice">The target choice</param>
        private void CreateTargetChoiceButtons(BattleAction action, BaseBattleDriver[] targetChoice)
        {
            foreach (BaseBattleDriver target in targetChoice)
            {
                Button targetButton = MonoBehaviour.Instantiate(MainManager.GenericButtonPrefab, this.targetButtonHolder.transform);
                targetButton.SetText(action.GetTargetLabel());
                
                targetButton.SetupButtonController(target.transform, 0.5f);

                // Button to finalize a selection
                targetButton.onClick.AddListener(delegate
                {
                    MonoBehaviour.Destroy(this.targetButtonHolder);

                    action.Use(targetChoice);
                });
            }
        }
    }
}
