using System;

public interface IItemTriggerHandler
{
    event Action<ItemType> OnEndItemEffect;
    event Action<ItemType> OnStartItemEffect;
    event Action<ItemType> OnUseItemOneTime;
    void StartItemEffect(ItemType itemType);
    void EndItemEffect(ItemType itemType);
    void TriggerItemOneTime(ItemType itemType);
}