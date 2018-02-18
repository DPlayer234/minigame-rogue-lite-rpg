namespace SAE.RoguePG.Main.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public static class ButtonExtension
    {
        /// <summary>
        ///     Sets the text on a button
        /// </summary>
        /// <param name="button">A button</param>
        /// <param name="text">The text</param>
        public static void SetText(this Button button, string text)
        {
            button.GetComponentInChildren<Text>().text = text;
        }

        /// <summary>
        ///     Sets the anchored position of a button
        /// </summary>
        /// <param name="button">A button</param>
        /// <param name="position">The position</param>
        public static void SetAnchoredPosition3D(this Button button, Vector3 position)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition3D = position;
            }
        }

        /// <summary>
        ///     Sets the anchored position of a button
        /// </summary>
        /// <param name="button">A button</param>
        /// <param name="position">The position</param>
        /// <param name="anchor">The anchor point</param>
        public static void SetAnchoredPosition3D(this Button button, Vector3 position, Vector2 anchor)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                rectTransform.anchorMax = anchor;
                rectTransform.anchorMin = anchor;
                rectTransform.anchoredPosition3D = position;
            }
        }
    }
}