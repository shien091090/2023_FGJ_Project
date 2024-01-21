using System;
using SNShien.Common.AdapterTools;
using UnityEngine;

public interface ICharacterView
{
    float JumpForce { get; }
    float Speed { get; }
    float SuperJumpForce { get; }
    float JumpDelaySeconds { get; }
    float InteractDistance { get; }
    float FallDownLimitPosY { get; }
    IRigidbody2DAdapter GetRigidbody { get; }
    void SetProtectionActive(bool isActive);
    void SetFaceDirectionScale(int scale);
    void SetActive(bool isActive);
    void SetWalkAnimation(bool isWaling);
    void PlayAnimation(string animationKey);
    void Waiting(float seconds, Action callback);
    void Translate(Vector3 moveVector);
}