using System.Collections.Generic;

public class ItemInventoryModel
{
    public List<IItem> GetItems;

    public ItemInventoryModel()
    {
        GetItems = new List<IItem>();
    }
}