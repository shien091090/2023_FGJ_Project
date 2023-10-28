using System;
using UnityEngine;

public interface ICharacterView
{
    float JumpForce { get; }
    float Speed { get; }
    float JumpDelaySeconds { get; }
    float InteractDistance { get; }
    float FallDownLimitPosY { get; }
    void SetProtectionActive(bool isActive);
    void SetFaceDirectionScale(int scale);
    void SetActive(bool isActive);
    void PlayAnimation(string animationKey);
    void Waiting(float seconds, Action callback);
    void Translate(Vector3 moveVector);
}