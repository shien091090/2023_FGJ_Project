public class TimerUpdateEventInfo
{
    public float CurrentTime { get; }

    public TimerUpdateEventInfo(float currentTime)
    {
        CurrentTime = currentTime;
    }

    public string GetTimerString(TimerStringFormatType formatType)
    {
        switch (formatType)
        {
            case TimerStringFormatType.HHMMSS:
                string hour = ((int)CurrentTime / 3600).ToString("00");
                string minute = ((int)CurrentTime / 60 % 60).ToString("00");
                string second = ((int)CurrentTime % 60).ToString("00");
                return $"{hour}:{minute}:{second}";

            case TimerStringFormatType.MMSS:
                break;

            case TimerStringFormatType.SS:
                break;
        }

        return string.Empty;
    }
}