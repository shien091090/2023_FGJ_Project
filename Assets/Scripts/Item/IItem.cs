using System;
using UnityEngine;

public interface IItem
{
    event Action<IItem> OnItemUseCompleted;
    ItemType ItemType { get; }
    void SetPos(Vector3 pos);
    void RemoveItem();
    void UseItem();
}