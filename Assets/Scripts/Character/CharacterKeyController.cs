using UnityEngine;

public class CharacterKeyController : IKeyController
{
    public bool IsJumpKeyDown => Input.GetKeyDown(KeyCode.UpArrow);
    public bool IsInteractKeyDown => Input.GetKeyDown(KeyCode.Space);
    public bool IsUseItemKeyDown(int itemSlotIndex)
    {
        switch (itemSlotIndex)
        {
            case 0:
                return Input.GetKeyDown(KeyCode.Alpha1);
            
            case 1:
                return Input.GetKeyDown(KeyCode.Alpha2);
            
            case 2:
                return Input.GetKeyDown(KeyCode.Alpha3);
            
            case 3:
                return Input.GetKeyDown(KeyCode.Alpha4);
        }

        return false;
    }
}