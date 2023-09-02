using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace SNShien.Common.AudioTools
{
    [System.Serializable]
    public class FmodAudioCollection
    {
        [SerializeField] private string audioKey;
        [SerializeField] private List<EventReference> audioEventReferences;

        public string GetAudioKey => audioKey;

        public EventReference GetEventRef => audioEventReferences.Count == 1 ?
            audioEventReferences[0] :
            audioEventReferences[Random.Range(0, audioEventReferences.Count)];
    }
}