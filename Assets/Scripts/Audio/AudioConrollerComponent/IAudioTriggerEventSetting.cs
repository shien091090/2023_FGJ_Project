using System.Collections.Generic;
using FMODUnity;

namespace SNShien.Common.AudioTools
{
    public interface IAudioTriggerEventSetting
    {
        List<AudioEventCollection> GetAudioEventCollections { get; }
        EventReference GetAudioEventReference(string triggerTypeName);
        int GetAudioEventTrackIndex(string triggerTypeName);
    }
}