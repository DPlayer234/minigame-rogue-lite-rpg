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
    public class HudManager : MonoBehaviour
    {
        /// <summary> The parent object for the exploration HUD. </summary>
        [SerializeField]
        private GameObject exploreHud;

        /// <summary> The parent object for the battle HUD. </summary>
        [SerializeField]
        private GameObject battleHud;

        /// <summary>
        ///     The global instance of the <see cref="HudManager"/>.
        /// </summary>
        public static HudManager Instance { get; private set; }

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
            if (HudManager.Instance != null)
            {
                Debug.LogWarning("There was an additional active PauseMenuManager. The new instance was destroyed.");
                MonoBehaviour.Destroy(this);
                return;
            }

            HudManager.Instance = this;

#if UNITY_EDITOR
            // Debug code... or something goes here
#endif
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