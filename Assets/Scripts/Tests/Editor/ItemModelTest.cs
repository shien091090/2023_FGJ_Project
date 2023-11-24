using System;
using NSubstitute;
using NUnit.Framework;

public class ItemModelTest
{
    private ICharacterModel characterModel;

    private Action itemUseCompleteEvent;
    private Action<int> refreshCurrentUseTimesEvent;
    private Action<float> refreshCurrentTimerEvent;
    private Action<ItemType> useItemEvent;
    private Action<ItemType> startItemEffectEvent;
    private Action<ItemType> endItemEffectEvent;

    [SetUp]
    public void Setup()
    {
        characterModel = Substitute.For<ICharacterModel>();
    }

    [Test]
    //使用次數限制型道具, 僅限制1次, 使用完即消失
    public void use_item_once()
    {
        ItemModel itemModel = CreateModel(ItemType.Shoes, ItemUseType.UseTimes, 1);

        itemModel.UseItem();

        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshUseTimesEvent(1, 0);
    }

    [Test]
    //使用次數限制型道具, 限制多次, 使用完即消失
    public void use_item_multiple_times()
    {
        ItemModel itemModel = CreateModel(ItemType.Shoes, ItemUseType.UseTimes, 2);

        itemModel.UseItem();

        ShouldReceiveItemUseCompleteEvent(0);
        ShouldReceiveRefreshUseTimesEvent(1, 1);

        itemModel.UseItem();

        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshUseTimesEvent(1, 0);
    }

    [Test]
    //使用秒數限制型道具, 使用後過指定時間消失
    public void use_item_pass_time()
    {
        ItemModel itemModel = CreateModel(ItemType.Protection, ItemUseType.PassTime, 3);

        itemModel.UseItem();

        itemModel.UpdateTimer(1);
        ShouldReceiveItemUseCompleteEvent(0);
        ShouldReceiveRefreshTimerEvent(1, 2);

        itemModel.UpdateTimer(1);
        ShouldReceiveItemUseCompleteEvent(0);
        ShouldReceiveRefreshTimerEvent(1, 1);

        itemModel.UpdateTimer(2);
        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshTimerEvent(1, 0);

        itemModel.UpdateTimer(1);
        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshTimerEvent(1, 0);
    }

    [Test]
    //使用秒數限制型道具, 等待過程中再次使用沒反應
    public void use_item_pass_time_twice()
    {
        ItemModel itemModel = CreateModel(ItemType.Protection, ItemUseType.PassTime, 3);

        itemModel.UseItem();
        itemModel.UpdateTimer(1);

        ShouldReceiveItemUseCompleteEvent(0);
        ShouldReceiveRefreshTimerEvent(1, 2);

        itemModel.UseItem();
        itemModel.UpdateTimer(1);

        ShouldReceiveItemUseCompleteEvent(0);
        ShouldReceiveRefreshTimerEvent(1, 1);
    }

    [Test]
    //秒數型道具使用完畢後, 再次刷新時間也不會再繼續發出事件
    public void when_pass_time_item_is_used_should_not_send_event_after_refresh_timer()
    {
        ItemModel itemModel = CreateModel(ItemType.Protection, ItemUseType.PassTime, 1);

        itemModel.UseItem();

        itemModel.UpdateTimer(1);
        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshTimerEvent(1, 0);

        itemModel.UpdateTimer(1);
        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshTimerEvent(1, 0);
    }

    [Test]
    //次數型道具使用完畢後, 再次使用也不會再繼續發出事件
    public void when_use_times_item_is_used_should_not_send_event_after_use_again()
    {
        ItemModel itemModel = CreateModel(ItemType.Weapon, ItemUseType.UseTimes, 1);

        itemModel.UseItem();

        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshUseTimesEvent(1, 0);

        itemModel.UseItem();

        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshUseTimesEvent(1, 0);
    }

    [Test]
    //使用秒數型道具, 按下使用前不會計時
    public void use_item_pass_time_before_use()
    {
        ItemModel itemModel = CreateModel(ItemType.Protection, ItemUseType.PassTime, 1);

        itemModel.UpdateTimer(1);

        ShouldReceiveItemUseCompleteEvent(0);
        ShouldNotReceiveAnyRefreshTimerEvent();
    }

    [Test]
    //秒數型道具使用完畢, 重置資料為次數型道具
    public void pass_time_item_is_used_then_reset_data_to_use_times_item()
    {
        ItemModel itemModel = CreateModel(ItemType.Protection, ItemUseType.PassTime, 1);

        itemModel.UseItem();
        itemModel.UpdateTimer(1);

        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshTimerEvent(1, 0);

        itemModel.SetUseTimesType(2);
        itemModel.UseItem();
        itemModel.UpdateTimer(1);

        ShouldReceiveRefreshUseTimesEvent(1, 1);
        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshTimerEvent(1, 0);
    }

