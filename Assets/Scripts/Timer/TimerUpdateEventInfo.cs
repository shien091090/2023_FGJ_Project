public class TimerUpdateEventInfo
{
    private readonly float currentTime;

    public TimerUpdateEventInfo(float currentTime)
    {
        this.currentTime = currentTime;
    }

    public string GetTimerString(TimerStringFormatType formatType)
    {
        switch (formatType)
        {
            case TimerStringFormatType.HHMMSS:
                string hour = ((int)currentTime / 3600).ToString("00");
                string minute = ((int)currentTime / 60 % 60).ToString("00");
                string second = ((int)currentTime % 60).ToString("00");
                return $"{hour}:{minute}:{second}";

            case TimerStringFormatType.MMSS:
                break;

            case TimerStringFormatType.SS:
                break;
        }

        return string.Empty;
    }
}