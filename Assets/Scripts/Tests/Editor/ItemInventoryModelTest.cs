using System;
using NSubstitute;
using NUnit.Framework;

public class ItemInventoryModelTest
{
    private ItemInventoryModel itemInventoryModel;

    private void CallItemUseEvent(IItem item)
    {
        item.OnItemUsed += Raise.Event<Action<IItem>>(item);
    }

    [SetUp]
    public void Setup()
    {
        itemInventoryModel = new ItemInventoryModel();
    }

    [Test]
    //沒有道具時, 所有道具格都是空的
    public void all_item_slot_is_empty_when_no_item()
    {
        CurrentItemCountShouldBe(0);
    }

    [Test]
    //獲得一個道具, 第一格道具格有道具
    public void first_item_slot_has_item_when_get_one_item()
    {
        itemInventoryModel.SetSlotLimit(4);

        IItem item = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item);

        CurrentItemCountShouldBe(1);
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
        itemInventoryModel.AddItem(CreateItem(ItemType.Weapon));

        CurrentItemCountShouldBe(3);
        ItemTypeShouldBe(ItemType.Shoes, 0);
        ItemTypeShouldBe(ItemType.Protection, 1);
        ItemTypeShouldBe(ItemType.Weapon, 2);
    }

    [Test]
    //有多個道具時, 最後一格道具使用完畢後, 消失
    public void last_item_slot_is_empty_when_use_last_item()
    {
        itemInventoryModel.SetSlotLimit(4);

        IItem item1 = CreateItem(ItemType.Shoes);
        IItem item2 = CreateItem(ItemType.Protection);
        IItem item3 = CreateItem(ItemType.Weapon);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);
        itemInventoryModel.AddItem(item3);

        CallItemUseEvent(item3);

        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Shoes, 0);
        ItemTypeShouldBe(ItemType.Protection, 1);
        ShouldHaveItem(false, 2);
    }
    
    [Test]
    //有多個道具時, 使用前面的道具, 後面的道具會往前移動
    public void item_slot_move_forward_when_use_item()
    {
        itemInventoryModel.SetSlotLimit(4);

        IItem item1 = CreateItem(ItemType.Protection);
        IItem item2 = CreateItem(ItemType.Weapon);
        IItem item3 = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);
        itemInventoryModel.AddItem(item3);

        CallItemUseEvent(item2);

        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Protection, 0);
        ItemTypeShouldBe(ItemType.Shoes, 1);
        ShouldHaveItem(false, 2);
    }
    
    [Test]
    //有多個道具時, 連續使用兩個道具
    public void item_slot_move_forward_when_use_two_item()
    {
        itemInventoryModel.SetSlotLimit(4);

        IItem item1 = CreateItem(ItemType.Protection);
        IItem item2 = CreateItem(ItemType.Weapon);
        IItem item3 = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);
        itemInventoryModel.AddItem(item3);

        CallItemUseEvent(item1);
        CallItemUseEvent(item3);

        CurrentItemCountShouldBe(1);
        ItemTypeShouldBe(ItemType.Weapon, 0);
        ShouldHaveItem(false, 1);
        ShouldHaveItem(false, 2);
    }
    
    private void ShouldHaveItem(bool expectedHaveItem, int slotIndex)
    {
        Assert.AreEqual(expectedHaveItem, itemInventoryModel.HaveItem(slotIndex));
    }

    private void ItemTypeShouldBe(ItemType expectedItemType, int slotIndex)
    {
        Assert.AreEqual(expectedItemType, itemInventoryModel.GetItem(slotIndex).ItemType);
    }

    private void CurrentItemCountShouldBe(int expectedItemCount)
    {
        Assert.AreEqual(expectedItemCount, itemInventoryModel.GetCurrentItemCount);
    }

    private IItem CreateItem(ItemType itemType)
    {
        IItem item = Substitute.For<IItem>();
        item.ItemType.Returns(itemType);
        return item;
    }

    //只剩一個道具時, 使用後, 道具格變空的
    //道具欄裡有相同道具時, 不可再放入相同道具
    //道具欄已滿時, 不可再放入道具
}