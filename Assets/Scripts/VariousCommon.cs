using System;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RougePG
{
    /// <summary>
    ///     Contains various common functions.
    /// </summary>
    public static class VariousCommon
    {
        /// <summary>
        ///     Lerps the first value via an exponential operation.
        /// </summary>
        /// <param name="value">The current value to lerp</param>
        /// <param name="target">The target value</param>
        /// <param name="base">The modification base; has to be 0.0..1.0</param>
        /// <param name="exponent">The modification exponent</param>
        /// <returns>The lerped value</returns>
        public static float ExponentialLerp(float value, float target, float @base, float exponent)
        {
            if (@base < 0.0f || @base > 1.0f) throw new ArgumentOutOfRangeException("base");

            return target - (target - value) * Mathf.Pow(@base, exponent);
        }

        /// <summary>
        ///     Lerps the first value via an exponential operation.
        /// </summary>
        /// <param name="value">The current value to lerp</param>
        /// <param name="target">The target value</param>
        /// <param name="base">The modification base; has to be 0.0..1.0</param>
        /// <param name="exponent">The modification exponent</param>
        /// <returns>The lerped value</returns>
        public static Vector3 ExponentialLerp(Vector3 value, Vector3 target, float @base, float exponent)
        {
            if (@base < 0.0f || @base > 1.0f) throw new ArgumentOutOfRangeException("base");

            return target - (target - value) * Mathf.Pow(@base, exponent);
        }

        /// <summary>
        ///     Lerps the first value via an exponential operation; improved for angles... maybe.
        /// </summary>
        /// <param name="value">The current value to lerp</param>
        /// <param name="target">The target value</param>
        /// <param name="base">The modification base; has to be 0.0..1.0</param>
        /// <param name="exponent">The modification exponent</param>
        /// <returns>The lerped value</returns>
        public static Vector3 ExponentialLerpEuler(Vector3 value, Vector3 target, float @base, float exponent)
        {
            return ExponentialLerp(ClampAngle(value), ClampAngle(target), @base, exponent);
        }

        /// <summary>
        ///     Clamps a Euler Angle to -180..180
        /// </summary>
        /// <param name="value">The value to "clamp"</param>
        /// <returns>A better value</returns>
        private static float ClampAngle(float value)
        {
            return (value + 180.0f) % 360.0f - 180.0f;
        }

        /// <summary>
        ///     Clamps a Euler Angle Rotation to -180..180 on each axis
        /// </summary>
        /// <param name="value">The value to "clamp"</param>
        /// <returns>A better value</returns>
        private static Vector3 ClampAngle(Vector3 value)
        {
            return new Vector3(ClampAngle(value.x), ClampAngle(value.y), ClampAngle(value.z));
        }
    }
}
