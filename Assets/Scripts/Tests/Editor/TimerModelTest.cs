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
    [TestCase(45, "00:00:45")]
    [TestCase(505, "00:08:25")]
    [TestCase(3650, "01:00:50")]
    [TestCase(89500, "24:51:40")]
    [TestCase(359999, "99:59:59")]
    [TestCase(360000, "99:59:59+")]
    //驗證倒數計時器格式(時:分:秒)
    public void verify_timer_format_HHMMSS(int deltaTime, string expectedTimerString)
    {
        timerModel.StartTimer();
        timerModel.UpdateTimer(deltaTime);

        Assert.AreEqual(expectedTimerString, GetLastTimerUpdateEvent().GetTimerString(TimerStringFormatType.HHMMSS));
    }

    [Test]
    [TestCase(45, "00:45")]
    [TestCase(505, "08:25")]
    [TestCase(5999, "99:59")]
    [TestCase(6000, "99:59+")]
    //驗證倒數計時器格式(分:秒)
    public void verify_timer_format_MMSS(int deltaTime, string expectedTimerString)
    {
        timerModel.StartTimer();
        timerModel.UpdateTimer(deltaTime);

        Assert.AreEqual(expectedTimerString, GetLastTimerUpdateEvent().GetTimerString(TimerStringFormatType.MMSS));
    }

    [Test]
    [TestCase(45, "45")]
    [TestCase(99, "99")]
    [TestCase(100, "99+")]
    //驗證倒數計時器格式(秒)
    public void verify_timer_format_SS(int deltaTime, string expectedTimerString)
    {
        timerModel.StartTimer();
        timerModel.UpdateTimer(deltaTime);

        Assert.AreEqual(expectedTimerString, GetLastTimerUpdateEvent().GetTimerString(TimerStringFormatType.SS));
    }

    [Test]
    //開始倒數計時後, 暫停計時器, 之後不會觸發更新事件且時間維持不變
    public void pause_timer_should_not_trigger_update_event_and_time_should_not_change()
    {
        timerModel.StartTimer();
        timerModel.UpdateTimer(1);

        CurrentTimeShouldBe(1);
        ShouldTriggerAnyTimerUpdateEvent(1);
        CurrentTimerStateShouldBe(TimerState.Running);

        timerModel.SetPause(true);
        timerModel.UpdateTimer(1);
        timerModel.UpdateTimer(1);
        timerModel.UpdateTimer(1);

        CurrentTimeShouldBe(1);
        ShouldTriggerAnyTimerUpdateEvent(1);
        CurrentTimerStateShouldBe(TimerState.Paused);
    }

    private void CurrentTimerStateShouldBe(TimerState expectedTimerState)
    {
        Assert.AreEqual(expectedTimerState, timerModel.CurrentTimerState);
    }

    //開始倒數計時後, 重置計時器, 時間歸零且之後不會觸發更新事件
    //開始倒數計時後, 暫停計時器, 再重置計時器, 時間歸零且之後不會觸發更新事件
    //尚未倒數計時, 暫停計時器, 不做事
    //尚未倒數計時, 重置計時器, 不做事

    private void ShouldNotTiggerUpdateEvent()
    {
        timerUpdateEvent.DidNotReceive().Invoke(Arg.Any<TimerUpdateEventInfo>());
    }

    private void ShouldTriggerAnyTimerUpdateEvent(int expectedTriggerTimes)
    {
        timerUpdateEvent.Received(expectedTriggerTimes).Invoke(Arg.Any<TimerUpdateEventInfo>());
    }

    private void CurrentTimeShouldBe(int expectedCurrentTime)
    {
        Assert.AreEqual(expectedCurrentTime, timerModel.CurrentTime);
    }

    private TimerUpdateEventInfo GetLastTimerUpdateEvent()
    {
        return (TimerUpdateEventInfo)timerUpdateEvent.ReceivedCalls().Last().GetArguments()[0];
    }
}