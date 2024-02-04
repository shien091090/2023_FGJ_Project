using System;

public interface ITimerModel
{
    event Action<TimerUpdateEventInfo> OnUpdateTimer;
    float CurrentTime { get; }
    TimerState CurrentTimerState { get; set; }
    void SetPause(bool isPause);
    void UpdateTimer(float deltaTime);
    void StartTimer();
    void Reset();
}