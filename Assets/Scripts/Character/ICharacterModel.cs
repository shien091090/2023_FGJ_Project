using System;
using SNShien.Common.AdapterTools;

public interface ICharacterModel : ICollider2DHandler
{
    event Action OnTriggerInteractiveObject;
    event Action OnUnTriggerInteractiveObject;
    event Action OnCharacterDie;
    event Action<CharacterState> OnChangeCharacterState;
    CharacterState CurrentCharacterState { get; }
    void CallUpdate();
    void ColliderTriggerExitWall(bool isRightWall);
    void ColliderTriggerEnterWall(bool isRightWall);
    void ColliderTriggerEnter2D(ICollider2DAdapter col);
    void ColliderTriggerExit2D(ICollider2DAdapter col);
    void ColliderTriggerStay2D(ICollider2DAdapter col);
    void BindPresenter(ICharacterPresenter presenter);
}