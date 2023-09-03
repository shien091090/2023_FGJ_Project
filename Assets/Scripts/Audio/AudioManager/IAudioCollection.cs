using System.Collections.Generic;
using FMODUnity;

namespace SNShien.Common.AudioTools
{
    public interface IAudioCollection
    {
        List<string> GetLoadBankNameList { get; }
        EventReference GetEventReference(string audioKey);
    }
}