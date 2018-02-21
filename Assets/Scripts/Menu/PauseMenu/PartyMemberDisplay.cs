//-----------------------------------------------------------------------
// <copyright file="PartyMemberDisplay.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Menu.PauseMenu
{
    using DPlay.RoguePG.Main.BattleAction;
    using DPlay.RoguePG.Main.BattleDriver;
    using DPlay.RoguePG.Main.Driver;
    using DPlay.RoguePG.Main.UI;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Displays information about a single party member
    /// </summary>
    public class PartyMemberDisplay : MonoBehaviour
    {
        /// <summary>
        ///     The height of each element
        /// </summary>
        public const float Height = 72.0f;

        /// <summary>
        ///     The battle driver to display information about
        /// </summary>
        [HideInInspector]
        public BaseBattleDriver battleDriver;

        /// <summary>
        ///     The pause menu this is part of
        /// </summary>
        [HideInInspector]
        public PauseMenu pauseMenu;

        /// <summary> The image component </summary>
        [HideInInspector]
        public Image image;

        /// <summary> The text storing the member name </summary>
        public Text memberName;

        /// <summary> The dismissal button </summary>
        public Button dismissalButton;

        /// <summary> The button for moving a member up </summary>
        public Button moveUpButton;

        /// <summary> The button for moving a member down </summary>
        public Button moveDownButton;

        /// <summary> The text display the status (left aligned) </summary>
        public Text statusTextLeft;

        /// <summary> The text display the status (right aligned) </summary>
        public Text statusTextRight;

        /// <summary> The text displaying all actions </summary>
        public Text actionText;

        /// <summary>
        ///     The format string for the label.
        ///     {0} is the <seealso cref="BaseBattleDriver.BattleName"/> and
        ///     {1} is the <seealso cref="BaseBattleDriver.Level"/>.
        /// </summary>
        private const string LabelFormat = "{0} <color=#ffff00ff>[Lvl. {1}]</color>";

        /// <summary>
        ///     The format string used for the right of the status.
        ///     {0}/{1}: Current/Maximum Health
        ///     {2}: Physical Damage
        ///     {3}: Magical Damage
        ///     {4}: Defense
        ///     {5}: Turn Speed
        /// </summary>
        private const string StatusRightFormat = "{0}/{1}\n{2:N0}\n{3:N0}\n{4:N0}\n{5:N0}";

        /// <summary>
        ///     The text displayed on the left of the status.
        /// </summary>
        private const string StatusLeft = "Health:\nPhys.Dam.:\nMag.Dam.:\nDefense:\nTurn Spd.:";

        /// <summary>
        ///     The format string used for actions.
        ///     {0}: Action Name
        ///     {1}: Action Description
        /// </summary>
        private const string ActionFormat = "{0}\n\t<color=#999999ff>{1}</color>";

        /// <summary>
        ///     Sets all values.
        /// </summary>
        public void Setup()
        {
            if (this.battleDriver == null || this.pauseMenu == null) throw new RPGException(RPGException.Cause.MenuMissingComponent);

            this.memberName.text = string.Format(PartyMemberDisplay.LabelFormat, this.battleDriver.BattleName, this.battleDriver.Level);
            this.battleDriver.RecalculateStats();

            PlayerDriver player = this.battleDriver.entityDriver as PlayerDriver;

            // Dismiss Button
            if (PlayerDriver.Party.Count > 1)
            {
                this.dismissalButton.onClick.AddListener(delegate
                {
                    player.LeaveParty();

                    this.pauseMenu.CreateMenu();
                });
            }
            else
            {
                this.dismissalButton.interactable = false;
            }
            
            int partyIndex = PlayerDriver.Party.IndexOf(player);

            // Move up button
            if (partyIndex > 0)
            {
                this.moveUpButton.onClick.AddListener(delegate
                {
                    PlayerDriver.Party.Move(partyIndex, partyIndex - 1);

                    this.pauseMenu.CreateMenu();
                });
            }
            else
            {
                this.moveUpButton.interactable = false;
            }

            // Move down button
            if (partyIndex < PlayerDriver.Party.Count - 1)
            {
                this.moveDownButton.onClick.AddListener(delegate
                {
                    PlayerDriver.Party.Move(partyIndex, partyIndex + 1);

                    this.pauseMenu.CreateMenu();
                });
            }
            else
            {
                this.moveDownButton.interactable = false;
            }

            // Statistics
            this.statusTextLeft.text = PartyMemberDisplay.StatusLeft;

            this.statusTextRight.text = string.Format(
                PartyMemberDisplay.StatusRightFormat,
                this.battleDriver.CurrentHealth,
                this.battleDriver.MaximumHealth,
                this.battleDriver.PhysicalDamage,
                this.battleDriver.MagicalDamage,
                this.battleDriver.Defense,
                this.battleDriver.TurnSpeed);

            // Actions
            string actionText = "";
            foreach (BattleAction.ActionClass actionClass in this.battleDriver.actionClasses)
            {
                if (actionClass != BattleAction.ActionClass.NoAction)
                {
                    BattleAction action = BattleAction.GetBattleAction(actionClass, this.battleDriver);
                    actionText += (actionText.Length > 0 ? "\n" : "") + string.Format(PartyMemberDisplay.ActionFormat, action.Name, action.Description);
                }
            }

            this.actionText.text = actionText;
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="PartyMemberDisplay"/> whether it is active or not
        /// </summary>
        private void Awake()
        {
            this.image = this.GetComponent<Image>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="PartyMemberDisplay"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            if (this.memberName == null || this.dismissalButton == null ||
                this.moveUpButton == null || this.moveDownButton == null ||
                this.statusTextLeft == null || this.statusTextRight == null ||
                this.actionText == null || this.image == null)
            {
                throw new RPGException(RPGException.Cause.MenuMissingComponent);
            }
        }
    }
}