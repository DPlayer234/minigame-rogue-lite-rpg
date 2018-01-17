using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG.Main
{
    /// <summary>
    ///     Contains information about Animations
    /// </summary>
    public class SpriteAnimationStatus3D
    {
        /// <summary>
        ///     How fast does the animation go?
        ///     A value of 1.0 means that it'll take 1 second; a value of 2.0 is twice as fast f.e.
        /// </summary>
        public readonly float speed;

        /// <summary>
        ///     Where should the body go?
        /// </summary>
        public readonly Vector3 position;

        /// <summary>
        ///     Where should anything else rotate to?
        ///     Only affects the Body Element's Children.
        ///     They are in the order the elements are displayed in the Editor, from top to bottom.
        /// </summary>
        public readonly Vector3[] rotations;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteAnimationStatus3D"/> class.
        /// </summary>
        /// <param name="speed">How fast does the animation go?</param>
        /// <param name="position">Where should the body go?</param>
        /// <param name="rotations">Where should anything else rotate to?</param>
        public SpriteAnimationStatus3D(float speed, Vector3 position, params Vector3[] rotations)
        {
            this.speed = speed;
            this.position = position;
            this.rotations = rotations;
        }
    }
}
