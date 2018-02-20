namespace DPlay.RoguePG.Main.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using DPlay.RoguePG.Main.BattleDriver;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Controls the rotation and movement of a 3D text
    /// </summary>
    public class Text3DController : MonoBehaviour
    {
        /// <summary>
        ///     Time (in seconds) it takes for the text to fully scale.
        /// </summary>
        private const float ScaleTime = 0.2f;

        /// <summary>
        ///     How long (in seconds) the text stays at full size.
        /// </summary>
        private const float FullSizeTime = 2.6f;

        /// <summary>
        ///     The <seealso cref="TextMesh"/> also attached to this GameObject
        /// </summary>
        private TextMesh textMesh;

        /// <summary>
        ///     The initial text size when <seealso cref="Awake"/> is called
        /// </summary>
        private float initialTextSize;

        /// <summary>
        ///     The age of this in seconds
        /// </summary>
        private float age;

        /// <summary>
        ///     The text, if set before the <seealso cref="textMesh"/> is assigned
        /// </summary>
        private string temporaryText;

        /// <summary>
        ///     The text.
        /// </summary>
        public string Text
        {
            get
            {
                if (this.textMesh != null)
                {
                    return this.textMesh.text;
                }

                return null;
            }

            set
            {
                if (this.textMesh != null)
                {
                    this.textMesh.text = value;
                }

                this.temporaryText = value;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="Text3DController"/> whether it is active or not
        /// </summary>
        private void Awake()
        {
            this.textMesh = this.GetComponent<TextMesh>();

            this.initialTextSize = this.textMesh.characterSize;

            this.age = 0.0f;

            if (this.temporaryText != null)
            {
                this.textMesh.text = this.temporaryText;
            }

            this.FixedUpdate();

            this.age = 0.0f;
        }

        /// <summary>
        ///     Called by Unity every fixed update to update the <seealso cref="Text3DController"/>
        /// </summary>
        private void FixedUpdate()
        {
            this.transform.forward = MainManager.CameraController.transform.forward;

            this.textMesh.characterSize =
                initialTextSize * (
                    // Scaling up; new object
                    this.age < Text3DController.ScaleTime ?
                    this.age / Text3DController.ScaleTime :

                    // Full size
                    this.age < Text3DController.ScaleTime + Text3DController.FullSizeTime ?
                    1.0f :

                    // Scaling down
                    1.0f - (this.age - Text3DController.ScaleTime - Text3DController.FullSizeTime) / Text3DController.ScaleTime);

            this.age += Time.fixedDeltaTime;

            if (this.age > Text3DController.FullSizeTime + Text3DController.ScaleTime * 2)
            {
                MonoBehaviour.Destroy(this.gameObject);
                return;
            }
        }
    }
}