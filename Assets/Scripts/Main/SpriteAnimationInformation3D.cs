using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG.Main
{
    /// <summary>
    ///     Contains information about Animations
    /// </summary>
    public class SpriteAnimationInformation3D
    {
        /// <summary>
        ///     How important is this current state?
        ///     Higher values interpolate to the desired result faster.
        ///     Range: 0.0..1.0
        /// </summary>
        public float importance;

        /// <summary>
        ///     Where should the body go?
        /// </summary>
        public Vector3 position;

        /// <summary>
        ///     Where should anything else rotate to?
        ///     Only affects the Body Element's Children.
        ///     They are in the order the elements are displayed in the Editor, from top to bottom.
        /// </summary>
        public Vector3[] rotations;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteAnimationInformation3D"/> class.
        /// </summary>
        /// <param name="importance">How important is it?</param>
        /// <param name="position">Where should the body go?</param>
        /// <param name="rotations">Where should anything else rotate to?</param>
        public SpriteAnimationInformation3D(float importance, Vector3 position, params Vector3[] rotations)
        {
            this.importance = importance;
            this.position = position;
            this.rotations = rotations;
        }
    }
}
