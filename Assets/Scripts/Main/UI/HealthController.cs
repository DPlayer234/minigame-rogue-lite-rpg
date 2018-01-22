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
                this.healthText.text = string.Format("{0}: {1} / {2}", battleDriver.name, battleDriver.CurrentHealth, battleDriver.MaximumHealth);
            }
        }
    }
}