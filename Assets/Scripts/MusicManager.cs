//-----------------------------------------------------------------------
// <copyright file="MusicManager.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Manages music.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : Singleton<MusicManager>
    {
        /// <summary>
        ///     The time it takes for a song to fade in seconds.
        /// </summary>
        private const float SongFadeTime = 0.5f;

        /// <summary> The available songs </summary>
        [SerializeField]
        private AudioClip[] songs;

        /// <summary> The audio source also attached to this GameObject </summary>
        private AudioSource audioSource;

        /// <summary> The original volume on the audio source </summary>
        private float volume;

        /// <summary>
        ///     The available audio clips.
        ///     The keys are the clip names.
        /// </summary>
        public static IDictionary<string, AudioClip> SongDictionary { get; private set; }

        /// <summary>
        ///     The audio listener to snap to
        /// </summary>
        private static AudioListener AudioListener
        {
            get
            {
                return MainAudioListener.AudioListener;
            }
        }

        /// <summary>
        ///     Plays a song.
        /// </summary>
        /// <param name="song">A clip to use as a song</param>
        public static void PlayMusic(AudioClip song)
        {
            MusicManager.Instance.StartCoroutine(MusicManager.Instance.FadeInAndOut(song));
        }

        /// <summary>
        ///     Plays a song.
        /// </summary>
        /// <param name="name">The name of the song</param>
        public static void PlayMusic(string name)
        {
            if (!MusicManager.SongDictionary.ContainsKey(name)) throw new RPGException(RPGException.Cause.UnknownAudioClip);

            MusicManager.Instance.StartCoroutine(MusicManager.Instance.FadeInAndOut(MusicManager.SongDictionary[name]));
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="MusicManager"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.NewPreferOld(true);

            MonoBehaviour.DontDestroyOnLoad(this);
            this.GenerateAudioClipDictionary();

            this.audioSource = this.GetComponent<AudioSource>();
            this.volume = this.audioSource.volume;
        }

        /// <summary>
        ///     Called by Unity every frame to update the <seealso cref="MusicManager"/>
        /// </summary>
        private void Update()
        {
            if (MusicManager.AudioListener != null)
            {
                this.transform.position = MusicManager.AudioListener.transform.position;
            }
        }

        /// <summary>
        ///     Fades the current song out and a new one in.
        /// </summary>
        /// <param name="song">The new song to fade in</param>
        /// <returns>An enumerator</returns>
        private IEnumerator FadeInAndOut(AudioClip song)
        {
            float timePassed = MusicManager.SongFadeTime * (1.0f - this.audioSource.volume / this.volume);

            // Fade song out
            while (timePassed < MusicManager.SongFadeTime)
            {
                this.audioSource.volume = (1.0f - timePassed / MusicManager.SongFadeTime) * this.volume;
                timePassed += Time.deltaTime;
                yield return null;
            }

            this.audioSource.volume = 0.0f;

            // Switch audio clip
            this.audioSource.Stop();
            if (song == null) yield break;
            yield return null;

            this.audioSource.clip = song;
            this.audioSource.Play();

            // Fade song in
            while (timePassed < MusicManager.SongFadeTime * 2.0f)
            {
                this.audioSource.volume = ((timePassed - MusicManager.SongFadeTime) / MusicManager.SongFadeTime) * this.volume;
                timePassed += Time.deltaTime;
                yield return null;
            }

            this.audioSource.volume = this.volume;
        }

        /// <summary>
        ///     Generates a dictionary from <seealso cref="songs"/> and
        ///     assigns it to <seealso cref="SongDictionary"/>.
        /// </summary>
        private void GenerateAudioClipDictionary()
        {
            MusicManager.SongDictionary = new Dictionary<string, AudioClip>(this.songs.Length);

            foreach (AudioClip audioClip in this.songs)
            {
                MusicManager.SongDictionary.Add(audioClip.name, audioClip);
            }
        }
    }
}
