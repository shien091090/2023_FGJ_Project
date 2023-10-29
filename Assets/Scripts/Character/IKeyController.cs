public interface IKeyController
{
    bool IsJumpKeyDown { get; }
    bool IsInteractKeyDown { get; }
    bool IsLeftKeyDown { get; }
    bool IsRightKeyDown { get; }
    bool IsUseItemKeyDown(int itemSlotIndex);
}