    [Test]
    //次數型道具使用完畢, 重置資料為秒數型道具
    public void use_times_item_is_used_then_reset_data_to_pass_time_item()
    {
        ItemModel itemModel = CreateModel(ItemType.Weapon, ItemUseType.UseTimes, 1);

        itemModel.UseItem();

        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshUseTimesEvent(1, 0);

        itemModel.SetPassTimeType(2);
        itemModel.UseItem();
        itemModel.UpdateTimer(1);

        ShouldReceiveRefreshTimerEvent(1, 1);
        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshUseTimesEvent(1, 0);
    }

    [Test]
    //使用次數型道具, 每使用一次會發出使用事件
    public void use_times_item_should_send_event_when_use()
    {
        ItemModel itemModel = CreateModel(ItemType.Shoes, ItemUseType.UseTimes, 2);

        itemModel.UseItem();

        ShouldReceiveUseItemEvent(1, ItemType.Shoes);

        itemModel.UseItem();

        ShouldReceiveUseItemEvent(2, ItemType.Shoes);

        itemModel.UseItem();

        ShouldReceiveUseItemEvent(2, ItemType.Shoes);
    }

    [Test]
    //使用秒數型道具, 使用時跟時間結束會發事件, 過程中再次按使用不會再發事件
    public void use_pass_time_item_should_send_event_when_use_and_time_up()
    {
        ItemModel itemModel = CreateModel(ItemType.Protection, ItemUseType.PassTime, 3);

        itemModel.UseItem();
        itemModel.UpdateTimer(1);

        ShouldReceiveStartItemEffectEvent(1, ItemType.Protection);
        ShouldReceivedEndItemEffectEvent(0, ItemType.Protection);

        itemModel.UseItem();
        itemModel.UpdateTimer(1);

        ShouldReceiveStartItemEffectEvent(1, ItemType.Protection);
        ShouldReceivedEndItemEffectEvent(0, ItemType.Protection);

        itemModel.UpdateTimer(1);

        ShouldReceiveStartItemEffectEvent(1, ItemType.Protection);
        ShouldReceivedEndItemEffectEvent(1, ItemType.Protection);
    }

    private void ShouldReceivedEndItemEffectEvent(int triggerTimes, ItemType expectedItemType)
    {
        if (triggerTimes == 0)
            endItemEffectEvent.DidNotReceive().Invoke(expectedItemType);
        else
            endItemEffectEvent.Received(triggerTimes).Invoke(expectedItemType);
    }

    private void ShouldReceiveStartItemEffectEvent(int triggerTimes, ItemType expectedItemType)
    {
        startItemEffectEvent.Received(triggerTimes).Invoke(expectedItemType);
    }

    private void ShouldReceiveUseItemEvent(int triggerTimes, ItemType itemType)
    {
        useItemEvent.Received(triggerTimes).Invoke(itemType);
    }

    private void ShouldNotReceiveAnyRefreshTimerEvent()
    {
        refreshCurrentTimerEvent.DidNotReceive().Invoke(Arg.Any<float>());
    }

    private void ShouldReceiveRefreshTimerEvent(int triggerTimes, float expectedTimerValue)
    {
        refreshCurrentTimerEvent.Received(triggerTimes).Invoke(expectedTimerValue);
    }

    private void ShouldReceiveRefreshUseTimesEvent(int triggerTimes, int expectedCurrentUseTimes)
    {
        refreshCurrentUseTimesEvent.Received(triggerTimes).Invoke(expectedCurrentUseTimes);
    }

    private void ShouldReceiveItemUseCompleteEvent(int triggerTimes)
    {
        itemUseCompleteEvent.Received(triggerTimes).Invoke();
    }

    private ItemModel CreateModel(ItemType itemType, ItemUseType itemUseType, int useLimit)
    {
        // ItemModel itemModel = new ItemModel(itemType, characterModel, );
        // if (itemUseType == ItemUseType.UseTimes)
        //     itemModel.SetUseTimesType(useLimit);
        // else
        //     itemModel.SetPassTimeType(useLimit);
        //
        // itemUseCompleteEvent = Substitute.For<Action>();
        // itemModel.OnItemUseComplete += itemUseCompleteEvent;
        //
        // refreshCurrentUseTimesEvent = Substitute.For<Action<int>>();
        // itemModel.OnRefreshCurrentUseTimes += refreshCurrentUseTimesEvent;
        //
        // refreshCurrentTimerEvent = Substitute.For<Action<float>>();
        // itemModel.OnRefreshCurrentTimer += refreshCurrentTimerEvent;
        //
        // useItemEvent = Substitute.For<Action<ItemType>>();
        // itemModel.OnUseItemOneTime += useItemEvent;
        //
        // startItemEffectEvent = Substitute.For<Action<ItemType>>();
        // itemModel.OnStartItemEffect += startItemEffectEvent;
        //
        // endItemEffectEvent = Substitute.For<Action<ItemType>>();
        // itemModel.OnEndItemEffect += endItemEffectEvent;

        return null;
    }
}