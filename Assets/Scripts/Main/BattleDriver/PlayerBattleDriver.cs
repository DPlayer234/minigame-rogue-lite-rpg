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
        ///     Button height
        /// </summary>
        private const float ButtonHeight = 30.0f;

        /// <summary>
        ///     Button width
        /// </summary>
        private const float ButtonWidth = 150.0f;

        /// <summary>
        ///     The buttons' anchor point
        ///     (Pretend this is const)
        /// </summary>
        private static Vector2 ButtonAnchorPoint = Vector2.zero;

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

            this.actionButtonHolder = MonoBehaviour.Instantiate(GenericPrefab.Panel, HudManager.BattleHud.transform);

            for (int actionIndex = 0; actionIndex < this.actions.Length; actionIndex++)
            {
                BattleAction action = this.actions[actionIndex];

                Button actionButton = MonoBehaviour.Instantiate(GenericPrefab.Button, this.actionButtonHolder.transform);
                actionButton.SetText(string.Format("{0} [{1} AP]", action.Name, action.AttackPointCost));

                actionButton.SetAnchoredPosition3D(
                    new Vector3(0.0f, (this.actions.Length - actionIndex) * PlayerBattleDriver.ButtonHeight, 0.0f),
                    PlayerBattleDriver.ButtonAnchorPoint);

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

            this.targetButtonHolder = MonoBehaviour.Instantiate(GenericPrefab.Panel, HudManager.BattleHud.transform);

            BaseBattleDriver[][] targetChoices = action.GetTargets();

            for (int targetIndex = 0; targetIndex < targetChoices.Length; targetIndex++)
            {
                BaseBattleDriver[] targetChoice = targetChoices[targetIndex];

                Button targetButton = MonoBehaviour.Instantiate(GenericPrefab.Button, this.targetButtonHolder.transform);
                targetButton.SetText(action.GetTargetLabel(targetChoice));

                targetButton.SetAnchoredPosition3D(
                    new Vector3(PlayerBattleDriver.ButtonWidth, (targetChoices.Length - targetIndex) * PlayerBattleDriver.ButtonHeight, 0.0f),
                    PlayerBattleDriver.ButtonAnchorPoint);

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
