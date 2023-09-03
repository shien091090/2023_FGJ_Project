using System;

public class ItemModel
{
    private readonly ItemUseType itemUseType;
    private readonly float useLimit;
    private int currentUseTimes;

    public event Action OnItemUseComplete;
    public event Action<int> OnRefreshCurrentUseTimes;

    public ItemModel(ItemUseType itemUseType, float useLimit)
    {
        this.itemUseType = itemUseType;
        this.useLimit = useLimit;
        currentUseTimes = (int)useLimit;
    }

    public void UseItem()
    {
        if (itemUseType == ItemUseType.PassTime)
            return;

        currentUseTimes--;

        OnRefreshCurrentUseTimes?.Invoke(currentUseTimes);

        if (currentUseTimes <= 0)
            OnItemUseComplete?.Invoke();
    }
}