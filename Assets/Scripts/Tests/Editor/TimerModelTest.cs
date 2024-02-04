using System;
using NSubstitute;
using NUnit.Framework;

public class TimerModelTest
{
    private TimerModel timerModel;
    private Action timerUpdateEvent;

    [SetUp]
    public void Setup()
    {
        timerModel = new TimerModel();

        timerUpdateEvent = Substitute.For<Action>();
        timerModel.OnUpdateTimer += timerUpdateEvent;
    }

    [Test]
    //尚未倒數計時, 當前時間為0
    public void current_time_is_0_when_timer_is_not_started()
    {
        CurrentTimeShouldBe(0);
    }

    [Test]
    //開始倒數計時, 每秒觸發一次更新事件
    public void start_timer_trigger_update_event_every_second()
    {
        timerModel.StartTimer();
        timerModel.UpdateTimer(0.3f); //0.3
        timerModel.UpdateTimer(0.3f); //0.6
        timerModel.UpdateTimer(0.3f); //0.9

        ShouldTiggerUpdateEvent(0);
        
        timerModel.UpdateTimer(0.3f); //1.2
        timerModel.UpdateTimer(0.3f); //1.5
        timerModel.UpdateTimer(0.3f); //1.8
        
        ShouldTiggerUpdateEvent(1);
        
        timerModel.UpdateTimer(0.3f); //2.1
        
        ShouldTiggerUpdateEvent(2);
    }

    private void ShouldTiggerUpdateEvent(int expectedTriggerTimes)
    {
        if (expectedTriggerTimes == 0)
            timerUpdateEvent.DidNotReceive().Invoke();
        else
            timerUpdateEvent.Received(expectedTriggerTimes).Invoke();
    }

    private void CurrentTimeShouldBe(int expectedCurrentTime)
    {
        Assert.AreEqual(expectedCurrentTime, timerModel.CurrentTime);
    }

    //驗證倒數計時器格式(時:分:秒)
    //驗證倒數計時器格式(分:秒)
    //驗證倒數計時器格式(秒)
    //開始倒數計時後, 暫停計時器, 之後不會觸發更新事件且時間維持不變
    //開始倒數計時後, 重置計時器, 時間歸零且之後不會觸發更新事件
    //開始倒數計時後, 暫停計時器, 再重置計時器, 時間歸零且之後不會觸發更新事件
    //尚未倒數計時, 暫停計時器, 不做事
    //尚未倒數計時, 重置計時器, 不做事
}