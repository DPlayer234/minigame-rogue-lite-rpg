namespace DPlay.RoguePG.Main.Camera
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Manages the activity of game objects and behaviours.
    ///     Behaves like a singleton and will delete any new instances.
    /// </summary>
    public class ActivityHandler : Singleton<ActivityHandler>
    {
        /// <summary> Distance from the camera in which <seealso cref="LimitedRangeObjects"/> are active </summary>
        public const float ActiveRange = 40.0f;

        /// <summary> The delay in seconds between updates in <seealso cref="UpdateRangeActivity"/> </summary>
        private const float ActivityUpdateRate = 0.5f;

        /// <summary> GameObjects which are only enabled in a limited range </summary>
        public static List<GameObject> LimitedRangeObjects { get; private set; }

        /// <summary> Behaviours which are only enabled in a limited range </summary>
        public static List<Behaviour> LimitedRangeBehaviours { get; private set; }

        /// <summary>
        ///     Whether this is enabled and updated.
        /// </summary>
        public static bool Enabled
        {
            get
            {
                ActivityHandler.CheckInstance();
                return ActivityHandler.Instance.enabled;
            }

            set
            {
                ActivityHandler.CheckInstance();
                ActivityHandler.Instance.enabled = value;
            }
        }

        /// <summary>
        ///     Restarts the range activity check and immediately performs one
        /// </summary>
        public static void UpdateAndRestart()
        {
            ActivityHandler.CheckInstance();
            ActivityHandler.Instance.UpdateAndRestartRangeActivityCheck();
        }

        /// <summary>
        ///     Adds a behaviour to be only active in a limited range
        /// </summary>
        /// <param name="behaviour">The behaviour</param>
        public static void Add(Behaviour behaviour)
        {
            ActivityHandler.CheckInstance();

            behaviour.enabled = false;
            ActivityHandler.LimitedRangeBehaviours.Add(behaviour);
        }

        /// <summary>
        ///     Adds a GameObject to be only active in a limited range
        /// </summary>
        /// <param name="gameObject">The GameObject</param>
        public static void Add(GameObject gameObject)
        {
            ActivityHandler.CheckInstance();

            gameObject.SetActive(false);
            ActivityHandler.LimitedRangeObjects.Add(gameObject);
        }

        /// <summary>
        ///     Adds behaviours to be only active in a limited range
        /// </summary>
        /// <param name="behaviours">The behaviours</param>
        public static void Add(Behaviour[] behaviours)
        {
            foreach (Behaviour behaviour in behaviours)
            {
                ActivityHandler.Add(behaviour);
            }
        }

        /// <summary>
        ///     Adds GameObjects to be only active in a limited range
        /// </summary>
        /// <param name="gameObjects">The GameObjects</param>
        public static void Add(GameObject[] gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                ActivityHandler.Add(gameObject);
            }
        }

        /// <summary>
        ///     Returns whether the given transform is in the active range
        /// </summary>
        /// <param name="transform">The transform to check</param>
        /// <returns>Whether it is in the active range</returns>
        public bool IsInActiveRange(Transform transform)
        {
            return (this.transform.position - transform.position).sqrMagnitude < ActivityHandler.ActiveRange * ActivityHandler.ActiveRange;
        }

        /// <summary>
        ///     Throws an exception if there is no instance.
        /// </summary>
        private static void CheckInstance()
        {
            if (ActivityHandler.Instance == null) throw new RPGException(RPGException.Cause.ActivityHandlerNoInstance);
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="ActivityHandler"/> whether it is enabled or not.
        /// </summary>
        private void Awake()
        {
            this.NewPreferThis();

            ActivityHandler.LimitedRangeObjects = new List<GameObject>();
            ActivityHandler.LimitedRangeBehaviours = new List<Behaviour>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="ActivityHandler"/> when it is first enabled.
        /// </summary>
        private void Start()
        {
            this.UpdateAndRestartRangeActivityCheck();
        }

        /// <summary>
        ///     Restarts the range activity check and immediately performs one.
        /// </summary>
        private void UpdateAndRestartRangeActivityCheck()
        {
            this.StopAllCoroutines();
            this.StartCoroutine(this.UpdateRangeActivity());
        }

        /// <summary>
        ///     Updates which objects and behaviour in <seealso cref="LimitedRangeObjects"/> and
        ///     <seealso cref="LimitedRangeBehaviours"/> are active.
        /// </summary>
        /// <returns>An iterator</returns>
        private IEnumerator UpdateRangeActivity()
        {
            Func<bool> waitWhileInactive = delegate
            {
                return !this.enabled;
            };

            while (true)
            {
                foreach (GameObject gameObject in ActivityHandler.LimitedRangeObjects)
                {
                    if (gameObject == null)
                    {
                        continue;
                    }

                    bool shouldBeActive = this.IsInActiveRange(gameObject.transform);

                    if (gameObject.activeSelf != shouldBeActive)
                    {
                        gameObject.SetActive(shouldBeActive);
                    }
                }

                foreach (Behaviour behaviour in ActivityHandler.LimitedRangeBehaviours)
                {
                    if (behaviour == null)
                    {
                        continue;
                    }

                    bool shouldBeEnabled = this.IsInActiveRange(behaviour.transform);

                    if (behaviour.enabled != shouldBeEnabled)
                    {
                        behaviour.enabled = shouldBeEnabled;
                    }
                }

                yield return new WaitForSecondsRealtime(ActivityHandler.ActivityUpdateRate);

                if (waitWhileInactive())
                {
                    yield return new WaitWhile(waitWhileInactive);
                }
            }
        }
    }
}
