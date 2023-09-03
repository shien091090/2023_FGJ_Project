using System;

public class MonsterModel
{
    private readonly float keepStunTime;
    private float stunTimer;

    public event Action<MonsterState> OnChangeState;
    public MonsterState CurrentState { get; private set; }
    public bool IsMovable { get; set; }

    public MonsterModel(float keepStunTime)
    {
        this.keepStunTime = keepStunTime;
        CurrentState = MonsterState.Normal;
        IsMovable = true;
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
            IsMovable = true;
            OnChangeState?.Invoke(MonsterState.Normal);
        }
    }

    public void BeAttack()
    {
        bool isSendEvent = CurrentState != MonsterState.Stun;

        CurrentState = MonsterState.Stun;
        IsMovable = false;
        stunTimer = 0;

        if (isSendEvent)
            OnChangeState?.Invoke(MonsterState.Stun);
    }
}