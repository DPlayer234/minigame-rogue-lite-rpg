namespace SAE.RoguePG.Main
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Manages the menus
    /// </summary>
    [DisallowMultipleComponent]
    public class HudManager : Singleton<HudManager>
    {
        /// <summary> The parent object for the exploration HUD. </summary>
        [SerializeField]
        private GameObject exploreHud;

        /// <summary> The parent object for the battle HUD. </summary>
        [SerializeField]
        private GameObject battleHud;

        /// <summary>
        ///     The parent object for the exploration HUD.
        /// </summary>
        public static GameObject ExploreHud { get { return HudManager.Instance.exploreHud; } }

        /// <summary>
        ///     The parent object for the battle HUD.
        /// </summary>
        public static GameObject BattleHud { get { return HudManager.Instance.battleHud; } }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="HudManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.NewInstance();
        }

        /// <summary>
        ///     Called by Unity once every fixed update to update the <seealso cref="HudManager"/>
        /// </summary>
        private void FixedUpdate()
        {
            if (BattleManager.IsBattleActive && HudManager.ExploreHud.activeSelf)
            {
                HudManager.ExploreHud.SetActive(false);
                HudManager.BattleHud.SetActive(true);
            }
            else if (!BattleManager.IsBattleActive && HudManager.BattleHud.activeSelf)
            {
                HudManager.ExploreHud.SetActive(true);
                HudManager.BattleHud.SetActive(false);
            }
        }
    }
}