using System;

public class TimerModel
{
    private float updateEventTimer;
    public event Action OnUpdateTimer;
    public float CurrentTime { get; private set; }

    public void UpdateTimer(float deltaTime)
    {
        CurrentTime += deltaTime;
        updateEventTimer += deltaTime;

        while (updateEventTimer >= 1)
        {
            OnUpdateTimer?.Invoke();
            updateEventTimer -= 1;
        }
    }

    public void StartTimer()
    {
        CurrentTime = 0;
        updateEventTimer = 0;
    }
}