namespace SAE.RoguePG.Menu.PauseMenu
{
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using SAE.RoguePG.Main.UI;
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
        public const float Height = 100.0f;

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

        /// <summary> The text storing the member name </summary>
        public Text memberName;

        /// <summary> The dismissal button </summary>
        public Button dismissalButton;

        /// <summary> The button for moving a member up </summary>
        public Button moveUpButton;

        /// <summary> The button for moving a member down </summary>
        public Button moveDownButton;

        /// <summary> The text displaying all main stats </summary>
        public Text statText;

        /// <summary> The text display level and current health </summary>
        public Text statusText;

        /// <summary>
        ///     Sets all values.
        /// </summary>
        public void Setup()
        {
            if (this.battleDriver == null || this.pauseMenu == null) throw new RPGException(RPGException.Cause.MenuMissingComponent);

            this.memberName.text = this.battleDriver.BattleName;

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
            // TODO
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="PartyMemberDisplay"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            if (this.memberName == null || this.dismissalButton == null ||
                this.moveUpButton == null || this.moveDownButton == null ||
                this.statText == null || this.statusText == null)
            {
                throw new RPGException(RPGException.Cause.MenuMissingComponent);
            }
        }
    }
}