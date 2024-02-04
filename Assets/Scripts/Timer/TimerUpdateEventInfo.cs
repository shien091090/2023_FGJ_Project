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
                return CurrentTime >= 360000 ?
                    "99:59:59+" :
                    $"{(int)CurrentTime / 3600:00}:{(int)CurrentTime / 60 % 60:00}:{(int)CurrentTime % 60:00}";

            case TimerStringFormatType.MMSS:
                return CurrentTime >= 3600 ?
                    "59:59+" :
                    $"{(int)CurrentTime / 60 % 60:00}:{(int)CurrentTime % 60:00}";

            case TimerStringFormatType.SS:
                return CurrentTime >= 100 ?
                    "99+" :
                    $"{(int)CurrentTime:00}";
        }

        return string.Empty;
    }
}