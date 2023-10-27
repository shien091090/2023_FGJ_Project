public interface ICharacterEventHandler
{
    CharacterState CurrentCharacterState { get; }
    void ChangeCurrentCharacterState(CharacterState state);
}