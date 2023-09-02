using UnityEngine;

public class CharacterMoveController : IMoveController
{
    public float GetHorizontalAxis()
    {
        return Input.GetAxis("Horizontal");
    }
}