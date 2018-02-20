//-----------------------------------------------------------------------
// <copyright file="GameOverMenu.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Menu.GameOverMenu
{
    using DPlay.RoguePG.Main.Driver;
    using DPlay.RoguePG.Main.Dungeon;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    ///     The Game Over menu
    /// </summary>
    public class GameOverMenu : MonoBehaviour
    {
        /// <summary>
        ///     The format for the results display.
        /// </summary>
        [SerializeField]
        [TextArea]
        [Tooltip("{0}: Floor\n{1}: Average Character Level\n{2}: Score")]
        private string gameOverResultsFormat = "";

        /// <summary>
        ///     The text displaying the results.
        /// </summary>
        [SerializeField]
        private Text resultsText;

        /// <summary>
        ///     Return to the Main Menu scene.
        /// </summary>
        public void GoBackToMainMenu()
        {
            SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="GameOverMenu"/> whether it is active or not.
        /// </summary>
        private void Awake()
        {
            if (this.resultsText == null) throw new RPGException(RPGException.Cause.MenuMissingComponent);

            this.UpdateResultsText();
        }

        /// <summary>
        ///     Called by Unity when this object becomes active.
        /// </summary>
        private void OnEnable()
        {
            this.UpdateResultsText();
        }

        /// <summary>
        ///     Updates the results text.
        /// </summary>
        private void UpdateResultsText()
        {
            int floorNumber = DungeonGenerator.FloorNumber;

            int averageLevel = 0;
            foreach (PlayerDriver player in PlayerDriver.Party)
            {
                averageLevel += player.battleDriver.Level;
            }
            averageLevel /= PlayerDriver.Party.Count;

            int score = floorNumber * averageLevel;

            this.SetResultsText(floorNumber, averageLevel, score);
        }

        /// <summary>
        ///     Sets the results text.
        /// </summary>
        /// <param name="floorNumber">The floor number</param>
        /// <param name="averageLevel">The average level of all party members</param>
        /// <param name="score">The score</param>
        private void SetResultsText(int floorNumber, int averageLevel, int score)
        {
            try
            {
                this.resultsText.text = string.Format(
                    this.gameOverResultsFormat,
                    floorNumber,
                    averageLevel,
                    score);
            }
            catch (System.FormatException e)
            {
                Debug.LogError("The format for the game over menu is incorrect:");
                Debug.LogError(e);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Previews the content. (DEBUG)
        /// </summary>
        private void OnValidate()
        {
            if (this.resultsText == null) Debug.LogWarning("Please set the text.");

            // Arbitrary values
            this.SetResultsText(6210, 74, 6210 * 74);
        }
#endif
    }
}