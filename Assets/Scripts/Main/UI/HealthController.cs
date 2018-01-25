namespace SAE.RoguePG.Main.UI
{
    using SAE.RoguePG.Main.BattleDriver;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Controls UI Health Bars...
    /// </summary>
    public class HealthController : MonoBehaviour
    {
        /// <summary> The <seealso cref="BaseBattleDriver"/> to display the information of </summary>
        [HideInInspector]
        public BaseBattleDriver battleDriver;

        /// <summary> The <seealso cref="Text"/> object to modify </summary>
        private Text healthText;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="HealthController"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.healthText = this.GetComponent<Text>();
        }

        /// <summary>
        ///     Called by Unity every fixed update to update the <seealso cref="HealthController"/>
        /// </summary>
        private void FixedUpdate()
        {
            if (this.battleDriver != null)
            {
                this.healthText.text = string.Format(
                    "<color=#{4}>{0}</color> <color=#ff0000ff>{1} / {2}</color> <color=#00ffffff>[{3} AP]</color>",
                    this.battleDriver.name,
                    this.battleDriver.CurrentHealth,
                    this.battleDriver.MaximumHealth,
                    this.battleDriver.AttackPoints,
                    this.battleDriver.TakingTurn ? "ffff00ff" : "#ffffffff");
            }
        }
    }
}