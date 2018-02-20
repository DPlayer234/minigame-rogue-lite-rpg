namespace DPlay.RoguePG.Main
{
    using System.Collections;
    using DPlay.RoguePG.Main.BattleDriver;
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
        ///     The intensity of highlights
        /// </summary>
        private const float Intensity = 50.0f;

        /// <summary> The found light source used for highlighting </summary>
        new private Light light;

        /// <summary> Coroutine used to activate and deactivate the <seealso cref="Highlighter"/> after calling <seealso cref="Enable(float)"/> </summary>
        private Coroutine disablingCoroutine;

        /// <summary> Color used for battle targets </summary>
        public static Color TargetColor { get { return Color.red; } }

        /// <summary> Color used for active fighters in battle </summary>
        public static Color ActiveColor { get { return Color.green; } }

        /// <summary>
        ///     Gets whether or not the highlight is enabled (not the behaviour)
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.light.enabled && this.light.gameObject.activeSelf;
            }
        }

        /// <summary>
        ///     Gets the color of the highlight
        /// </summary>
        public Color Color
        {
            get
            {
                return this.light.color;
            }
        }

        /// <summary>
        ///     Enables or changes the highlight.
        /// </summary>
        /// <param name="intensity">The light intensity</param>
        public void Enable()
        {
            this.StopDeactivationCoroutine();

            this.light.gameObject.SetActive(true);

            this.light.enabled = true;
        }

        /// <summary>
        ///     Enables or changes the highlight.
        /// </summary>
        /// <param name="color">The color for the light</param>
        /// <param name="intensity">The light intensity</param>
        public void Enable(Color color)
        {
            this.Enable();
            this.light.color = color;
        }

        /// <summary>
        ///     Enables the highlight and leaves it enabled for the <paramref name="duration"/> in seconds.
        /// </summary>
        /// <param name="color">The color for the light</param>
        /// <param name="duration">The duration in seconds</param>
        /// <param name="intensity">The light intensity</param>
        public void Enable(Color color, float duration)
        {
            this.Enable(duration);
            this.light.color = color;
        }

        /// <summary>
        ///     Enables the highlight and leaves it enabled for the <paramref name="duration"/> in seconds.
        /// </summary>
        /// <param name="duration">The duration in seconds</param>
        /// <param name="intensity">The light intensity</param>
        public void Enable(float duration)
        {
            this.Enable();
            this.StartDisablingCoroutine(duration);
        }

        /// <summary>
        ///     Disables the highlight.
        /// </summary>
        public void Disable()
        {
            this.StopDeactivationCoroutine();
            this.light.enabled = false;
            this.light.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Starts the disabling coroutine with the given <paramref name="waitFor"/>
        /// </summary>
        /// <param name="waitFor">The time to wait for until it is disabled</param>
        private void StartDisablingCoroutine(float waitFor)
        {
            this.disablingCoroutine = this.StartCoroutine(this.DisableAfter(waitFor));
        }

        /// <summary>
        ///     When run as a coroutine, will disable the highlight after the <paramref name="waitTime"/> in seconds.
        /// </summary>
        /// <param name="waitTime"></param>
        private IEnumerator DisableAfter(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            this.disablingCoroutine = null;
            this.Disable();
        }

        /// <summary>
        ///     Stops the <seealso cref="disablingCoroutine"/> if there is one.
        /// </summary>
        private void StopDeactivationCoroutine()
        {
            if (this.disablingCoroutine != null)
            {
                this.StopCoroutine(this.disablingCoroutine);
                this.disablingCoroutine = null;
            }
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

            this.light.intensity = Highlighter.Intensity;
            this.light.bounceIntensity = 0.0f;
            this.light.range = this.light.transform.localPosition.y * 2.0f;
            
            this.Disable();
        }
    }
}
