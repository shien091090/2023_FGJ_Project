using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SNShien.Common.AudioTools
{
    public class FmodAudioManager : MonoBehaviour, IAudioManager
    {
        private static FmodAudioManager _instance;

        [SerializeField] private FmodAudioCollectionScriptableObject audioCollection;

        private Dictionary<int, EventInstance> eventInstanceTrackDict;

        public static FmodAudioManager Instance => _instance;

        public void PlayOneShot(string audioKey)
        {
            RuntimeManager.PlayOneShot(audioCollection.GetEventReference(audioKey));
        }

        public void PlayOneShot(EventReference eventReference)
        {
            RuntimeManager.PlayOneShot(eventReference);
        }

        public void Play(EventReference eventReference, int trackIndex = 0)
        {
            if (eventInstanceTrackDict == null)
                eventInstanceTrackDict = new Dictionary<int, EventInstance>();

            EventInstance eventInstance;
            if (eventInstanceTrackDict.ContainsKey(trackIndex))
            {
                eventInstance = eventInstanceTrackDict[trackIndex];
                eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
                if (playbackState == PLAYBACK_STATE.PLAYING)
                    eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }

            eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstanceTrackDict[trackIndex] = eventInstance;

            eventInstance.start();
        }

        public void Play(string audioKey, int trackIndex = 0)
        {
            EventReference eventReference = audioCollection.GetEventReference(audioKey);
            Play(eventReference, trackIndex);
        }

        public void SetParam(string audioParamKey, float paramValue)
        {
            RuntimeManager.StudioSystem.setParameterByName(audioParamKey, paramValue);
        }

        public void Stop(int trackIndex = 0, bool stopImmediately = false)
        {
            if (!eventInstanceTrackDict.ContainsKey(trackIndex))
                return;

            EventInstance eventInstance = eventInstanceTrackDict[trackIndex];
            eventInstance.stop(stopImmediately ?
                STOP_MODE.IMMEDIATE :
                STOP_MODE.ALLOWFADEOUT);
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }
    }
}