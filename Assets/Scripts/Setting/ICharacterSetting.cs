public interface ICharacterSetting
{
    float JumpForce { get; }
    float SuperJumpForce { get; }
    float Speed { get; }
    float JumpDelaySeconds { get; }
    float InteractDistance { get; }
    float FallDownLimitPosY { get; }
}