public interface IKeyController
{
    bool IsJumpKeyDown { get; }
    bool IsSuperJumpKeyDown { get; }
    bool IsInteractKeyDown { get; }
    bool IsUseItemKeyDown(int itemSlotIndex);
}