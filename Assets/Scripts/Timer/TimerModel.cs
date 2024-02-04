using System;

public class TimerModel
{
    private float updateEventTimer;

    public event Action<TimerUpdateEventInfo> OnUpdateTimer;
    public float CurrentTime { get; private set; }
    public TimerState CurrentTimerState { get; set; }

    public TimerModel()
    {
        CurrentTimerState = TimerState.Stopped;
    }

    public void SetPause(bool isPause)
    {
        CurrentTimerState = isPause ?
            TimerState.Paused :
            TimerState.Running;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (CurrentTimerState != TimerState.Running)
            return;

        CurrentTime += deltaTime;
        updateEventTimer += deltaTime;

        if (updateEventTimer >= 1)
        {
            OnUpdateTimer?.Invoke(new TimerUpdateEventInfo((float)Math.Floor(CurrentTime)));
            updateEventTimer %= 1;
        }
    }

    public void StartTimer()
    {
        CurrentTime = 0;
        updateEventTimer = 0;
        CurrentTimerState = TimerState.Running;
    }
}