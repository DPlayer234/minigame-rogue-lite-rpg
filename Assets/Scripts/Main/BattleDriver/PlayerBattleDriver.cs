using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SAE.RoguePG.Main.Driver;
using SAE.RoguePG.Main.BattleActions;

namespace SAE.RoguePG.Main.BattleDriver
{
    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerBattleDriver : BaseBattleDriver
    {
        /// <summary> Prefab for action buttons </summary>
        [SerializeField]
        private Button actionButtonPrefab;

        private GameObject actionButtonHolder;
        private GameObject targetButtonHolder;

        /// <summary> The <seealso cref="PlayerDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        private PlayerDriver playerDriver;

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
            
            // TODO: Make this prettier, split it up
            actionButtonHolder = Instantiate(MainManager.GenericPanelPrefab, MainManager.BattleHud.transform);

            for (int actionIndex = 0; actionIndex < this.actions.Length; actionIndex++)
            {
                BattleAction action = this.actions[actionIndex];

                Button actionButton = Instantiate(actionButtonPrefab, actionButtonHolder.transform);
                actionButton.GetComponentInChildren<Text>().text = string.Format("{0} [{1} AP]", action.Name, action.AttackPointCost);
                actionButton.transform.localPosition += new Vector3(
                    0.0f,
                    actionIndex * 30,
                    0.0f);

                // Action Selection
                actionButton.onClick.AddListener(delegate ()
                {
                    if (targetButtonHolder != null) Destroy(targetButtonHolder);

                    targetButtonHolder = Instantiate(MainManager.GenericPanelPrefab, MainManager.BattleHud.transform);

                    BaseBattleDriver[][] targetChoices = action.GetTargets();

                    for (int targetIndex = 0; targetIndex < targetChoices.Length; targetIndex++)
                    {
                        BaseBattleDriver[] targetChoice = targetChoices[targetIndex];

                        string label = "> ";
                        foreach (BaseBattleDriver target in targetChoice)
                        {
                            label += target.name + " ";
                        }

                        Button targetButton = Instantiate(actionButtonPrefab, targetButtonHolder.transform);
                        targetButton.GetComponentInChildren<Text>().text = label;
                        targetButton.transform.localPosition += new Vector3(
                            250.0f,
                            targetIndex * 30,
                            0.0f);

                        // Target Selection
                        targetButton.onClick.AddListener(delegate ()
                        {
                            Destroy(targetButtonHolder);

                            action.Use(targetChoice);
                            StartCoroutine(this.JumpForward());
                        });
                    }
                });
            }
        }

        /// <summary>
        ///     Ends the Player's turn
        /// </summary>
        public override void EndTurn()
        {
            base.EndTurn();

            if (actionButtonHolder != null) Destroy(actionButtonHolder);
            if (targetButtonHolder != null) Destroy(targetButtonHolder);
        }

        /// <summary>
        ///     Updates the Player's turn once a frame
        /// </summary>
        public override void UpdateTurn()
        {
            base.UpdateTurn();

            if (this.AttackPoints <= 0)
            {
                if (actionButtonHolder != null) Destroy(actionButtonHolder);
                if (targetButtonHolder != null) Destroy(targetButtonHolder);

                this.TakingTurn = false;
            }
        }

        /// <summary>
        ///     Updates the Player once a frame while nothing is taking a turn
        /// </summary>
        public override void UpdateIdle()
        {
            base.UpdateIdle();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> whether it is or is not active.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            this.playerDriver = this.GetComponent<PlayerDriver>();
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
    }
}
