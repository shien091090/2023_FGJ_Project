using System;

public class CharacterMoveModel
{
    private readonly IMoveController moveController;
    private readonly IKeyController keyController;

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
        if (keyController.IsJumpKeyDown && IsJumping == false)
        {
            IsJumping = true;
            OnJump?.Invoke(jumpForce);
        }
    }

    public void TriggerFloor()
    {
        IsJumping = false;
    }
}