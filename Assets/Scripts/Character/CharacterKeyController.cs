using UnityEngine;

public class CharacterKeyController : IKeyController
{
    public bool IsJumpKeyDown => Input.GetKeyDown(KeyCode.W);
    public bool IsSuperJumpKeyDown => Input.GetKeyDown(KeyCode.E);
    public bool IsInteractKeyDown => Input.GetMouseButtonDown(0);
}