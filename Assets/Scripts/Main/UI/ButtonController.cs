namespace SAE.RoguePG.Main.UI
{
    using SAE.RoguePG.Main.BattleDriver;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Controls the position of a button
    /// </summary>
    public class ButtonController : MonoBehaviour
    { 
        /// <summary>
        ///     How many units closer should the button be
        /// </summary>
        private const float CloserBy = 0.25f;

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
        ///     Called by Unity every fixed update to update the <seealso cref="ButtonController"/>
        /// </summary>
        private void FixedUpdate()
        {
            this.transform.position = this.reference.position + positionOffset;
            this.transform.forward = MainManager.CameraController.transform.forward;
            this.transform.position -= this.transform.forward * ButtonController.CloserBy;
        }
    }
}