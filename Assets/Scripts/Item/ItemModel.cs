using System;
using UnityEngine;

public class ItemModel : IItem
{
    public event Action<IItem> OnItemUsed;
    public ItemType ItemType { get; }
    public void SetPos(Vector3 pos)
    {
        throw new NotImplementedException();
    }

    public void RemoveItem()
    {
        throw new NotImplementedException();
    }

    public void UseItem()
    {
        throw new NotImplementedException();
    }
}