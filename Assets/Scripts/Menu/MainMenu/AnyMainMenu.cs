//-----------------------------------------------------------------------
// <copyright file="AnyMainMenu.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Menu.MainMenu
{
    using UnityEngine;

    /// <summary>
    ///     Base class for any main menu section.
    /// </summary>
    [DisallowMultipleComponent]
    public class AnyMainMenu : MonoBehaviour
    {
        /// <summary>
        ///     Changes to the main menu
        /// </summary>
        public void GoToMainMenu()
        {
            MainMenuManager.SetMenu(MainMenuManager.TitleMenuIndex);
        }
    }
}