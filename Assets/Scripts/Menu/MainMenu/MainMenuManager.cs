namespace DPlay.RoguePG.Menu.MainMenu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Manages the menus
    /// </summary>
    [DisallowMultipleComponent]
    public class MainMenuManager : MonoBehaviour
    {
        [Header("List Of Menus")]

        /// <summary> An array of parent objects for menus </summary>
        [SerializeField]
        private GameObject[] menus;

        [Header("Indices Of The List")]

        /// <summary> Index of the first menu to open </summary>
        [SerializeField]
        private int firstOpenedMenuIndex;

        /// <summary> The index for the main menu </summary>
        [SerializeField]
        private int titleMenuIndex;

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
        ///     The global instance of the <see cref="MainMenuManager"/>.
        /// </summary>
        public static MainMenuManager Instance { get; private set; }

        /// <summary> The index for the main menu </summary>
        public static int TitleMenuIndex
        {
            get
            {
                return MainMenuManager.Instance.titleMenuIndex;
            }
        }

        /// <summary> The index for the character creation menu </summary>
        public static int CharacterCreationMenuIndex
        {
            get
            {
                return MainMenuManager.Instance.characterCreationMenuIndex;
            }
        }

        /// <summary> The index for the how to play screen </summary>
        public static int HowToPlayScreenIndex
        {
            get
            {
                return MainMenuManager.Instance.howToPlayScreenIndex;
            }
        }

        /// <summary> The index for the credits screen </summary>
        public static int CreditsScreenIndex
        {
            get
            {
                return MainMenuManager.Instance.creditsScreenIndex;
            }
        }

        /// <summary>
        ///     Sets a menu.
        /// </summary>
        /// <param name="index">The index of the menu within the menu list</param>
        public static void SetMenu(int index)
        {
            for (int i = 0; i < MainMenuManager.Instance.menus.Length; i++)
            {
                MainMenuManager.Instance.menus[i].SetActive(i == index);
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="MainMenuManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            if (MainMenuManager.Instance != null)
            {
                Debug.LogWarning("There was an additional active MainMenuManager. The new instance was destroyed.");
                MonoBehaviour.Destroy(this);
                return;
            }

            MainMenuManager.Instance = this;

            MainMenuManager.SetMenu(this.firstOpenedMenuIndex);

#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
        }
    }
}