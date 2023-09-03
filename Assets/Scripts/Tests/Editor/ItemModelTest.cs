using System;
using NSubstitute;
using NUnit.Framework;

public class ItemModelTest
{
    private Action itemUseCompleteEvent;
    private Action<int> refreshCurrentUseTimesEvent;
    private Action<float> refreshCurrentTimerEvent;

    [Test]
    //使用次數限制型道具, 僅限制1次, 使用完即消失
    public void use_item_once()
    {
        ItemModel itemModel = CreateModel(ItemUseType.UseTimes, 1);

        itemModel.UseItem();

        ShouldReceiveItemUseCompleteEvent(1);
        ShouldReceiveRefreshUseTimesEvent(1, 0);
    }

    [Test]
    //使用次數限制型道具, 限制多次, 使用完即消失
    public void use_item_multiple_times()
    {
        ItemModel itemModel = CreateModel(ItemUseType.UseTimes, 2);

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
        ItemModel itemModel = CreateModel(ItemUseType.PassTime, 3);

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

    private ItemModel CreateModel(ItemUseType itemUseType, int useLimit)
    {
        ItemModel itemModel = new ItemModel(itemUseType, useLimit);

        itemUseCompleteEvent = Substitute.For<Action>();
        itemModel.OnItemUseComplete += itemUseCompleteEvent;

        refreshCurrentUseTimesEvent = Substitute.For<Action<int>>();
        itemModel.OnRefreshCurrentUseTimes += refreshCurrentUseTimesEvent;
        
        refreshCurrentTimerEvent = Substitute.For<Action<float>>();
        itemModel.OnRefreshCurrentTimer += refreshCurrentTimerEvent;

        return itemModel;
    }
    //使用秒數限制型道具, 等待過程中再次使用沒反應
}