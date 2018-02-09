namespace SAE.RoguePG.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Manages the menus
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        /// <summary> An array of parent objects for menus </summary>
        [SerializeField]
        private GameObject[] menus;

        /// <summary> The index for the main menu </summary>
        [SerializeField]
        private int mainMenuIndex;

        /// <summary> The index for the character creation menu </summary>
        [SerializeField]
        private int characterCreationMenuIndex;

        /// <summary> The index for the how to play screen </summary>
        [SerializeField]
        private int howToPlayScreenIndex;

        /// <summary> The index for the credits screen </summary>
        [SerializeField]
        private int creditsScreenIndex;

        /// <summary>
        ///     The global instance of the <see cref="MenuManager"/>.
        /// </summary>
        public static MenuManager Instance { get; private set; }

        /// <summary> The index for the main menu </summary>
        public static int MainMenuIndex
        {
            get
            {
                return MenuManager.Instance.mainMenuIndex;
            }
        }

        /// <summary> The index for the character creation menu </summary>
        public static int CharacterCreationMenuIndex
        {
            get
            {
                return MenuManager.Instance.characterCreationMenuIndex;
            }
        }

        /// <summary> The index for the how to play screen </summary>
        public static int HowToPlayScreenIndex
        {
            get
            {
                return MenuManager.Instance.howToPlayScreenIndex;
            }
        }

        /// <summary> The index for the credits screen </summary>
        public static int CreditsScreenIndex
        {
            get
            {
                return MenuManager.Instance.creditsScreenIndex;
            }
        }

        /// <summary>
        ///     Sets a menu.
        /// </summary>
        /// <param name="index">The index of the menu within the menu list</param>
        public static void SetMenu(int index)
        {
            for (int i = 0; i < MenuManager.Instance.menus.Length; i++)
            {
                MenuManager.Instance.menus[i].SetActive(i == index);
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="MenuManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            if (MenuManager.Instance != null)
            {
                Debug.LogWarning("There was an additional active MenuManager. The new instance was destroyed.");
                Destroy(this);
                return;
            }

            MenuManager.Instance = this;

            this.ValidateSetup();

#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
        }

        /// <summary>
        ///     Validates that everything is correctly setup and throws an exception otherwise.
        /// </summary>
        private void ValidateSetup()
        {

        }
    }
}