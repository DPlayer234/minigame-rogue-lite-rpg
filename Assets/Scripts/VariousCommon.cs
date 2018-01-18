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
        ///     Allows smoothly transitioning between two points
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">End point</param>
        /// <param name="time">Time progress 0.0..1.0</param>
        /// <returns>A value smoothly interpolated based on time in the range 0.0..1.0</returns>
        public static float SmoothStep(float start, float end, float time)
        {
            return time < 0.0f ? start
                : time < 1.0f ? start + (time * time * (3.0f - 2.0f * time)) * (end - start)
                : end;
        }

        /// <summary>
        ///     Allows smoothly transitioning between two points
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">End point</param>
        /// <param name="time">Time progress 0.0..1.0</param>
        /// <returns>A value smoothly interpolated based on time in the range 0.0..1.0</returns>
        public static Vector3 SmoothStep(Vector3 start, Vector3 end, float time)
        {
            return new Vector3(
                SmoothStep(start.x, end.x, time),
                SmoothStep(start.y, end.y, time),
                SmoothStep(start.z, end.z, time));
        }

        /// <summary>
        ///     Allows smoothly transitioning between two points
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">End point</param>
        /// <param name="time">Time progress 0.0..1.0</param>
        /// <returns>A value smoothly interpolated based on time in the range 0.0..1.0</returns>
        public static float SmootherStep(float start, float end, float time)
        {
            return time < 0.0f ? start
                : time < 1.0f ? start + (time * time * time * (time * (6.0f * time - 15.0f) + 10.0f)) * (end - start)
                : end;
        }

        /// <summary>
        ///     Allows smoothly transitioning between two points
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">End point</param>
        /// <param name="time">Time progress 0.0..1.0</param>
        /// <returns>A value smoothly interpolated based on time in the range 0.0..1.0</returns>
        public static Vector3 SmootherStep(Vector3 start, Vector3 end, float time)
        {
            return new Vector3(
                SmootherStep(start.x, end.x, time),
                SmootherStep(start.y, end.y, time),
                SmootherStep(start.z, end.z, time));
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
