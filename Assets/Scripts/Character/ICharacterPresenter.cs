using System;
using SNShien.Common.AdapterTools;
using UnityEngine;

public interface ICharacterPresenter
{
    Vector3 CurrentPos { get; }
    bool IsFaceRight { get; }
    IRigidbody2DAdapter GetRigidbody { get; }
    void CallUpdate();
    void BindView(ICharacterView view);
    void Jump();
    void PlayDieEffect(Action afterDieAnimationCallback, Action afterBackOriginCallback);
    void Teleport();
    void PlayProtectionEffect();
    void PlaySuperJumpEffect();
    void PlayCollidePlatformEffect(bool isLanding);
    void PlayBackToOriginEffect(Action callback);
    void PlayerMoveEffect(float moveValue);
    void PlayExitHouseEffect(Action callback);
    void PlayEnterHouseEffect(Action callback);
    void PlayProtectionEndEffect();
}