using System;
using UnityEngine;

public class CharacterMoveModel
{
    private readonly IMoveController moveController;
    public event Action<float> OnHorizontalMove;

    public CharacterMoveModel(IMoveController moveController)
    {
        this.moveController = moveController;
    }

    public void UpdateMove(int deltaTime, float speed)
    {
        OnHorizontalMove?.Invoke(moveController.GetHorizontalAxis() * Time.deltaTime * speed);
    }
}