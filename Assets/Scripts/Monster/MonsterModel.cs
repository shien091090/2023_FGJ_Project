using System;

public class MonsterModel
{
    private readonly int keepStunTime;
    private float stunTimer;

    public event Action<MonsterState> OnChangeState;
    public MonsterState CurrentState { get; private set; }

    public MonsterModel(int keepStunTime)
    {
        this.keepStunTime = keepStunTime;
        CurrentState = MonsterState.Normal;
    }

    public void UpdateStunTimer(float deltaTime)
    {
        if (CurrentState == MonsterState.Normal)
            return;

        stunTimer += deltaTime;

        if (stunTimer >= keepStunTime)
        {
            CurrentState = MonsterState.Normal;
            stunTimer = 0;
            OnChangeState?.Invoke(MonsterState.Normal);
        }
    }

    public void BeAttack()
    {
        bool isSendEvent = CurrentState != MonsterState.Stun;

        CurrentState = MonsterState.Stun;
        stunTimer = 0;

        if (isSendEvent)
            OnChangeState?.Invoke(MonsterState.Stun);
    }
}