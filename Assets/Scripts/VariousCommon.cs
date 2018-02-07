namespace SAE.RoguePG
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Contains various common functions.
    /// </summary>
    /// "Magic Numbers" used in functions of this class don't really have any name;
    /// the functions don't correctly work with any other value.
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

        /// <summary>
        ///     Returns an array of all components in a given collection of <seealso cref="GameObject"/>s
        /// </summary>
        /// <typeparam name="T">The type of component to get</typeparam>
        /// <param name="gameObjects">The collection of <seealso cref="GameObject"/>s</param>
        /// <returns>An array of all components</returns>
        public static T[] GetComponentsInCollection<T>(ICollection<GameObject> gameObjects)
        {
            T[] components = new T[gameObjects.Count];

            int i = 0;
            foreach (GameObject gameObject in gameObjects)
            {
                components[i++] = gameObject.GetComponent<T>();
            }

            return components;
        }

        /// <summary>
        ///     Splits the original array into an array of equal lenght with each element being another array,
        ///     containing solely one item of the original array.
        /// </summary>
        /// <typeparam name="T">The type of items of the original array</typeparam>
        /// <param name="originalArray">The original array</param>
        /// <returns>A new array</returns>
        public static T[][] SplitIntoArrayOfLenghtOneArrays<T>(T[] originalArray)
        {
            T[][] newArray = new T[originalArray.Length][];

            for (int i = 0; i < originalArray.Length; i++)
            {
                newArray[i] = new T[]
                {
                    originalArray[i]
                };
            }

            return newArray;
        }

        /// <summary>
        ///     Returns a random item from a list
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="list"/></typeparam>
        /// <param name="list">The list to pick an element from</param>
        /// <returns>A random element</returns>
        public static T GetRandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new InvalidOperationException("Cannot get random item from an empty IList.");

            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}
