namespace SAE.RoguePG.Main.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleDriver;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Controls the information displayed in a StatusDisplay
    /// </summary>
    public class StatusDisplayController : MonoBehaviour
    {
        /// <summary> The tag used by the label </summary>
        private const string LabelTag = "StatusDisplayLabel";

        /// <summary> The tag used by the health bar elements </summary>
        private const string HealthBarTag = "StatusDisplayHealth";

        /// <summary> The tag used by the AP bar elements </summary>
        private const string APBarTag = "StatusDisplayAP";

        /// <summary>
        ///     The <seealso cref="BaseBattleDriver"/> to display the information of.
        ///     It is assumed to be a component of any parent.
        /// </summary>
        private BaseBattleDriver battleDriver;

        /// <summary> The label <seealso cref="TextMesh"/> </summary>
        private TextMesh label;

        /// <summary> The health bar <seealso cref="Transform"/> </summary>
        private Transform healthBar;

        /// <summary> The health bar label <seealso cref="TextMesh"/> </summary>
        private TextMesh healthLabel;

        /// <summary> The AP bar <seealso cref="Transform"/> </summary>
        private Transform apBar;

        /// <summary> The AP bar label <seealso cref="TextMesh"/> </summary>
        private TextMesh apLabel;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="StatusDisplayController"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.battleDriver = this.GetComponentInParent<BaseBattleDriver>();

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);

                if (child.CompareTag(LabelTag))
                {
                    // Found label
                    this.label = child.GetComponent<TextMesh>();
                }
                else if (child.CompareTag(HealthBarTag))
                {
                    // Found Health Bar Part
                    this.AssignTransformOrMesh(child, ref this.healthBar, ref this.healthLabel);
                }
                else if (child.CompareTag(APBarTag))
                {
                    // Found AP Bar Part
                    this.AssignTransformOrMesh(child, ref this.apBar, ref this.apLabel);
                }
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="StatusDisplayController"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            this.Validate();
        }

        /// <summary>
        ///     Assigns a <seealso cref="Transform"/> or <seealso cref="TextMesh"/>
        /// </summary>
        /// <param name="transform">The transform to search</param>
        /// <param name="transformTo">The variable to assign the transform to</param>
        /// <param name="mesh">The variable to assign the mesh to</param>
        private void AssignTransformOrMesh(Transform transform, ref Transform transformTo, ref TextMesh mesh)
        {
            TextMesh assignedMesh = transform.GetComponent<TextMesh>();

            if (assignedMesh == null)
            {
                transformTo = transform;
            }
            else
            {
                mesh = assignedMesh;
            }
        }

        /// <summary>
        ///     Validates that everything has been correctly assigns and throws an exception if not.
        /// </summary>
        private void Validate()
        {
            if (this.label == null || this.healthBar == null || this.healthLabel == null || this.apBar == null || this.apLabel == null)
            {
                throw new RPGException(RPGException.Cause.StatusDisplayMissingComponent);
            }

            if (this.battleDriver == null) throw new RPGException(RPGException.Cause.StatusDisplayNoBattleDriver);
        }

        /// <summary>
        ///     Updates a bar made up of <paramref name="bar"/> and <paramref name="textMesh"/>.
        /// </summary>
        /// <param name="bar">The <seealso cref="Transform"/> of the used bar</param>
        /// <param name="textMesh">The <seealso cref="TextMesh"/> of the used bar</param>
        /// <param name="currentValue">The current value of the bar</param>
        /// <param name="maximumValue">The maximum value of the bar</param>
        private void UpdateBar(Transform bar, TextMesh textMesh, float currentValue, float maximumValue)
        {
            // Update width of the bar
            bar.transform.localScale = new Vector3(Mathf.Clamp01(currentValue / maximumValue), 1.0f, 1.0f);

            // Update the associated display text
            textMesh.text = string.Format("{0}/{1}", Mathf.RoundToInt(currentValue), Mathf.RoundToInt(maximumValue));
        }

        /// <summary>
        ///     Called by Unity every fixed update to update the <seealso cref="StatusDisplayController"/>
        /// </summary>
        private void FixedUpdate()
        {
            if (this.battleDriver != null)
            {
                this.label.text = string.Format("{0} Lv. {1}", this.battleDriver.battleName, this.battleDriver.Level);

                this.UpdateBar(this.healthBar, this.healthLabel, this.battleDriver.CurrentHealth, this.battleDriver.MaximumHealth);
                this.UpdateBar(this.apBar, this.apLabel, this.battleDriver.AttackPoints, BaseBattleDriver.MaximumAttackPoints);
            }
        }
    }
}