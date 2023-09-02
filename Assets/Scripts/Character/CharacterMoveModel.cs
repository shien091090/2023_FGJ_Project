using System;
using UnityEngine;

public class CharacterMoveModel
{
    private readonly IMoveController moveController;
    private readonly IKeyController keyController;
    private float jumpTimer;
    private float jumpDelaySeconds;

    public event Action<float> OnHorizontalMove;
    public event Action<float> OnJump;

    public bool IsJumping { get; private set; }

    public CharacterMoveModel(IMoveController moveController, IKeyController keyController)
    {
        this.moveController = moveController;
        this.keyController = keyController;
    }

    public void UpdateMove(float deltaTime, float speed)
    {
        OnHorizontalMove?.Invoke(moveController.GetHorizontalAxis() * deltaTime * speed);
    }

    public void UpdateCheckJump(float jumpForce)
    {
        if (jumpTimer < jumpDelaySeconds)
            return;

        if (keyController.IsJumpKeyDown && IsJumping == false)
        {
            // Debug.Log("OnJump");
            jumpTimer = 0;
            IsJumping = true;
            OnJump?.Invoke(jumpForce);
        }
    }

    public void UpdateJumpTimer(float deltaTime)
    {
        if (jumpTimer >= jumpDelaySeconds)
            return;

        jumpTimer = Math.Min(jumpTimer + deltaTime, jumpDelaySeconds);
    }

    public void SetJumpDelay(float jumpDelaySeconds)
    {
        this.jumpDelaySeconds = jumpDelaySeconds;
        jumpTimer = jumpDelaySeconds;
    }

    public void TriggerFloor()
    {
        if (jumpTimer < jumpDelaySeconds)
            return;
        
        IsJumping = false;
    }
}