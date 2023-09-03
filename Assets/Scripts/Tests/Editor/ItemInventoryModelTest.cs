using System.Collections.Generic;
using NSubstitute;
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

    [Test]
    //獲得一個道具, 第一格道具格有道具
    public void first_item_slot_has_item_when_get_one_item()
    {
        itemInventoryModel.SetSlotLimit(4);

        IItem item = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item);

        ItemCountShouldBe(4);
        ItemTypeShouldBe(ItemType.Shoes, 0);
        ShouldHaveItem(false, 1);
        ShouldHaveItem(false, 2);
        ShouldHaveItem(false, 3);
    }

    [Test]
    //獲得多個道具, 按照順序放入道具格
    public void item_slot_has_item_when_get_multiple_item()
    {
        itemInventoryModel.SetSlotLimit(4);

        itemInventoryModel.AddItem(CreateItem(ItemType.Shoes));
        itemInventoryModel.AddItem(CreateItem(ItemType.Protection));
        itemInventoryModel.AddItem(CreateItem(ItemType.Shoes));
        itemInventoryModel.AddItem(CreateItem(ItemType.Weapon));

        ItemCountShouldBe(4);
        ItemTypeShouldBe(ItemType.Shoes, 0);
        ItemTypeShouldBe(ItemType.Protection, 1);
        ItemTypeShouldBe(ItemType.Shoes, 2);
        ItemTypeShouldBe(ItemType.Weapon, 3);
    }

    private void ShouldHaveItem(bool expectedHaveItem, int slotIndex)
    {
        Assert.AreEqual(expectedHaveItem, itemInventoryModel.HaveItem(slotIndex));
    }

    private void ItemTypeShouldBe(ItemType expectedItemType, int slotIndex)
    {
        Assert.AreEqual(expectedItemType, itemInventoryModel.GetItem(slotIndex).ItemType);
    }

    private void ItemCountShouldBe(int expectedItemCount)
    {
        Assert.AreEqual(expectedItemCount, itemInventoryModel.GetCurrentItemCount);
    }

    private IItem CreateItem(ItemType itemType)
    {
        IItem item = Substitute.For<IItem>();
        item.ItemType.Returns(itemType);
        return item;
    }

    //有多個道具時, 最後一格道具使用完畢後, 消失
    //有多個道具時, 使用前面的道具, 後面的道具會往前移動
    //只剩一個道具時, 使用後, 道具格變空的
    //道具欄裡有相同道具時, 不可再放入相同道具
    //道具欄已滿時, 不可再放入道具
}