﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.Audio
{
    public class AudioManager : GameComponent
    {
        private static AudioManager audioManager = null;

        /// <summary>
        /// The audio engine used to play all cues.
        /// </summary>
        private AudioEngine audioEngine;

        /// <summary>
        /// The soundbank that contains all cues.
        /// </summary>
        private SoundBank soundBank;

        /// <summary>
        /// The wavebank with all wave files for this game.
        /// </summary>
        private WaveBank waveBank;

        /// <summary>
        /// Constructs the manager for audio playback of all cues.
        /// </summary>
        /// <param name="game">The game that this component will be attached to.</param>
        /// <param name="settingsFile">The filename of the XACT settings file.</param>
        /// <param name="waveBankFile">The filename of the XACT wavebank file.</param>
        /// <param name="soundBankFile">The filename of the XACT soundbank file.</param>
        private AudioManager(Game game, string settingsFile, string waveBankFile,
            string soundBankFile)
            : base(game)
        {
            try
            {
                audioEngine = new AudioEngine(settingsFile);
                waveBank = new WaveBank(audioEngine, waveBankFile);
                soundBank = new SoundBank(audioEngine, soundBankFile);
            }
            catch (NoAudioHardwareException)
            {
                // silently fall back to silence
                audioEngine = null;
                waveBank = null;
                soundBank = null;
            }
        }

        /// <summary>
        /// Initialize the static AudioManager functionality.
        /// </summary>
        /// <param name="game">The game that this component will be attached to.</param>
        /// <param name="settingsFile">The filename of the XACT settings file.</param>
        /// <param name="waveBankFile">The filename of the XACT wavebank file.</param>
        /// <param name="soundBankFile">The filename of the XACT soundbank file.</param>
        public static void Initialize(Game game, string settingsFile,
            string waveBankFile, string soundBankFile)
        {
            audioManager = new AudioManager(game, settingsFile, waveBankFile,
                soundBankFile);
            if (game != null)
            {
                game.Components.Add(audioManager);
            }
        }

        /// <summary>
        /// Retrieve a cue by name.
        /// </summary>
        /// <param name="cueName">The name of the cue requested.</param>
        /// <returns>The cue corresponding to the name provided.</returns>
        public static Cue GetCue(string cueName)
        {
            if (String.IsNullOrEmpty(cueName) ||
                (audioManager == null) || (audioManager.audioEngine == null) ||
                (audioManager.soundBank == null) || (audioManager.waveBank == null))
            {
                return null;
            }
            return audioManager.soundBank.GetCue(cueName);
        }


        /// <summary>
        /// Plays a cue by name.
        /// </summary>
        /// <param name="cueName">The name of the cue to play.</param>
        public static void PlayCue(string cueName)
        {
            if ((audioManager != null) && (audioManager.audioEngine != null) &&
                (audioManager.soundBank != null) && (audioManager.waveBank != null))
            {
                audioManager.soundBank.PlayCue(cueName);
            }
        }

        /// <summary>
        /// The cue for the music currently playing, if any.
        /// </summary>
        private Cue musicCue;


        /// <summary>
        /// Stack of music cue names, for layered music playback.
        /// </summary>
        private Stack<string> musicCueNameStack = new Stack<string>();


        /// <summary>
        /// Plays the desired music, clearing the stack of music cues.
        /// </summary>
        /// <param name="cueName">The name of the music cue to play.</param>
        public static void PlayMusic(string cueName)
        {
            // start the new music cue
            if (audioManager != null)
            {
                audioManager.musicCueNameStack.Clear();
                PushMusic(cueName);
            }
        }


        /// <summary>
        /// Plays the music for this game, adding it to the music stack.
        /// </summary>
        /// <param name="cueName">The name of the music cue to play.</param>
        public static void PushMusic(string cueName)
        {
            // start the new music cue
            if ((audioManager != null) && (audioManager.audioEngine != null) &&
                (audioManager.soundBank != null) && (audioManager.waveBank != null))
            {
                audioManager.musicCueNameStack.Push(cueName);
                if ((audioManager.musicCue == null) ||
                    (audioManager.musicCue.Name != cueName))
                {
                    if (audioManager.musicCue != null)
                    {
                        audioManager.musicCue.Stop(AudioStopOptions.AsAuthored);
                        audioManager.musicCue.Dispose();
                        audioManager.musicCue = null;
                    }
                    audioManager.musicCue = GetCue(cueName);
                    if (audioManager.musicCue != null)
                    {
                        audioManager.musicCue.Play();
                    }
                }
            }
        }


        /// <summary>
        /// Stops the current music and plays the previous music on the stack.
        /// </summary>
        public static void PopMusic()
        {
            // start the new music cue
            if ((audioManager != null) && (audioManager.audioEngine != null) &&
                (audioManager.soundBank != null) && (audioManager.waveBank != null))
            {
                string cueName = null;
                if (audioManager.musicCueNameStack.Count > 0)
                {
                    audioManager.musicCueNameStack.Pop();
                    if (audioManager.musicCueNameStack.Count > 0)
                    {
                        cueName = audioManager.musicCueNameStack.Peek();
                    }
                }
                if ((audioManager.musicCue == null) ||
                    (audioManager.musicCue.Name != cueName))
                {
                    if (audioManager.musicCue != null)
                    {
                        audioManager.musicCue.Stop(AudioStopOptions.AsAuthored);
                        audioManager.musicCue.Dispose();
                        audioManager.musicCue = null;
                    }
                    if (!String.IsNullOrEmpty(cueName))
                    {
                        audioManager.musicCue = GetCue(cueName);
                        if (audioManager.musicCue != null)
                        {
                            audioManager.musicCue.Play();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Stop music playback, clearing the cue.
        /// </summary>
        public static void StopMusic()
        {
            if (audioManager != null)
            {
                audioManager.musicCueNameStack.Clear();
                if (audioManager.musicCue != null)
                {
                    audioManager.musicCue.Stop(AudioStopOptions.AsAuthored);
                    audioManager.musicCue.Dispose();
                    audioManager.musicCue = null;
                }
            }
        }

        /// <summary>
        /// Update the audio manager, particularly the engine.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // update the audio engine
            if (audioEngine != null)
            {
                audioEngine.Update();
            }

            //if ((musicCue != null) && musicCue.IsStopped)
            //{
            //    AudioManager.PopMusic();
            //}

            base.Update(gameTime);
        }

        /// <summary>
        /// Clean up the component when it is disposing.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    StopMusic();
                    if (soundBank != null)
                    {
                        soundBank.Dispose();
                        soundBank = null;
                    }
                    if (waveBank != null)
                    {
                        waveBank.Dispose();
                        waveBank = null;
                    }
                    if (audioEngine != null)
                    {
                        audioEngine.Dispose();
                        audioEngine = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
