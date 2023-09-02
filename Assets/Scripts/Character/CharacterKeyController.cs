using UnityEngine;

public class CharacterKeyController : IKeyController
{
    public bool IsJumpKeyDown => Input.GetKeyDown(KeyCode.W);
}