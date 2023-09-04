using System;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

public class ItemInventoryModelTest
{
    private ItemInventoryModel itemInventoryModel;
    private IKeyController keyController;

    [SetUp]
    public void Setup()
    {
        keyController = Substitute.For<IKeyController>();
        itemInventoryModel = new ItemInventoryModel(keyController);
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
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item);
        ShouldCallSetPos(item, new Vector3(5, 5, 0));

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
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item1 = CreateItem(ItemType.Shoes);
        IItem item2 = CreateItem(ItemType.Protection);
        IItem item3 = CreateItem(ItemType.Weapon);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);
        itemInventoryModel.AddItem(item3);

        ShouldCallSetPos(item1, new Vector3(5, 5, 0));
        ShouldCallSetPos(item2, new Vector3(4, 5, 0));
        ShouldCallSetPos(item3, new Vector3(3, 5, 0));

        CurrentItemCountShouldBe(3);
        ItemTypeShouldBe(ItemType.Shoes, 0);
        ItemTypeShouldBe(ItemType.Protection, 1);
        ItemTypeShouldBe(ItemType.Weapon, 2);
    }

    [Test]
    //有多個道具時, 最後一格道具使用完畢後, 消失
    public void last_item_slot_is_empty_when_use_last_item()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item1 = CreateItem(ItemType.Shoes);
        IItem item2 = CreateItem(ItemType.Protection);
        IItem item3 = CreateItem(ItemType.Weapon);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);
        itemInventoryModel.AddItem(item3);

        CallItemUseCompletedEvent(item3);

        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Shoes, 0);
        ItemTypeShouldBe(ItemType.Protection, 1);
        ShouldHaveItem(false, 2);
    }

    [Test]
    //有多個道具時, 使用前面的道具, 後面的道具會往前移動
    public void item_slot_move_forward_when_use_item()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item1 = CreateItem(ItemType.Protection);
        IItem item2 = CreateItem(ItemType.Weapon);
        IItem item3 = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);
        itemInventoryModel.AddItem(item3);

        ShouldCallSetPos(item3, new Vector3(3, 5, 0));

        CallItemUseCompletedEvent(item2);

        ShouldCallSetPos(item3, new Vector3(4, 5, 0));
        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Protection, 0);
        ItemTypeShouldBe(ItemType.Shoes, 1);
        ShouldHaveItem(false, 2);
    }

    [Test]
    //有多個道具時, 連續使用兩個道具
    public void item_slot_move_forward_when_use_two_item()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item1 = CreateItem(ItemType.Protection);
        IItem item2 = CreateItem(ItemType.Weapon);
        IItem item3 = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);
        itemInventoryModel.AddItem(item3);

        ShouldCallSetPos(item3, new Vector3(3, 5, 0));

        CallItemUseCompletedEvent(item1);
        ShouldCallSetPos(item3, new Vector3(4, 5, 0));

        CallItemUseCompletedEvent(item2);
        ShouldCallSetPos(item3, new Vector3(5, 5, 0));

        CurrentItemCountShouldBe(1);
        ItemTypeShouldBe(ItemType.Shoes, 0);
        ShouldHaveItem(false, 1);
        ShouldHaveItem(false, 2);
    }

    [Test]
    //使用道具後再次使用, 不會有反應
    public void item_slot_is_empty_when_use_item_twice()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item1 = CreateItem(ItemType.Protection);
        IItem item2 = CreateItem(ItemType.Weapon);
        IItem item3 = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);
        itemInventoryModel.AddItem(item3);

        CallItemUseCompletedEvent(item1);

        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Weapon, 0);
        ItemTypeShouldBe(ItemType.Shoes, 1);
        ShouldHaveItem(false, 2);

        CallItemUseCompletedEvent(item1);

        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Weapon, 0);
        ItemTypeShouldBe(ItemType.Shoes, 1);
        ShouldHaveItem(false, 2);
    }

    [Test]
    //使用未加入道具欄的道具, 不會有反應
    public void use_item_not_in_inventory()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item1 = CreateItem(ItemType.Protection);
        IItem item2 = CreateItem(ItemType.Weapon);
        IItem item3 = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);

        CallItemUseCompletedEvent(item3);

        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Protection, 0);
        ItemTypeShouldBe(ItemType.Weapon, 1);
        ShouldHaveItem(false, 2);
    }

    [Test]
    //只剩一個道具時, 使用後, 道具格變空的
    public void item_slot_is_empty_when_use_last_item()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item1 = CreateItem(ItemType.Protection);

        itemInventoryModel.AddItem(item1);

        CallItemUseCompletedEvent(item1);

        CurrentItemCountShouldBe(0);
        ShouldHaveItem(false, 0);
        ShouldHaveItem(false, 1);
        ShouldHaveItem(false, 2);
        ShouldHaveItem(false, 3);
    }

    [Test]
    //道具欄裡有相同道具時, 不可再放入相同道具
    public void can_not_add_same_item()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        itemInventoryModel.AddItem(CreateItem(ItemType.Protection));

        CurrentItemCountShouldBe(1);
        ItemTypeShouldBe(ItemType.Protection, 0);
        ShouldHaveItem(false, 1);
        ShouldAlreadyHaveSpecificTypeItem(true, ItemType.Protection);

        itemInventoryModel.AddItem(CreateItem(ItemType.Protection));

        CurrentItemCountShouldBe(1);
        ItemTypeShouldBe(ItemType.Protection, 0);
        ShouldHaveItem(false, 1);
    }

    [Test]
    //道具欄已滿時, 不可再放入道具
    public void can_not_add_item_when_inventory_is_full()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0));

        itemInventoryModel.AddItem(CreateItem(ItemType.Protection));
        itemInventoryModel.AddItem(CreateItem(ItemType.Weapon));

        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Protection, 0);
        ItemTypeShouldBe(ItemType.Weapon, 1);

        itemInventoryModel.AddItem(CreateItem(ItemType.Shoes));

        CurrentItemCountShouldBe(2);
        ItemTypeShouldBe(ItemType.Protection, 0);
        ItemTypeShouldBe(ItemType.Weapon, 1);
    }

    [Test]
    //擁有多個道具時, 按下指定按鍵使用道具
    public void use_item_by_key()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item = CreateItem(ItemType.Weapon);

        itemInventoryModel.AddItem(CreateItem(ItemType.Protection));
        itemInventoryModel.AddItem(item);
        itemInventoryModel.AddItem(CreateItem(ItemType.Shoes));

        GivenUseItemKeyDown(1, true);
        itemInventoryModel.UpdateCheckUseItem();

        ShouldCallUseItem(item);
    }

    [Test]
    //擁有多個道具時, 按下指定按鍵使用道具後, 道具移位, 再按下同一號碼按鍵, 使用下一個道具
    public void use_item_by_key_twice()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item0 = CreateItem(ItemType.Protection);
        IItem item1 = CreateItem(ItemType.Weapon);
        IItem item2 = CreateItem(ItemType.Shoes);

        itemInventoryModel.AddItem(item0);
        itemInventoryModel.AddItem(item1);
        itemInventoryModel.AddItem(item2);

        GivenUseItemKeyDown(1, true);

        itemInventoryModel.UpdateCheckUseItem();
        CallItemUseCompletedEvent(item1);

        ShouldCallUseItem(item1);

        GivenUseItemKeyDown(1, true);

        itemInventoryModel.UpdateCheckUseItem();
        CallItemUseCompletedEvent(item2);

        ShouldCallUseItem(item2);
    }

    [Test]
    //按下使用道具按鍵, 該位置沒有道具時, 不會使用道具
    public void use_item_by_key_when_slot_is_empty()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        itemInventoryModel.AddItem(CreateItem(ItemType.Protection));
        itemInventoryModel.AddItem(CreateItem(ItemType.Shoes));

        GivenUseItemKeyDown(2, true);
        itemInventoryModel.UpdateCheckUseItem();

        ShouldHaveItem(false, 2);
    }
    
    [Test]
    //道具使用完畢時, 呼叫移除道具
    public void remove_item_when_item_use_completed()
    {
        itemInventoryModel.SetSlotLimit(
            new Vector3(5, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(3, 5, 0),
            new Vector3(2, 5, 0));

        IItem item = CreateItem(ItemType.Protection);

        itemInventoryModel.AddItem(item);
        CallItemUseCompletedEvent(item);

        ShouldCallRemoveItem(item, 1);
        CurrentItemCountShouldBe(0);
    }

    private void GivenUseItemKeyDown(int itemSlotIndex, bool isKeyDown)
    {
        keyController.IsUseItemKeyDown(itemSlotIndex).Returns(isKeyDown);
    }

    private void CallItemUseCompletedEvent(IItem item)
    {
        item.OnItemUseCompleted += Raise.Event<Action<IItem>>(item);
    }

    private void ShouldCallRemoveItem(IItem item, int callTimes)
    {
        item.Received(callTimes).RemoveItem();
    }

    private void ShouldCallSetPos(IItem item, Vector3 pos)
    {
        item.Received(1).SetPos(pos);
    }

    private void ShouldCallUseItem(IItem item)
    {
        item.Received(1).UseItem();
    }

    private void ShouldAlreadyHaveSpecificTypeItem(bool expectedAlreadyHave, ItemType itemType)
    {
        Assert.AreEqual(expectedAlreadyHave, itemInventoryModel.AlreadyHaveSpecificTypeItem(itemType));
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
}