using System;
using NSubstitute;
using NUnit.Framework;

public class CharacterTest
{
    private Action<float> horizontalMoveEvent;
    private IMoveController moveController;
    private CharacterModel characterModel;
    private Action<float> jumpEvent;
    private IKeyController keyController;
    private ITeleport teleport;

    [SetUp]
    public void Setup()
    {
        moveController = Substitute.For<IMoveController>();
        keyController = Substitute.For<IKeyController>();
        teleport = Substitute.For<ITeleport>();

        characterModel = new CharacterModel(moveController, keyController, teleport);

        horizontalMoveEvent = Substitute.For<Action<float>>();
        jumpEvent = Substitute.For<Action<float>>();
        characterModel.OnHorizontalMove += horizontalMoveEvent;
        characterModel.OnJump += jumpEvent;
    }

    [Test]
    //左右移動
    public void right_and_left_move()
    {
        GivenHorizontalAxis(0.5f);

        characterModel.UpdateMove(1, 1);

        ShouldReceiveHorizontalMoveEvent(Arg.Is<float>(x => x >= 0));

        GivenHorizontalAxis(-0.5f);

        characterModel.UpdateMove(1, 1);

        ShouldReceiveHorizontalMoveEvent(Arg.Is<float>(x => x <= 0));
    }

    [Test]
    //沒有按跳躍按鍵時, 不會跳躍
    public void jump_not_press_key()
    {
        ShouldIsJumping(false);

        GivenIsJumpKeyDown(false);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(0);
        ShouldIsJumping(false);
    }

    [Test]
    //跳躍時不能再跳
    public void jump_cannot_jump_again()
    {
        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);
        ShouldIsJumping(true);

        GivenIsJumpKeyDown(true);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);
    }

    [Test]
    //跳躍落地後可再跳
    public void jump_can_jump_again()
    {
        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);
        ShouldIsJumping(true);

        characterModel.TriggerFloor();

        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(2);
        ShouldIsJumping(true);
    }

    [Test]
    //跳躍後, 在跳躍延遲時間內觸發地板, 不可再跳, 過延遲時間後再次觸發地板才可跳
    public void cannot_jump_again_until_delay_time()
    {
        characterModel.SetJumpDelay(0.5f);

        GivenIsJumpKeyDown(true);
        characterModel.UpdateJumpTimer(0.3f);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);

        characterModel.TriggerFloor();

        GivenIsJumpKeyDown(true);
        characterModel.UpdateJumpTimer(0.3f);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);

        GivenIsJumpKeyDown(true);
        characterModel.UpdateJumpTimer(0.3f);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);

        characterModel.TriggerFloor();

        GivenIsJumpKeyDown(true);
        characterModel.UpdateJumpTimer(0.3f);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(2);
    }

    [Test]
    //跳躍後, 在跳躍延遲時間過後觸發地板, 可再跳
    public void can_jump_again_after_delay_time()
    {
        characterModel.SetJumpDelay(0.5f);

        GivenIsJumpKeyDown(true);
        characterModel.UpdateJumpTimer(0.3f);
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);

        GivenIsJumpKeyDown(true);
        characterModel.UpdateJumpTimer(0.5f);
        characterModel.TriggerFloor();
        characterModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(2);
    }

    [Test]
    //墜落一定時間後, 傳送回原點
    public void fall_down_and_teleport()
    {
        characterModel.SetFallDownTime(1f);

        characterModel.ExitFloor();
        characterModel.UpdateFallDownTimer(0.5f);

        ShouldCallBackToOrigin(0);

        characterModel.UpdateFallDownTimer(0.5f);

        ShouldCallBackToOrigin(1);
    }

    [Test]
    //墜落一段時間後落地, 不會傳送回原點
    public void fall_down_and_trigger_floor()
    {
        characterModel.SetFallDownTime(1f);

        characterModel.ExitFloor();
        characterModel.UpdateFallDownTimer(0.5f);

        ShouldCallBackToOrigin(0);

        characterModel.TriggerFloor();
        characterModel.UpdateFallDownTimer(0.5f);

        ShouldCallBackToOrigin(0);
    }
    
    //墜落一段時間後落地, 再墜落時會重算回原點時間
    //墜落傳送回原點後, 不會繼續觸發回原點

    private void GivenIsJumpKeyDown(bool isKeyDown)
    {
        keyController.IsJumpKeyDown.Returns(isKeyDown);
    }

    private void GivenHorizontalAxis(float axisValue)
    {
        moveController.GetHorizontalAxis().Returns(axisValue);
    }

    private void ShouldCallBackToOrigin(int callTimes)
    {
        teleport.Received(callTimes).BackToOrigin();
    }

    private void ShouldReceiveJumpEvent(int triggerTimes)
    {
        jumpEvent.Received(triggerTimes).Invoke(Arg.Any<float>());
    }

    private void ShouldIsJumping(bool expectedIsJumping)
    {
        Assert.AreEqual(expectedIsJumping, characterModel.IsJumping);
    }

    private void ShouldReceiveHorizontalMoveEvent(float expectedLogic)
    {
        horizontalMoveEvent.Received(1).Invoke(expectedLogic);
    }
}