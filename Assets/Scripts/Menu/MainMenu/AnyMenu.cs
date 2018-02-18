namespace SAE.RoguePG.Menu.MainMenu
{
    using System.Collections;
    using System.Collections.Generic;
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