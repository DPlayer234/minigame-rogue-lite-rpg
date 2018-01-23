using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SAE.RoguePG.Main.BattleDriver;

namespace SAE.RoguePG.Main.UI
{
    /// <summary>
    ///     Controls UI Health Bars...
    /// </summary>
    public class HealthController : MonoBehaviour
    {
        [HideInInspector]
        public BaseBattleDriver battleDriver;

        private Text healthText;

        private void Awake()
        {
            this.healthText = this.GetComponent<Text>();
        }

        private void FixedUpdate()
        {
            if (this.battleDriver != null)
            {
                this.healthText.text = string.Format(
                    "<color=#{4}>{0}</color> <color=#ff0000ff>{1} / {2}</color> <color=#00ffffff>[{3} AP]</color>",
                    battleDriver.name,
                    battleDriver.CurrentHealth,
                    battleDriver.MaximumHealth,
                    battleDriver.AttackPoints,
                    battleDriver.TakingTurn ? "ffff00ff" : "#ffffffff");
            }
        }
    }
}