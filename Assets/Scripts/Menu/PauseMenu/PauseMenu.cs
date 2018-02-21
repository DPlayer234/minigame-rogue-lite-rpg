//-----------------------------------------------------------------------
// <copyright file="PauseMenu.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Menu.PauseMenu
{
    using DPlay.RoguePG.Main.Driver;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    ///     The pause menu.
    /// </summary>
    [DisallowMultipleComponent]
    public class PauseMenu : MonoBehaviour
    {
        /// <summary>
        ///     The color addition for every 2nd element (per channel)
        /// </summary>
        private const float ColorAdd2nd = 0.3f;

        /// <summary>
        ///     The actual pause menu root (may not be this object)
        /// </summary>
        [SerializeField]
        private GameObject pauseMenu;

        /// <summary>
        ///     The prefab used for constructing the party member displays
        /// </summary>
        [SerializeField]
        private PartyMemberDisplay partyMemberDisplayPrefab;

        /// <summary>
        ///     The last party size registered.
        /// </summary>
        private int updatedCount = 0;

        /// <summary>
        ///     All party member displays.
        /// </summary>
        private PartyMemberDisplay[] partyMemberDisplays;

        /// <summary>
        ///     Return to the Main Menu scene.
        /// </summary>
        public void GoBackToMainMenu()
        {
            SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        }

        /// <summary>
        ///     Creates the menu
        /// </summary>
        public void CreateMenu()
        {
            this.updatedCount = PlayerDriver.Party.Count;

            // Remove old menu
            if (this.partyMemberDisplays != null)
            {
                foreach (PartyMemberDisplay partyMemberDisplay in this.partyMemberDisplays)
                {
                    if (partyMemberDisplay != null)
                    {
                        MonoBehaviour.Destroy(partyMemberDisplay.gameObject);
                    }
                }
            }

            // Create new menu
            float height = PartyMemberDisplay.Height * (PlayerDriver.Party.Count - 1) * 0.5f;

            this.partyMemberDisplays = new PartyMemberDisplay[PlayerDriver.Party.Count];
            int index = 0;

            foreach (PlayerDriver player in PlayerDriver.Party)
            {
                if (player == null) continue;

                PartyMemberDisplay partyMemberDisplay = MonoBehaviour.Instantiate(this.partyMemberDisplayPrefab, this.pauseMenu.transform);

                partyMemberDisplay.battleDriver = player.battleDriver;
                partyMemberDisplay.pauseMenu = this;

                partyMemberDisplay.transform.localPosition = new Vector3(0.0f, height, 0.0f);

                if (index % 2 == 1)
                {
                    partyMemberDisplay.image.color += new Color(
                        PauseMenu.ColorAdd2nd,
                        PauseMenu.ColorAdd2nd,
                        PauseMenu.ColorAdd2nd,
                        0.0f);
                }

                partyMemberDisplay.Setup();

                this.partyMemberDisplays[index++] = partyMemberDisplay;
                height -= PartyMemberDisplay.Height;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PauseMenu"/> whether it is active or not
        /// </summary>
        private void Awake()
        {
            if (this.pauseMenu == null) throw new RPGException(RPGException.Cause.MenuNoPause);

            this.pauseMenu.SetActive(false);
        }

        /// <summary>
        ///     Called by Unity every frame to update the <seealso cref="PauseMenu"/>
        /// </summary>
        private void Update()
        {
            // Toggle menu status
            if (Input.GetButtonDown("Start"))
            {
                this.pauseMenu.SetActive(!this.pauseMenu.activeSelf);

                if (this.pauseMenu.activeSelf)
                {
                    this.CreateMenu();
                }
            }

            if (this.pauseMenu.activeSelf)
            {
                if (this.updatedCount != PlayerDriver.Party.Count)
                {
                    this.CreateMenu();
                }
            }
        }

        /// <summary>
        ///     Called by Unity when this behaviour is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (this.pauseMenu.activeSelf)
            {
                this.CreateMenu();
            }
        }
    }
}