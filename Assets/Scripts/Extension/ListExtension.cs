//-----------------------------------------------------------------------
// <copyright file="ListExtension.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Extension
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Adds extension methods for Lists and Collections
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        ///     Returns an array of all components in a given collection of <seealso cref="GameObject"/>s
        /// </summary>
        /// <typeparam name="T">The type of component to get</typeparam>
        /// <param name="gameObjects">The collection of <seealso cref="GameObject"/>s</param>
        /// <returns>An array of all components</returns>
        public static T[] GetComponentsInCollection<T>(this ICollection<GameObject> gameObjects)
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
        /// <param name="originalArray">The original array (or IList)</param>
        /// <returns>A new array</returns>
        public static T[][] SplitIntoArrayOfLenghtOneArrays<T>(this IList<T> originalArray)
        {
            T[][] newArray = new T[originalArray.Count][];

            for (int i = 0; i < originalArray.Count; i++)
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

        /// <summary>
        ///     Returns the element at <paramref name="index"/> from <paramref name="list"/>
        ///     or the default value if there is none.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="list"/></typeparam>
        /// <param name="list">The list</param>
        /// <param name="index">The index</param>
        /// <returns>The element at <paramref name="index"/> or the default</returns>
        public static T GetValueSafe<T>(this IList<T> list, int index) where T : struct
        {
            if (index >= 0 && index < list.Count)
            {
                return list[index];
            }

            return new T();
        }

        /// <summary>
        ///     Returns the element at <paramref name="index"/> from <paramref name="list"/>
        ///     or null if there is none.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="list"/></typeparam>
        /// <param name="list">The list</param>
        /// <param name="index">The index</param>
        /// <returns>The element at <paramref name="index"/> or null</returns>
        public static T GetReferenceSafe<T>(this IList<T> list, int index) where T : class
        {
            if (index >= 0 && index < list.Count)
            {
                return list[index];
            }

            return null;
        }

        /// <summary>
        ///     Returns the item at <paramref name="index"/> % <paramref name="list"/>.Count
        /// </summary>
        /// <typeparam name="T">The type of the list content</typeparam>
        /// <param name="list">The list</param>
        /// <param name="index">The wrapping index</param>
        /// <returns>The item at the given position</returns>
        public static T GetItemWrap<T>(this IList<T> list, int index)
        {
            return list[index % list.Count];
        }
    }
}
