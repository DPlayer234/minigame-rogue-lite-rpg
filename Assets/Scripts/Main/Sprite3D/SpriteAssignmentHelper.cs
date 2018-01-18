using UnityEngine;

namespace SAE.RougePG.Main.Sprite3D
{
    /// <summary>
    ///     Assigns sprites.
    /// </summary>
    [ExecuteInEditMode]
    class SpriteAssignmentHelper : MonoBehaviour
    {
        /// <summary> List of sprites to assign. Sprite name and GameObject names have to match. </summary>
        public Sprite[] sprites;

        /// <summary> Tick to update the sprites </summary>
        [Tooltip("Tick to update sprites.")]
        public bool update = true;

#if UNITY_EDITOR
        /// <summary>
        ///     Called by Unity to update the <seealso cref="SpriteAssignmentHelper"/>.
        /// </summary>
        private void Update()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            else if (!Application.isPlaying && this.update)
            {
                foreach (SpriteRenderer renderer in this.GetComponentsInChildren<SpriteRenderer>())
                {
                    foreach (Sprite sprite in this.sprites)
                    {
                        if (sprite.name == renderer.name)
                        {
                            renderer.sprite = sprite;
                            break;
                        }
                    }
                }

                this.update = false;
            }
        }
#endif
    }
}
