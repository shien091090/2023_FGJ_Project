using System;
using UnityEngine;

public interface ICharacterView
{
    float JumpForce { get; }
    float Speed { get; }
    float JumpDelaySeconds { get; }
    float InteractDistance { get; }
    Vector3 FootPointPosition { get; }
    float FootRadius { get; }
    float FallDownLimitPosY { get; }
    void SetProtectionActive(bool isActive);
    void SetSpriteFlipX(bool flipX);
    void PlayAnimation(string animationKey);
    void Waiting(float seconds, Action callback);
    void Translate(Vector3 moveVector);
}