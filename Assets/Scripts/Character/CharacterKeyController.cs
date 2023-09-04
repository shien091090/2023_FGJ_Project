using UnityEngine;

public class CharacterKeyController : IKeyController
{
    public bool IsJumpKeyDown => Input.GetKeyDown(KeyCode.W);
    public bool IsInteractKeyDown => Input.GetMouseButtonDown(0);
    public bool IsUseItemKeyDown(int itemSlotIndex)
    {
        switch (itemSlotIndex)
        {
            case 0:
                return Input.GetKeyDown(KeyCode.Alpha1);
            
            case 1:
                return Input.GetKeyDown(KeyCode.Alpha2);
        }

        return false;
    }
}