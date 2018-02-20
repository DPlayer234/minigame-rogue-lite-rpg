﻿namespace DPlay.RoguePG
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
        public static void PlayMusic(string name)
        {
            if (!MusicManager.SongDictionary.ContainsKey(name)) throw new RPGException(RPGException.Cause.UnknownAudioClip);

            MusicManager.Instance.StartCoroutine(MusicManager.Instance.FadeInAndOut(MusicManager.SongDictionary[name]));
        }

        /// <summary>
        ///     Plays a song.
        /// </summary>
        public static void PlayMusic(AudioClip song)
        {
            MusicManager.Instance.StartCoroutine(MusicManager.Instance.FadeInAndOut(song));
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
            float timePassed = MusicManager.SongFadeTime * (1.0f - this.audioSource.volume);

            // Fade song out
            while (timePassed < MusicManager.SongFadeTime)
            {
                this.audioSource.volume = 1.0f - timePassed / MusicManager.SongFadeTime;
                timePassed += Time.deltaTime;
                yield return null;
            }

            // Switch audio clip
            this.audioSource.Stop();
            this.audioSource.clip = song;
            this.audioSource.Play();

            // Fade song in
            while (timePassed < MusicManager.SongFadeTime * 2.0f)
            {
                this.audioSource.volume = (timePassed - MusicManager.SongFadeTime) / MusicManager.SongFadeTime;
                timePassed += Time.deltaTime;
                yield return null;
            }

            this.audioSource.volume = 1.0f;
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