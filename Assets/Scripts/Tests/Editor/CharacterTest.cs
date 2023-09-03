using System;
using NSubstitute;
using NUnit.Framework;

public class CharacterTest
{
    private Action<float> horizontalMoveEvent;
    private IMoveController moveController;
    private CharacterMoveModel characterMoveModel;
    private Action<float> jumpEvent;
    private IKeyController keyController;

    [SetUp]
    public void Setup()
    {
        moveController = Substitute.For<IMoveController>();
        keyController = Substitute.For<IKeyController>();

        characterMoveModel = new CharacterMoveModel(moveController, keyController);

        horizontalMoveEvent = Substitute.For<Action<float>>();
        jumpEvent = Substitute.For<Action<float>>();
        characterMoveModel.OnHorizontalMove += horizontalMoveEvent;
        characterMoveModel.OnJump += jumpEvent;
    }

    [Test]
    //左右移動
    public void right_and_left_move()
    {
        GivenHorizontalAxis(0.5f);

        characterMoveModel.UpdateMove(1, 1);

        ShouldReceiveHorizontalMoveEvent(Arg.Is<float>(x => x >= 0));

        GivenHorizontalAxis(-0.5f);

        characterMoveModel.UpdateMove(1, 1);

        ShouldReceiveHorizontalMoveEvent(Arg.Is<float>(x => x <= 0));
    }

    [Test]
    //沒有按跳躍按鍵時, 不會跳躍
    public void jump_not_press_key()
    {
        ShouldIsJumping(false);

        GivenIsJumpKeyDown(false);
        characterMoveModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(0);
        ShouldIsJumping(false);
    }

    [Test]
    //跳躍時不能再跳
    public void jump_cannot_jump_again()
    {
        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);
        ShouldIsJumping(true);

        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);
    }
    
    [Test]
    //跳躍落地後可再跳
    public void jump_can_jump_again()
    {
        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(1);
        ShouldIsJumping(true);

        characterMoveModel.TriggerFloor();

        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateCheckJump(1);

        ShouldReceiveJumpEvent(2);
        ShouldIsJumping(true);
    }
    
    [Test]
    //跳躍後, 在跳躍延遲時間內觸發地板, 不可再跳, 過延遲時間後再次觸發地板才可跳
    public void cannot_jump_again_until_delay_time()
    {
        characterMoveModel.SetJumpDelay(0.5f);
        
        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateJumpTimer(0.3f);
        characterMoveModel.UpdateCheckJump(1);
        
        ShouldReceiveJumpEvent(1);
        
        characterMoveModel.TriggerFloor();
        
        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateJumpTimer(0.3f);
        characterMoveModel.UpdateCheckJump(1);
        
        ShouldReceiveJumpEvent(1);
        
        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateJumpTimer(0.3f);
        characterMoveModel.UpdateCheckJump(1);
        
        ShouldReceiveJumpEvent(1);
        
        characterMoveModel.TriggerFloor();
        
        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateJumpTimer(0.3f);
        characterMoveModel.UpdateCheckJump(1);
        
        ShouldReceiveJumpEvent(2);
    }
    
    [Test]
    //跳躍後, 在跳躍延遲時間過後觸發地板, 可再跳
    public void can_jump_again_after_delay_time()
    {
        characterMoveModel.SetJumpDelay(0.5f);
        
        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateJumpTimer(0.3f);
        characterMoveModel.UpdateCheckJump(1);
        
        ShouldReceiveJumpEvent(1);
        
        GivenIsJumpKeyDown(true);
        characterMoveModel.UpdateJumpTimer(0.5f);
        characterMoveModel.TriggerFloor();
        characterMoveModel.UpdateCheckJump(1);
        
        ShouldReceiveJumpEvent(2);
    }

    private void GivenIsJumpKeyDown(bool isKeyDown)
    {
        keyController.IsJumpKeyDown.Returns(isKeyDown);
    }

    private void GivenHorizontalAxis(float axisValue)
    {
        moveController.GetHorizontalAxis().Returns(axisValue);
    }

    private void ShouldReceiveJumpEvent(int triggerTimes)
    {
        jumpEvent.Received(triggerTimes).Invoke(Arg.Any<float>());
    }

    private void ShouldIsJumping(bool expectedIsJumping)
    {
        Assert.AreEqual(expectedIsJumping, characterMoveModel.IsJumping);
    }

    private void ShouldReceiveHorizontalMoveEvent(float expectedLogic)
    {
        horizontalMoveEvent.Received(1).Invoke(expectedLogic);
    }

}