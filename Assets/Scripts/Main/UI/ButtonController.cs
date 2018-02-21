//-----------------------------------------------------------------------
// <copyright file="ButtonController.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Controls the position of a button
    /// </summary>
    public class ButtonController : MonoBehaviour
    { 
        /// <summary>
        ///     In what to stuff. (I'm tired)
        /// </summary>
        [HideInInspector]
        public Transform reference;

        /// <summary>
        ///     Local position offset
        /// </summary>
        [HideInInspector]
        public Vector3 positionOffset;

        /// <summary>
        ///     How many units closer should the button be
        /// </summary>
        private const float CloserBy = 0.25f;

        /// <summary>
        ///     Unity event that is triggered when the button is clicked
        /// </summary>
        public Button.ButtonClickedEvent OnClick
        {
            get
            {
                return this.GetComponent<Button>().onClick;
            }
        }

        /// <summary>
        ///     Sets the text on a button
        /// </summary>
        /// <param name="text">The text</param>
        public void SetText(string text)
        {
            this.GetComponentInChildren<Text>().text = text;
        }

        /// <summary>
        ///     Sets up (or assigns) a <seealso cref="ButtonController"/> to the button.
        /// </summary>
        /// <param name="targetTransform">The target transform to move relative to</param>
        /// <param name="height">The relative height</param>
        public void SetupButtonController(Transform targetTransform, float height)
        {
            this.reference = targetTransform;
            this.positionOffset = new Vector3(
                0.0f,
                height,
                0.0f);
        }

        /// <summary>
        ///     Called by Unity every fixed update to update the <seealso cref="ButtonController"/>
        /// </summary>
        private void FixedUpdate()
        {
            this.transform.position = this.reference.position + this.positionOffset;
            this.transform.forward = MainManager.CameraController.transform.forward;
            this.transform.position -= this.transform.forward * ButtonController.CloserBy;
        }
    }
}