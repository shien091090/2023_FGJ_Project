using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

public class TimerModelTest
{
    private TimerModel timerModel;
    private Action<TimerUpdateEventInfo> timerUpdateEvent;

    [SetUp]
    public void Setup()
    {
        timerModel = new TimerModel();

        timerUpdateEvent = Substitute.For<Action<TimerUpdateEventInfo>>();
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

        ShouldNotTiggerUpdateEvent();

        timerModel.UpdateTimer(0.3f); //1.2
        timerModel.UpdateTimer(0.3f); //1.5
        timerModel.UpdateTimer(0.3f); //1.8

        Assert.AreEqual(1, GetLastTimerUpdateEvent().CurrentTime);

        timerModel.UpdateTimer(0.3f); //2.1

        Assert.AreEqual(2, GetLastTimerUpdateEvent().CurrentTime);
    }

    [Test]
    //驗證倒數計時器格式(時:分:秒)
    public void timer_format_is_minute_second()
    {
        timerModel.StartTimer();
        timerModel.UpdateTimer(3650);

        Assert.AreEqual("01:00:50", GetLastTimerUpdateEvent().GetTimerString(TimerStringFormatType.HHMMSS));
    }

    private void ShouldNotTiggerUpdateEvent()
    {
        timerUpdateEvent.DidNotReceive().Invoke(Arg.Any<TimerUpdateEventInfo>());
    }

    private void CurrentTimeShouldBe(int expectedCurrentTime)
    {
        Assert.AreEqual(expectedCurrentTime, timerModel.CurrentTime);
    }

    private TimerUpdateEventInfo GetLastTimerUpdateEvent()
    {
        return (TimerUpdateEventInfo)timerUpdateEvent.ReceivedCalls().Last().GetArguments()[0];
    }

    //驗證倒數計時器格式(分:秒)
    //驗證倒數計時器格式(秒)
    //開始倒數計時後, 暫停計時器, 之後不會觸發更新事件且時間維持不變
    //開始倒數計時後, 重置計時器, 時間歸零且之後不會觸發更新事件
    //開始倒數計時後, 暫停計時器, 再重置計時器, 時間歸零且之後不會觸發更新事件
    //尚未倒數計時, 暫停計時器, 不做事
    //尚未倒數計時, 重置計時器, 不做事
}