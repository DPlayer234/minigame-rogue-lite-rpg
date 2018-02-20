//-----------------------------------------------------------------------
// <copyright file="RotationExtension.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Extension
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Contains some functions for working with angles and rotations.
    /// </summary>
    /// <remarks>
    ///     "Magic Numbers" used in functions of this class don't really have any name;
    ///     the functions don't correctly work with any other value.
    /// </remarks>
    public static class RotationExtension
    {
        /// <summary>
        ///     Lerps the first value via an exponential operation.
        ///     Adjusts values so there's no jumpiness in 3D rotations.
        /// </summary>
        /// <param name="value">The current value to lerp</param>
        /// <param name="target">The target value</param>
        /// <param name="base">The modification base; has to be 0.0..1.0</param>
        /// <param name="exponent">The modification exponent</param>
        /// <returns>The lerped rotation</returns>
        public static Vector3 ExponentialLerpRotation(Vector3 value, Vector3 target, float @base, float exponent)
        {
            if (@base < 0.0f || @base > 1.0f) throw new ArgumentOutOfRangeException("base");

            Vector3 start = new Vector3(
                GetDegreeDifference(value.x, target.x),
                GetDegreeDifference(value.y, target.y),
                GetDegreeDifference(value.z, target.z));

            return target - start * Mathf.Pow(@base, exponent);
        }

        /// <summary>
        ///     Wraps an Angle to -180..180
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <returns>A better value</returns>
        public static float WrapDegrees(float value)
        {
            return (value + 180.0f) % 360.0f - 180.0f;
        }

        /// <summary>
        ///     Wraps an Angle to -180..180
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <returns>A better value</returns>
        public static Vector3 WrapDegrees(Vector3 value)
        {
            return new Vector3(
                WrapDegrees(value.x),
                WrapDegrees(value.y),
                WrapDegrees(value.z));
        }

        /// <summary>
        ///     Returns the smallest "distance" between two angles in degrees
        /// </summary>
        /// <param name="angle1">The first angle</param>
        /// <param name="angle2">The second angle</param>
        /// <returns>The smallest "distance"</returns>
        /// I was too lazy to actually figure this out myself:
        /// https://stackoverflow.com/questions/28036652/finding-the-shortest-distance-between-two-angles
        public static float GetDegreeDifference(float angle1, float angle2)
        {
            float diff = (angle2 - angle1 + 180.0f) % 360.0f - 180.0f;
            return diff < -180.0f ? diff + 360.0f : diff;
        }

        /// <summary>
        ///     Wraps an Angle to -PI..PI
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <returns>A better value</returns>
        public static float WrapRadians(float value)
        {
            return (value + Mathf.PI) % (Mathf.PI * 2) - Mathf.PI;
        }
    }
}