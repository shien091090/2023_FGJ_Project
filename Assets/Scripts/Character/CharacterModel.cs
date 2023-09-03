using System;
using UnityEngine;

public class CharacterModel
{
    private readonly IMoveController moveController;
    private readonly IKeyController keyController;
    private readonly ITeleport teleport;
    private readonly ITransform characterTransform;
    private float jumpTimer;
    private float jumpDelaySeconds;
    private float fallDownTimer;
    private ITeleportGate currentTriggerTeleportGate;
    private ITransform selfTransform;

    public event Action<float> OnHorizontalMove;
    public event Action<float> OnJump;

    public bool IsJumping { get; private set; }
    public float FallDownTime { get; set; }
    public bool IsStayOnFloor { get; set; }

    public float InteractiveDistance { get; set; }
    public bool HaveInteractGate => currentTriggerTeleportGate != null;

    public CharacterModel(IMoveController moveController, IKeyController keyController, ITeleport teleport, ITransform characterTransform)
    {
        this.moveController = moveController;
        this.keyController = keyController;
        this.teleport = teleport;
        selfTransform = characterTransform;
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

    public void UpdateFallDownTimer(float deltaTime)
    {
        if (IsStayOnFloor)
        {
            fallDownTimer = 0;
            return;
        }

        fallDownTimer += deltaTime;
        if (fallDownTimer >= FallDownTime)
        {
            IsStayOnFloor = true;
            fallDownTimer = 0;
            teleport.BackToOrigin();
        }
    }

    public void UpdateCheckInteract()
    {
        if (!keyController.IsInteractKeyDown)
            return;

        if (selfTransform == null)
            return;

        float distance = Vector3.Distance(selfTransform.position, currentTriggerTeleportGate.GetPos);
        if (distance > InteractiveDistance)
        {
            currentTriggerTeleportGate = null;
            return;
        }

        if (HaveInteractGate)
            currentTriggerTeleportGate.Teleport(selfTransform);
    }

    public void SetJumpDelay(float jumpDelaySeconds)
    {
        this.jumpDelaySeconds = jumpDelaySeconds;
        jumpTimer = jumpDelaySeconds;
    }

    public void SetFallDownTime(float fallDownTime)
    {
        FallDownTime = fallDownTime;
    }

    public void SetInteractDistance(float distance)
    {
        InteractiveDistance = distance;
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
        currentTriggerTeleportGate = teleportGate;
    }
}