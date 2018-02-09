namespace SAE.RoguePG.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Any functions used within the main menu.
    /// </summary>
    public class MainMenu : AnyMenu
    {
        /// <summary>
        ///     Switches to the character creation menu.
        /// </summary>
        public void GoToCharacterCreation()
        {
            MenuManager.SetMenu(MenuManager.CharacterCreationMenuIndex);
        }

        /// <summary>
        ///     Switches to the how to play screen.
        /// </summary>
        public void GoToHowToPlay()
        {
            MenuManager.SetMenu(MenuManager.HowToPlayScreenIndex);
        }

        /// <summary>
        ///     Switches to the credits screen.
        /// </summary>
        public void GoToCredits()
        {
            MenuManager.SetMenu(MenuManager.CreditsScreenIndex);
        }

        /// <summary>
        ///     Exits the game.
        ///     No questions asked.
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = true;
#else
            Application.Quit();
#endif
        }
    }
}