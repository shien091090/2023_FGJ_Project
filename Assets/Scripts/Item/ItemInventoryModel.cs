using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemInventoryModel
{
    private readonly IKeyController keyController;
    private List<IItem> GetItems;
    private Vector3[] itemSlotPosArray;

    public int ItemCountLimit { get; set; }

    public int GetCurrentItemCount => GetItems.Count(x => x != null);

    public ItemInventoryModel(IKeyController keyController)
    {
        this.keyController = keyController;
        GetItems = new List<IItem>();
    }

    public void UpdateCheckUseItem()
    {
        for (int i = 0; i < ItemCountLimit; i++)
        {
            if (keyController.IsUseItemKeyDown(i))
            {
                IItem item = GetItem(i);
                item.UseItem();
            }
        }
    }

    public IItem GetItem(int index)
    {
        return GetItems[index];
    }

    public void SetSlotLimit(params Vector3[] slotPosArray)
    {
        ItemCountLimit = slotPosArray.Length;
        GetItems = new List<IItem>(new IItem[ItemCountLimit]);
        itemSlotPosArray = slotPosArray;
    }

    public void AddItem(IItem item)
    {
        if (AlreadyHaveSpecificTypeItem(item.ItemType))
            return;

        for (int index = 0; index < GetItems.Count; index++)
        {
            IItem currentItem = GetItems[index];
            if (currentItem != null)
                continue;

            GetItems[index] = item;
            item.SetPos(itemSlotPosArray[index]);

            item.OnItemUsed -= OnItemUse;
            item.OnItemUsed += OnItemUse;

            return;
        }
    }

    public bool HaveItem(int index)
    {
        return GetItems[index] != null;
    }

    public bool AlreadyHaveSpecificTypeItem(ItemType itemType)
    {
        return GetItems.FirstOrDefault(x => x != null && x.ItemType == itemType) != null;
    }

    private void RemoveItemAndShift(IItem item)
    {
        if (GetItems.Contains(item) == false)
            return;

        int index = GetItems.IndexOf(item);
        GetItems[index] = null;

        for (int i = index; i < GetItems.Count - 1; i++)
        {
            IItem shiftItem = GetItems[i + 1];
            GetItems[i] = shiftItem;

            shiftItem?.SetPos(itemSlotPosArray[i]);
        }
    }

    private void OnItemUse(IItem item)
    {
        RemoveItemAndShift(item);
        item.OnItemUsed -= OnItemUse;
    }
}