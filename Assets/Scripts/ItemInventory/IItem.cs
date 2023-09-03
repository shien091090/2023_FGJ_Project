using System;

public interface IItem
{
    event Action<IItem> OnItemUsed;
    ItemType ItemType { get; }
    void RemoveItem();
}