public interface IKeyController
{
    bool IsJumpKeyDown { get; }
    bool IsInteractKeyDown { get; }
    bool IsUseItemKeyDown(int itemSlotIndex);
}