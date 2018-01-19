using System.Collections;
using UnityEngine;
using SAE.RoguePG.Main;
using SAE.RoguePG.Main.Driver;
using SAE.RoguePG.Main.Sprite3D;

namespace SAE.RoguePG.Dev
{
    /// <summary>
    ///     Used to help assigning data to GameObjects.
    /// </summary>
    [RequireComponent(typeof(EntityDriver))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    class EntityCreationHelper : MonoBehaviour
    {
        /// <summary> List of sprites to assign. Sprite name and GameObject names have to match. </summary>
        public Sprite[] sprites = null;

        /// <summary> Material to assign to all <seealso cref="SpriteRenderer"/>s </summary>
        public Material material = null;

        /// <summary> Tick to update the sprites </summary>
        [Tooltip("Tick to update sprites.")]
        public bool update = true;

        /// <summary> Information to be set with a <seealso cref="PlayerDriver"/> </summary>
        private static readonly EntityInformation playerInformation = new EntityInformation(
            tag: StateManager.PlayerEntityTag,
            lightColor: new Color(0.0f, 1.0f, 0.6f));

        /// <summary> Information to be set with a <seealso cref="EnemyDriver"/> </summary>
        private static readonly EntityInformation enemyInformation = new EntityInformation(
            tag: StateManager.EnemyEntityTag,
            lightColor: new Color(1.0f, 0.3f, 0.3f));

        /// <summary>
        ///     Called by Unity to update the <seealso cref="EntityCreationHelper"/>.
        /// </summary>
        private void Update()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
#if UNITY_EDITOR
            else if (!Application.isPlaying && this.update)
            {
                EntityInformation entityInformation =
                    this.GetComponent<PlayerDriver>() != null ? playerInformation :
                    this.GetComponent<EnemyDriver>() != null ? enemyInformation :
                    null;

                if (entityInformation != null)
                {
                    entityInformation.AssignValuesTo(this.gameObject);
                }

                // Assign Sprites
                foreach (SpriteRenderer renderer in this.GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.material = this.material;

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
#endif
        }
    }

    /// <summary>
    ///     Stores information to be set to an Entity
    /// </summary>
    internal class EntityInformation
    {
        /// <summary> What to tag the Entity as </summary>
        public readonly string tag;

        /// <summary> Color of an attached light source </summary>
        public readonly Color lightColor;

        /// <summary>
        ///     Initializes an instance of the <seealso cref="EntityInformation"/> class
        /// </summary>
        /// <param name="lightColor">Color of an attached light source</param>
        public EntityInformation(string tag, Color lightColor)
        {
            this.tag = tag;
            this.lightColor = lightColor;
        }

        /// <summary>
        ///     Assigns its values to a <seealso cref="GameObject"/>
        /// </summary>
        /// <param name="gameObject">The <seealso cref="GameObject"/> to modify</param>
        public void AssignValuesTo(GameObject gameObject)
        {
            gameObject.tag = this.tag;

            // Assign Light Color
            Light light = gameObject.GetComponentInChildren<Light>();

            if (light != null)
            {
                light.color = this.lightColor;
            }
            else
            {
                Debug.LogWarningFormat("GameObject {0} does not have any Light attached.", gameObject);
            }
        }
    }
}
