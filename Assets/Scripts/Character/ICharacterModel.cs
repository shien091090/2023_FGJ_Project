using System;
using SNShien.Common.AdapterTools;
using UnityEngine;

public interface ICharacterModel : ICollider2DHandler
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
    void ColliderTriggerEnter2D(ICollider2DAdapter col);
    void ColliderTriggerExit2D(ICollider2DAdapter col);
    void ColliderTriggerStay2D(ICollider2DAdapter col);
}