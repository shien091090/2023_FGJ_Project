using UnityEngine;

public class CharacterKeyController : IKeyController
{
    public bool IsJumpKeyDown => Input.GetKeyDown(KeyCode.W);
    public bool IsInteractKeyDown => Input.GetMouseButtonDown(0);
}