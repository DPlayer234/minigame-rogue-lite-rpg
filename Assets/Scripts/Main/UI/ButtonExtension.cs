namespace SAE.RoguePG.Main.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Contains extension methods to modify buttons
    /// </summary>
    public static class ButtonExtension
    {
        /// <summary>
        ///     Sets the text on a button
        /// </summary>
        /// <param name="button">The button</param>
        /// <param name="text">The text</param>
        public static void SetText(this Button button, string text)
        {
            button.GetComponentInChildren<Text>().text = text;
        }

        /// <summary>
        ///     Sets up (or assigns) a <seealso cref="ButtonController"/> to the button.
        /// </summary>
        /// <param name="button">The button</param>
        /// <param name="targetTransform">The target transform to move relative to</param>
        /// <param name="height">The relative height</param>
        public static void SetupButtonController(this Button button, Transform targetTransform, float height)
        {
            var buttonController = button.GetComponent<ButtonController>();
            if (buttonController == null)
            {
                buttonController = button.gameObject.AddComponent<ButtonController>();
            }

            buttonController.reference = targetTransform;
            buttonController.positionOffset = new Vector3(
                0.0f,
                height,
                0.0f);
        }
    }
}
