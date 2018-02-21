//-----------------------------------------------------------------------
// <copyright file="SpriteAnimationStatus.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.Sprite3D
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Contains information about Animations
    /// </summary>
    public class SpriteAnimationStatus
    {
        /// <summary>
        ///     How fast does the animation go?
        ///     A value of 1.0 means that it'll take 1 second; a value of 2.0 is twice as fast f.e.
        ///     Cannot be less than 0.0.
        /// </summary>
        public readonly float Speed;

        /// <summary>
        ///     Where should the body go?
        /// </summary>
        public readonly Vector3 Position;

        /// <summary>
        ///     Where should everything rotate to? (Z-Axis)
        ///     They are in the order the elements are displayed in the Editor, from top to bottom, starting at the SpriteBody.
        /// </summary>
        public readonly float[] Rotations;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteAnimationStatus"/> class.
        /// </summary>
        /// <param name="speed">How fast does the animation go? (>= 0.0)</param>
        /// <param name="position">Where should the body go?</param>
        /// <param name="rotations">Where should everything rotate to?</param>
        public SpriteAnimationStatus(float speed, Vector3 position, params float[] rotations)
        {
            if (speed < 0.0f) throw new ArgumentOutOfRangeException("speed");

            this.Speed = speed;
            this.Position = position;
            this.Rotations = rotations;
        }
    }
}
