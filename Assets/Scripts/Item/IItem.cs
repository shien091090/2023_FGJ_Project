using System;
using UnityEngine;

public interface IItem
{
    event Action<IItem> OnItemUsed;
    ItemType ItemType { get; }
    void SetPos(Vector3 pos);
    void RemoveItem();
    void UseItem();
}