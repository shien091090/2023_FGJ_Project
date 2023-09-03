namespace SNShien.Common.AudioTools
{
    public interface IAudioTriggerEvent
    {
        string PreSetParamName { set; get; }
        float PreSetParamValue { set; get; }
    }
}