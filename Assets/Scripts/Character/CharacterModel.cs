using System;
using UnityEngine;

public class CharacterModel
{
    private readonly IMoveController moveController;
    private readonly IKeyController keyController;
    private readonly ITeleport teleport;
    private readonly IRigidbody characterRigidbody;
    private readonly IRigidbody selfRigidbody;
    private float jumpTimer;
    private float jumpDelaySeconds;
    private float fallDownTimer;

    public event Action<float> OnHorizontalMove;
    public event Action<float> OnJump;
    public ITeleportGate CurrentTriggerTeleportGate { get; private set; }

    public bool IsJumping { get; private set; }
    public bool IsStayOnFloor { get; set; }

    public float InteractiveDistance { get; set; }
    public bool HaveInteractGate => CurrentTriggerTeleportGate != null;

    public CharacterModel(IMoveController moveController, IKeyController keyController, ITeleport teleport, IRigidbody characterRigidbody)
    {
        this.moveController = moveController;
        this.keyController = keyController;
        this.teleport = teleport;
        selfRigidbody = characterRigidbody;
    }

    public void UpdateMove(float deltaTime, float speed)
    {
        OnHorizontalMove?.Invoke(moveController.GetHorizontalAxis() * deltaTime * speed);
    }

    public void UpdateCheckJump(float jumpForce)
    {
        if (jumpTimer < jumpDelaySeconds)
            return;

        if (keyController.IsJumpKeyDown)
        {
            Jump(jumpForce);
        }
    }

    public void UpdateJumpTimer(float deltaTime)
    {
        if (jumpTimer >= jumpDelaySeconds)
            return;

        jumpTimer = Math.Min(jumpTimer + deltaTime, jumpDelaySeconds);
    }

    // public void UpdateFallDownTimer(float deltaTime)
    // {
    //     if (IsStayOnFloor)
    //     {
    //         fallDownTimer = 0;
    //         return;
    //     }
    //
    //     fallDownTimer += deltaTime;
    //     if (fallDownTimer >= FallDownTime)
    //     {
    //         IsStayOnFloor = true;
    //         fallDownTimer = 0;
    //         teleport.BackToOrigin();
    //     }
    // }

    public void UpdateCheckInteract()
    {
        if (!keyController.IsInteractKeyDown)
            return;

        if (selfRigidbody == null)
            return;

        if (HaveInteractGate == false)
            return;

        float distance = Vector3.Distance(selfRigidbody.position, CurrentTriggerTeleportGate.GetPos);
        if (distance > InteractiveDistance)
        {
            CurrentTriggerTeleportGate = null;
            return;
        }

        if (HaveInteractGate)
            CurrentTriggerTeleportGate.Teleport(selfRigidbody);
    }

    public void SetJumpDelay(float jumpDelaySeconds)
    {
        this.jumpDelaySeconds = jumpDelaySeconds;
        jumpTimer = jumpDelaySeconds;
    }

    public void SetInteractDistance(float distance)
    {
        InteractiveDistance = distance;
    }

    public void Jump(float jumpForce)
    {
        if (IsJumping == false)
        {
            jumpTimer = 0;
            IsJumping = true;
            OnJump?.Invoke(jumpForce);
        }
    }

    public void TriggerFloor()
    {
        IsStayOnFloor = true;
        fallDownTimer = 0;

        if (jumpTimer < jumpDelaySeconds)
            return;

        IsJumping = false;
    }

    public void ExitFloor()
    {
        IsStayOnFloor = false;
    }

    public void TriggerTeleportGate(ITeleportGate teleportGate)
    {
        CurrentTriggerTeleportGate = teleportGate;
    }

    public void ExitTeleportGate()
    {
        CurrentTriggerTeleportGate = null;
    }
}