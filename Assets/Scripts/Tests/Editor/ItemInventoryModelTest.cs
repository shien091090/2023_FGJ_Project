using System.Collections.Generic;
using NUnit.Framework;

public class ItemInventoryModelTest
{
    private ItemInventoryModel itemInventoryModel;

    [SetUp]
    public void Setup()
    {
        itemInventoryModel = new ItemInventoryModel();
    }

    [Test]
    //沒有道具時, 所有道具格都是空的
    public void all_item_slot_is_empty_when_no_item()
    {
        ItemCountShouldBe(0);
    }

    private void ItemCountShouldBe(int expectedItemCount)
    {
        List<IItem> items = itemInventoryModel.GetItems;
        Assert.AreEqual(expectedItemCount, items.Count);
    }

    //獲得一個道具, 第一格道具格有道具
    //獲得多個道具, 按照順序放入道具格
    //有多個道具時, 最後一格道具使用完畢後, 消失
    //有多個道具時, 使用前面的道具, 後面的道具會往前移動
    //只剩一個道具時, 使用後, 道具格變空的
}