namespace DPlay.RoguePG
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Defines the main audio listener in a scene
    /// </summary>
    [RequireComponent(typeof(AudioListener))]
    public class MainAudioListener : Singleton<MainAudioListener>
    {
        /// <summary> The audio listener </summary>
        public static AudioListener AudioListener { get; private set; }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="MainAudioListener"/> class.
        /// </summary>
        private void Awake()
        {
            this.NewPreferThis();

            MainAudioListener.AudioListener = this.GetComponent<AudioListener>();
        }
    }
}
