//-----------------------------------------------------------------------
// <copyright file="Singleton.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG
{
    using UnityEngine;

    /// <summary>
    ///     Defines a new MonoBehaviour singleton
    /// </summary>
    /// <typeparam name="T">The type of the class</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        /// <summary> The current instance of the singleton </summary>
        public static T Instance { get; protected set; }

        /// <summary>
        ///     Returns the instance as T
        /// </summary>
        /// <param name="instance">The instance to convert</param>
        public static implicit operator T(Singleton<T> instance)
        {
            return instance as T;
        }

        /// <summary>
        ///     Call this when a new instance is added.
        ///     This is instance is going to override the old one.
        ///     To keep the old instance instead, call <seealso cref="NewPreferOld(bool)"/>.
        /// </summary>
        /// <param name="destroyGameObject">Whether to destroy the GameObject if this instance is destroyed</param>
        protected void NewPreferThis(bool destroyGameObject = false)
        {
            if (Singleton<T>.Instance == this) return;

            if (Singleton<T>.Instance != null)
            {
                this.DestroyOld(destroyGameObject);
            }

            Singleton<T>.Instance = this;
        }

        /// <summary>
        ///     Call this when a new instance is added.
        ///     If there is an old instance, this one is going to be destroyed.
        ///     To keep the new instance instead, call <seealso cref="NewPreferThis(bool)"/>.
        /// </summary>
        /// <param name="destroyGameObject">Whether to destroy the GameObject if this instance is destroyed</param>
        protected void NewPreferOld(bool destroyGameObject = false)
        {
            if (Singleton<T>.Instance == this) return;

            if (Singleton<T>.Instance != null)
            {
                this.DestroySelf(destroyGameObject);
                return;
            }

            Singleton<T>.Instance = this;
        }

        /// <summary>
        ///     Destroys the new instance.
        /// </summary>
        /// <param name="destroyGameObject">Whether to destroy the GameObject</param>
        private void DestroySelf(bool destroyGameObject = false)
        {
            Debug.LogWarning("There was an additional active " + this.GetType() + ". The new instance was destroyed.");
            this.DestroyInstance(destroyGameObject);
        }

        /// <summary>
        ///     Destroys the old instance.
        /// </summary>
        /// <param name="destroyGameObject">Whether to destroy the GameObject</param>
        private void DestroyOld(bool destroyGameObject = false)
        {
            Debug.LogWarning("There was an additional active " + this.GetType() + ". The old instance was destroyed.");
            Singleton<T>.Instance.DestroyInstance(destroyGameObject);
        }

        /// <summary>
        ///     Destroy this instance;
        /// </summary>
        /// <param name="destroyGameObject">Whether to destroy the GameObject</param>
        private void DestroyInstance(bool destroyGameObject = false)
        {
            if (destroyGameObject)
            {
                MonoBehaviour.Destroy(this.gameObject);
            }
            else
            {
                MonoBehaviour.Destroy(Singleton<T>.Instance);
            }
        }
    }
}
