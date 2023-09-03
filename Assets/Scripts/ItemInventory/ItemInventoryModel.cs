using System.Collections.Generic;
using System.Linq;

public class ItemInventoryModel
{
    private List<IItem> GetItems;

    public int ItemCountLimit { get; set; }

    public int GetCurrentItemCount => GetItems.Count(x => x != null);

    public ItemInventoryModel()
    {
        GetItems = new List<IItem>();
    }

    public IItem GetItem(int index)
    {
        return GetItems[index];
    }

    public void SetSlotLimit(int limit)
    {
        ItemCountLimit = limit;
        GetItems = new List<IItem>(new IItem[limit]);
    }

    public void AddItem(IItem item)
    {
        for (int index = 0; index < GetItems.Count; index++)
        {
            IItem currentItem = GetItems[index];
            if (currentItem != null)
                continue;

            GetItems[index] = item;

            item.OnItemUsed -= OnItemUse;
            item.OnItemUsed += OnItemUse;

            return;
        }
    }

    public bool HaveItem(int index)
    {
        return GetItems[index] != null;
    }

    private void RemoveItemAndShift(IItem item)
    {
        if (GetItems.Contains(item) == false)
            return;

        int index = GetItems.IndexOf(item);
        GetItems[index] = null;

        for (int i = index; i < GetItems.Count - 1; i++)
        {
            GetItems[i] = GetItems[i + 1];
        }
    }

    private void OnItemUse(IItem item)
    {
        RemoveItemAndShift(item);
        item.OnItemUsed -= OnItemUse;
    }
}