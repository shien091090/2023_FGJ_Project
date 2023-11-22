public interface IGameEventHandler
{
    CharacterState CurrentCharacterState { get; }
    void ChangeCurrentCharacterState(CharacterState state);
}