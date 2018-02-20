namespace DPlay.RoguePG.Main
{
    using DPlay.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Handles the game over status.
    ///     Behaves as a singleton
    /// </summary>
    public class GameOverHandler : Singleton<GameOverHandler>
    {
        /// <summary>
        ///     The game over menu
        /// </summary>
        [SerializeField]
        private GameObject gameOverMenu;

        /// <summary>
        ///     Initializes the static fields and properites of the <see cref="GameOverHandler"/> class.
        /// </summary>
        static GameOverHandler()
        {
            GameOverHandler.IsGameOver = false;
        }

        /// <summary>
        ///     Whether or not the game is over.
        /// </summary>
        public static bool IsGameOver { get; private set; }

        /// <summary>
        ///     Sets the game to be over
        /// </summary>
        public static void SetGameOver()
        {
            if (GameOverHandler.Instance == null) throw new RPGException(RPGException.Cause.GameOverHandlerNoInstance);

            GameOverHandler.IsGameOver = true;
            GameOverHandler.Instance.ShowGameOverUI();

            foreach (PlayerDriver player in PlayerDriver.Party)
            {
                player.OnGameOver();
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="GameOverHandler"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            if (this.gameOverMenu == null) throw new RPGException(RPGException.Cause.MenuNoGameOver);

            this.NewPreferThis();

            GameOverHandler.IsGameOver = false;
            this.gameOverMenu.SetActive(false);
        }

        /// <summary>
        ///     Hides the game over UI.
        /// </summary>
        private void HideGameOverUI()
        {
            this.gameOverMenu.SetActive(false);
        }

        /// <summary>
        ///     Shows the game over UI.
        /// </summary>
        private void ShowGameOverUI()
        {
            this.gameOverMenu.SetActive(true);
        }
    }
}
