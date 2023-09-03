public class MonsterModel
{
    private readonly int keepStunTime;
    private float stunTimer;
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
        }
    }

    public void BeAttack()
    {
        CurrentState = MonsterState.Stun;
        stunTimer = 0;
    }
}