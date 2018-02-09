namespace SAE.RoguePG.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Base class for any menu.
    /// </summary>
    [DisallowMultipleComponent]
    public class AnyMenu : MonoBehaviour
    {
        /// <summary>
        ///     Changes to the main menu
        /// </summary>
        public void GoToMainMenu()
        {
            MenuManager.SetMenu(MenuManager.MainMenuIndex);
        }
    }
}