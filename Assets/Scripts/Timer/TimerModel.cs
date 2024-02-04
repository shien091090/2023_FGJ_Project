using System;

public class TimerModel : ITimerModel
{
    public float CurrentTime { get; private set; }
    public TimerState CurrentTimerState { get; set; }
    private float updateEventTimer;

    public TimerModel()
    {
        ClearData();
    }

    public event Action<TimerUpdateEventInfo> OnUpdateTimer;

    public void SetPause(bool isPause)
    {
        if (CurrentTimerState != TimerState.Running && CurrentTimerState != TimerState.Paused)
            return;

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
        ClearData();
        CurrentTimerState = TimerState.Running;
        OnUpdateTimer?.Invoke(new TimerUpdateEventInfo(0));
    }

    public void Reset()
    {
        ClearData();
    }

    private void ClearData()
    {
        CurrentTime = 0;
        updateEventTimer = 0;
        CurrentTimerState = TimerState.Stopped;
    }
}