using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.AudioTools
{
    [CreateAssetMenu(fileName = "AudioTriggerEventSetting", menuName = "SNShien/Create AudioTriggerEventSetting")]
    public class AudioTriggerEventScriptableObject : SerializedScriptableObject, IAudioTriggerEventSetting
    {
        [SerializeField] private List<AudioEventCollection> audioEventCollections;

        public List<AudioEventCollection> GetAudioEventCollections => audioEventCollections;
        private Dictionary<string, EventReference> audioReferenceDict;
        private Dictionary<string, int> audioTrackIndexDict;

        public EventReference GetAudioEventReference(string triggerTypeName)
        {
            if (audioReferenceDict == null)
                audioReferenceDict = audioEventCollections.ToDictionary(x => x.GetTriggerEventTypeName().Name, x => x.GetAudioEventReference);

            return audioReferenceDict.ContainsKey(triggerTypeName) ?
                audioReferenceDict[triggerTypeName] :
                default;
        }

        public int GetAudioEventTrackIndex(string triggerTypeName)
        {
            if (audioTrackIndexDict == null)
                audioTrackIndexDict = audioEventCollections.ToDictionary(x => x.GetTriggerEventTypeName().Name, x => x.GetTrackIndex);

            return audioTrackIndexDict.ContainsKey(triggerTypeName) ?
                audioTrackIndexDict[triggerTypeName] :
                default;
        }
    }
}