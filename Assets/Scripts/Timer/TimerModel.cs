using System;

public class TimerModel
{
    private float updateEventTimer;
    public event Action<TimerUpdateEventInfo> OnUpdateTimer;
    public float CurrentTime { get; private set; }

    public void UpdateTimer(float deltaTime)
    {
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
    }
}