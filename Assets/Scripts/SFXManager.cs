namespace DPlay.RoguePG
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Manages SFX clips, sources and such.
    /// </summary>
    public class SFXManager : Singleton<SFXManager>
    {
        /// <summary> The available audio clips </summary>
        [SerializeField]
        private AudioClip[] audioClips;

        /// <summary>
        ///     The available audio clips.
        ///     The keys are the clip names.
        /// </summary>
        public static IDictionary<string, AudioClip> AudioClipDictionary { get; private set; }

        /// <summary>
        ///     Plays a clip by name.
        /// </summary>
        public static void PlayClip(string name, Vector3 position)
        {
            if (!SFXManager.AudioClipDictionary.ContainsKey(name)) throw new RPGException(RPGException.Cause.UnknownAudioClip);
            AudioSource.PlayClipAtPoint(SFXManager.AudioClipDictionary[name], position);
        }

        /// <summary>
        ///     Plays a clip by name with a set volume.
        /// </summary>
        public static void PlayClip(string name, Vector3 position, float volume)
        {
            if (!SFXManager.AudioClipDictionary.ContainsKey(name)) throw new RPGException(RPGException.Cause.UnknownAudioClip);
            AudioSource.PlayClipAtPoint(SFXManager.AudioClipDictionary[name], position, volume);
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="SFXManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.NewPreferOld(true);

            MonoBehaviour.DontDestroyOnLoad(this);
            this.GenerateAudioClipDictionary();
        }

        /// <summary>
        ///     Generates a dictionary from <seealso cref="audioClips"/> and
        ///     assigns it to <seealso cref="AudioClipDictionary"/>.
        /// </summary>
        private void GenerateAudioClipDictionary()
        {
            SFXManager.AudioClipDictionary = new Dictionary<string, AudioClip>(this.audioClips.Length);

            foreach (AudioClip audioClip in this.audioClips)
            {
                SFXManager.AudioClipDictionary.Add(audioClip.name, audioClip);
            }
        }
    }
}
