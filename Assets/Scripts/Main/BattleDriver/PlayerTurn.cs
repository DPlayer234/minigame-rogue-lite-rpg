namespace SAE.RoguePG.Main.BattleDriver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SAE.RoguePG.Main.BattleAction;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using SAE.RoguePG.Main.UI;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Actions to be used during a player's turn
    /// </summary>
    public class PlayerTurn
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

        /// <summary> The action buttons </summary>
        private Button[] actionButtons;

        /// <summary> The target buttons </summary>
        private Button[] targetButtons;

        /// <summary> Current battle target choices </summary>
        private BaseBattleDriver[][] targetChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerTurn"/> class.
        /// </summary>
        /// <param name="actions">The actions to generate buttons for</param>
        public PlayerTurn(BattleAction[] actions)
        {
            this.Actions = actions;

            this.CreateActionButtons();
        }

        /// <summary>
        ///     The actions to generate buttons for
        /// </summary>
        public BattleAction[] Actions { get; private set; }

        /// <summary>
        ///     Destroy all action buttons.
        /// </summary>
        public void DestroyActionButtons()
        {
            if (this.actionButtonHolder != null) MonoBehaviour.Destroy(this.actionButtonHolder);
            if (this.actionButtons != null) this.actionButtons = null;

            this.DestroyTargetButtons();
        }

        /// <summary>
        ///     Creates the the buttons for all actions
        /// </summary>
        public void CreateActionButtons()
        {
            this.DestroyActionButtons();

            this.actionButtons = new Button[this.Actions.Length];
            this.actionButtonHolder = MonoBehaviour.Instantiate(GenericPrefab.Panel, HudManager.BattleHud.transform);

            for (int i = 0; i < this.Actions.Length; i++)
            {
                int actionIndex = i;
                BattleAction action = this.Actions[actionIndex];

                Button actionButton = MonoBehaviour.Instantiate(GenericPrefab.Button, this.actionButtonHolder.transform);
                actionButton.SetText(string.Format("{0} [{1} AP]", action.Name, action.AttackPointCost));

                actionButton.SetAnchoredPosition3D(
                    new Vector3(0.0f, (this.Actions.Length - actionIndex) * PlayerTurn.ButtonHeight, 0.0f),
                    PlayerTurn.ButtonAnchorPoint);

                // Action Selection
                actionButton.onClick.AddListener(delegate
                {
                    this.CreateTargetButtons(action);

                    for (int j = 0; j < this.actionButtons.Length; j++)
                    {
                        this.actionButtons[j].interactable = j != actionIndex;
                    }
                });

                this.actionButtons[actionIndex] = actionButton;
            }
        }
        
        /// <summary>
        ///     Destroy all target buttons.
        /// </summary>
        private void DestroyTargetButtons()
        {
            if (this.targetButtonHolder != null) MonoBehaviour.Destroy(this.targetButtonHolder);
            if (this.targetButtons != null) this.targetButtons = null;
        }

        /// <summary>
        ///     Creates the target buttons, given an action
        /// </summary>
        /// <param name="action">The action to create the target buttons for</param>
        private void CreateTargetButtons(BattleAction action)
        {
            this.DestroyTargetButtons();

            this.targetChoices = action.GetTargets();

            this.targetButtons = new Button[this.targetChoices.Length];
            this.targetButtonHolder = MonoBehaviour.Instantiate(GenericPrefab.Panel, HudManager.BattleHud.transform);

            for (int choiceIndex = 0; choiceIndex < this.targetChoices.Length; choiceIndex++)
            {
                BaseBattleDriver[] targetChoice = this.targetChoices[choiceIndex];

                Button targetButton = MonoBehaviour.Instantiate(GenericPrefab.Button, this.targetButtonHolder.transform);
                targetButton.SetText(action.GetTargetLabel(targetChoice));

                targetButton.SetAnchoredPosition3D(
                    new Vector3(PlayerTurn.ButtonWidth, (this.targetChoices.Length - choiceIndex) * PlayerTurn.ButtonHeight, 0.0f),
                    PlayerTurn.ButtonAnchorPoint);

                // Button to finalize a selection
                targetButton.onClick.AddListener(delegate
                {
                    this.DestroyTargetButtons();

                    foreach (Button actionButton in this.actionButtons)
                    {
                        actionButton.interactable = true;
                    }

                    action.Use(targetChoice);
                });

                this.targetButtons[choiceIndex] = targetButton;
            }
        }

        /// <summary>
        ///     Adds a highlight to the targets
        /// </summary>
        /// <param name="targets">The targets</param>
        private void AddTargetHighlight(BaseBattleDriver[] targets)
        {
            foreach (BaseBattleDriver target in targets)
            {
                target.highlight.Enable(Highlighter.TargetColor);
            }
        }
    }
}
