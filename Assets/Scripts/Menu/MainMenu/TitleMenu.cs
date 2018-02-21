//-----------------------------------------------------------------------
// <copyright file="TitleMenu.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Menu.MainMenu
{
    using UnityEngine;

    /// <summary>
    ///     Any functions used within the main menu.
    /// </summary>
    public class TitleMenu : AnyMainMenu
    {
        /// <summary>
        ///     Switches to the character creation menu.
        /// </summary>
        public void GoToCharacterCreation()
        {
            MainMenuManager.SetMenu(MainMenuManager.CharacterCreationMenuIndex);
        }

        /// <summary>
        ///     Switches to the how to play screen.
        /// </summary>
        public void GoToHowToPlay()
        {
            MainMenuManager.SetMenu(MainMenuManager.HowToPlayScreenIndex);
        }

        /// <summary>
        ///     Switches to the credits screen.
        /// </summary>
        public void GoToCredits()
        {
            MainMenuManager.SetMenu(MainMenuManager.CreditsScreenIndex);
        }

        /// <summary>
        ///     Exits the game.
        ///     No questions asked.
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}