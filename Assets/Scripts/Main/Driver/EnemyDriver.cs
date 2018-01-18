using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RougePG.Main.Sprite3D;

namespace SAE.RougePG.Main.Driver
{
    /// <summary>
    ///     Makes Entities work.
    /// </summary>
    [RequireComponent(typeof(EntityDriver))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SpriteAnimator))]
    public class EnemyDriver : MonoBehaviour
    {
        /// <summary> The <seealso cref="SpriteAnimator"/> also attached to this <seealso cref="GameObject"/> </summary>
        private SpriteAnimator spriteAnimator;

        /// <summary> The <seealso cref="Rigidbody"/> also attached to this <seealso cref="GameObject"/> </summary>
        new private Rigidbody rigidbody;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyDriver"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.tag = "EnemyEntity";

            this.spriteAnimator = this.GetComponent<SpriteAnimator>();
            if (this.spriteAnimator == null) throw new Exceptions.EntityDriverException("There is no SpriteAnimator3D attached to this GameObject.");
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="EnemyDriver"/>
        /// </summary>
        private void FixedUpdate()
        {
            
        }
    }
}
