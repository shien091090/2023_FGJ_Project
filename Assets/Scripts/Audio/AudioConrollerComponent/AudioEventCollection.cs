using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Sirenix.OdinInspector;
using SNShien.Common.DataTools;
using UnityEngine;

namespace SNShien.Common.AudioTools
{
    [Serializable]
    public struct AudioEventCollection
    {
        [SerializeField] [ValueDropdown("GetArchitectureEventList")]
        private string triggerEventName;

        [SerializeField] [FoldoutGroup("$GetEventCaption")]
        private AudioTriggerEventActionType actionType;

        [SerializeField] [FoldoutGroup("$GetEventCaption")] [HideIf("actionType", AudioTriggerEventActionType.Stop)]
        private EventReference audioEventReference;

        [SerializeField] [FoldoutGroup("$GetEventCaption")] [HideIf("actionType", AudioTriggerEventActionType.PlayOneShot)]
        private int trackIndex;

        private Dictionary<string, Type> architectureEventTypeNameDict;

        public string GetEventCaption()
        {
#if UNITY_EDITOR
            switch (actionType)
            {
                case AudioTriggerEventActionType.PlayOneShot:
                    return $"[{actionType.ToString()}] {audioEventReference.Path}";

                case AudioTriggerEventActionType.Play:
                    return $"[{actionType.ToString()} {trackIndex}] {audioEventReference.Path}";

                case AudioTriggerEventActionType.Stop:
                    return $"[{actionType.ToString()}] {trackIndex}";
            }
#endif

            return string.Empty;
        }

        public Type GetTriggerEventTypeName()
        {
            if (architectureEventTypeNameDict == null)
                InitEventTypeNameDict();

            return architectureEventTypeNameDict[triggerEventName];
        }

        public EventReference GetAudioEventReference => audioEventReference;
        public int GetTrackIndex => trackIndex;

        public AudioTriggerEventActionType GetActionType => actionType;

        public IEnumerable<string> GetArchitectureEventList()
        {
            if (architectureEventTypeNameDict == null)
                InitEventTypeNameDict();

            List<string> typeNames = architectureEventTypeNameDict.Keys.ToList();
            return typeNames;
        }

        private void InitEventTypeNameDict()
        {
            if (ReflectionManager.HaveAssemblyStorageSource(typeof(IAudioTriggerEvent)) == false)
                ReflectionManager.AddAssemblyStorage(typeof(IAudioTriggerEvent));

            List<Type> types = ReflectionManager.GetInheritedTypes<IAudioTriggerEvent>().ToList();
            types.Remove(typeof(IAudioTriggerEvent));

            architectureEventTypeNameDict = types.ToDictionary(x => x.Name);
        }
    }
}