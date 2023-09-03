using System.Collections.Generic;

public class ItemInventoryModel
{
    private List<IItem> GetItems;

    public int ItemCountLimit { get; set; }
    public int GetCurrentItemCount => GetItems.Count;

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
            return;
        }
    }

    public bool HaveItem(int index)
    {
        return GetItems[index] != null;
    }
}