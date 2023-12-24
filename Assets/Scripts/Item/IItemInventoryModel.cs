using UnityEngine;

public interface IItemInventoryModel
{
    bool CheckAddItem(ItemType itemType);
    bool AlreadyHaveSpecificTypeItem(ItemType itemType);
    void SetSlotLimit(params Vector3[] slotPosArray);
    void UpdateCheckUseItem();
    void BindView(ItemInventoryView view);
}