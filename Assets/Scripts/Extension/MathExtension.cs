//-----------------------------------------------------------------------
// <copyright file="MathExtension.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Extension
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Contains various additional math related methods.
    /// </summary>
    /// <remarks>
    ///     "Magic Numbers" used in functions of this class don't really have any name;
    ///     the functions don't correctly work with any other value.
    /// </remarks>
    public static class MathExtension
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
        ///     Sums all integers in the range from <paramref name="start"/> to <paramref name="end"/>
        /// </summary>
        /// <param name="start">The inclusive starting point</param>
        /// <param name="end">The inclusive ending point</param>
        /// <returns>The sum</returns>
        public static int SumRange(int start, int end)
        {
            int sum = 0;
            for (int i = start; i <= end; i++)
            {
                sum += i;
            }

            return sum;
        }

        /// <summary>
        ///     Sums all results of <paramref name="func"/>() in the range from <paramref name="start"/> to <paramref name="end"/>
        /// </summary>
        /// <param name="func">The function to call</param>
        /// <param name="start">The inclusive starting point</param>
        /// <param name="end">The inclusive ending point</param>
        /// <returns>The sum</returns>
        public static float SumFuncRange(Func<int, float> func, int start, int end)
        {
            float sum = 0;
            for (int i = start; i <= end; i++)
            {
                sum += func(i);
            }

            return sum;
        }

        /// <summary>
        ///     Sums all results of <paramref name="func"/>() in the range from <paramref name="start"/> to <paramref name="end"/>
        /// </summary>
        /// <param name="func">The function to call</param>
        /// <param name="start">The inclusive starting point</param>
        /// <param name="end">The inclusive ending point</param>
        /// <returns>The sum</returns>
        public static int SumFuncRange(Func<int, int> func, int start, int end)
        {
            int sum = 0;
            for (int i = start; i <= end; i++)
            {
                sum += func(i);
            }

            return sum;
        }
    }
}
