using System;

public class ItemTriggerHandler : IItemTriggerHandler
{
    private static ItemTriggerHandler _instance;
    public static ItemTriggerHandler Instance => _instance;

    public ItemTriggerHandler()
    {
        _instance = this;
    }

    public event Action<ItemType> OnEndItemEffect;
    public event Action<ItemType> OnStartItemEffect;
    public event Action<ItemType> OnUseItemOneTime;

    public void StartItemEffect(ItemType itemType)
    {
        OnStartItemEffect?.Invoke(itemType);
    }

    public void EndItemEffect(ItemType itemType)
    {
        OnEndItemEffect?.Invoke(itemType);
    }

    public void TriggerItemOneTime(ItemType itemType)
    {
        OnUseItemOneTime?.Invoke(itemType);
    }
}