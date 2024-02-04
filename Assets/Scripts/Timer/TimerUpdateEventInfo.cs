public class TimerUpdateEventInfo
{
    public float CurrentTime { get; }

    public TimerUpdateEventInfo(float currentTime)
    {
        CurrentTime = currentTime;
    }

    public string GetTimerString(TimerStringFormatType formatType)
    {
        string hour = ((int)CurrentTime / 3600).ToString("00");
        string minute = ((int)CurrentTime / 60 % 60).ToString("00");
        string second = ((int)CurrentTime % 60).ToString("00");

        switch (formatType)
        {
            case TimerStringFormatType.HHMMSS:
                return CurrentTime >= 360000 ?
                    "99:59:59+" :
                    $"{hour}:{minute}:{second}";

            case TimerStringFormatType.MMSS:
                return CurrentTime >= 3600 ?
                    "59:59+" :
                    $"{minute}:{second}";

            case TimerStringFormatType.SS:
                break;
        }

        return string.Empty;
    }
}