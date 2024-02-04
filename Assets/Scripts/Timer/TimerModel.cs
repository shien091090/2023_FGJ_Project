using System;

public class TimerModel
{
    private float updateEventTimer;

    public event Action<TimerUpdateEventInfo> OnUpdateTimer;
    public float CurrentTime { get; private set; }
    public TimerState CurrentTimerState { get; set; }

    public TimerModel()
    {
        ClearData();
    }

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