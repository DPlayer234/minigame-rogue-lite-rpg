namespace SAE.RoguePG
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
        ///     Call this when a new instance is added.
        /// </summary>
        protected void NewInstance()
        {
            if (Singleton<T>.Instance == this) return;

            if (Singleton<T>.Instance != null)
            {
                Debug.LogWarning("There was an additional active " + this.GetType() + ". The old instance was destroyed.");
                MonoBehaviour.Destroy(Singleton<T>.Instance);
            }

            Singleton<T>.Instance = this;
        }

        /// <summary>
        ///     Returns the instance as T
        /// </summary>
        /// <param name="instance">The instance to convert</param>
        public static implicit operator T(Singleton<T> instance)
        {
            return instance as T;
        }
    }
}
