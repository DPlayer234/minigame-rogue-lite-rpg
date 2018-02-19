namespace SAE.RoguePG.Main.Driver
{
    using SAE.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Allows highlighting something
    /// </summary>
    public class Highlighter : MonoBehaviour
    {
        /// <summary>
        ///     The tag used by the object holding the <seealso cref="Light"/>
        /// </summary>
        private const string HighlightTag = "Highlight";

        /// <summary>
        ///     The default intensity if none is set.
        /// </summary>
        private const float DefaultIntensity = 2.0f;

        /// <summary> The found light source used for highlighting </summary>
        new private Light light;

        /// <summary> Color used for battle targets </summary>
        public static Color TargetColor { get { return Color.red; } }

        /// <summary> Color used for active fighters in battle </summary>
        public static Color ActiveColor { get { return Color.green; } }

        /// <summary>
        ///     Highlights all entities as targets
        /// </summary>
        public static void HighlightBattleTargets(BaseBattleDriver[] battleDrivers)
        {
            foreach (BaseBattleDriver battleDriver in battleDrivers)
            {
                battleDriver.highlight.Enable(Highlighter.TargetColor);
            }
        }

        /// <summary>
        ///     Removes the highlighting from all battle drivers
        /// </summary>
        public static void RemoveBattleHighlights(BaseBattleDriver[] battleDrivers)
        {
            foreach (BaseBattleDriver battleDriver in battleDrivers)
            {
                battleDriver.highlight.Disable();
            }
        }

        /// <summary>
        ///     Enables or changes the highlight.
        /// </summary>
        /// <param name="color">The color for the light</param>
        public void Enable(Color color, float intensity = Highlighter.DefaultIntensity)
        {
            this.light.gameObject.SetActive(true);

            this.light.enabled = true;
            this.light.color = color;
            this.light.intensity = intensity;
        }

        /// <summary>
        ///     Disables the highlight.
        /// </summary>
        public void Disable()
        {
            this.light.enabled = false;
            this.light.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="Highlighter"/> whether it is active or not.
        /// </summary>
        private void Awake()
        {
            Light[] lights = this.GetComponentsInChildren<Light>();

            if (lights.Length < 1) throw new RPGException(RPGException.Cause.HighlightNoLight);

            foreach (Light light in lights)
            {
                if (light.CompareTag(Highlighter.HighlightTag))
                {
                    if (light.gameObject == this.gameObject) throw new RPGException(RPGException.Cause.HighlightOnGameObject);

                    this.light = light;
                    break;
                }
            }

            if (this.light == null) throw new RPGException(RPGException.Cause.HighlightNotFound);
            
            this.Disable();
        }
    }
}
