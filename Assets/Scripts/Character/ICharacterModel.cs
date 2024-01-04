using System;
using UnityEngine;

public interface ICharacterModel : IColliderHandler
{
    event Action OnTriggerInteractiveObject;
    event Action OnUnTriggerInteractiveObject;
    event Action OnCharacterDie;
    event Action<CharacterState> OnChangeCharacterState;
    CharacterState CurrentCharacterState { get; }
    bool IsFaceRight { get; }
    Vector3 CurrentPos { get; }
    void CallUpdate();
    void BindView(ICharacterView view);
    void ColliderTriggerExitWall(bool isRightWall);
    void ColliderTriggerEnterWall(bool isRightWall);
    void ColliderTriggerEnter(ICollider col);
    void ColliderTriggerExit(ICollider col);
    void ColliderTriggerStay(ICollider col);
}