using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemInventoryModel : IItemInventoryModel
{
    private static IItemInventoryModel _instance;
    private readonly IKeyController keyController;

    private ItemInventoryView view;
    private List<IItem> GetItems;
    private Vector3[] itemSlotPosArray;

    public static IItemInventoryModel Instance => _instance;
    public int ItemCountLimit { get; set; }

    public int GetCurrentItemCount => GetItems.Count(x => x != null);

    public ItemInventoryModel(IKeyController keyController)
    {
        this.keyController = keyController;
        GetItems = new List<IItem>();

        _instance = this;
    }

    public bool AlreadyHaveSpecificTypeItem(ItemType itemType)
    {
        return GetItems.FirstOrDefault(x => x != null && x.ItemType == itemType) != null;
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

    public bool CheckAddItem(ItemType itemType)
    {
        IItem item = view.GetItemObject(itemType);
        
        if (AlreadyHaveSpecificTypeItem(item.ItemType))
            return false;

        for (int index = 0; index < GetItems.Count; index++)
        {
            IItem currentItem = GetItems[index];
            if (currentItem != null)
                continue;

            GetItems[index] = item;
            item.SetPos(itemSlotPosArray[index]);

            item.OnItemUseCompleted -= OnItemUseCompleted;
            item.OnItemUseCompleted += OnItemUseCompleted;

            return true;
        }

        return false;
    }

    public void UpdateCheckUseItem()
    {
        for (int i = 0; i < ItemCountLimit; i++)
        {
            if (keyController.IsUseItemKeyDown(i) && HaveItem(i))
            {
                IItem item = GetItem(i);
                item.UseItem();
            }
        }
    }

    public bool HaveItem(int index)
    {
        return GetItems[index] != null;
    }

    public void BindView(ItemInventoryView view)
    {
        this.view = view;
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

    private void OnItemUseCompleted(IItem item)
    {
        RemoveItemAndShift(item);
        item.RemoveItem();
        item.OnItemUseCompleted -= OnItemUseCompleted;
    }
}