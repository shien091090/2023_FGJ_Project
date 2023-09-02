using System;
using NSubstitute;
using NUnit.Framework;

public class CharacterMoveTest
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
        characterMoveModel.CheckJump(1);

        ShouldReceiveJumpEvent(0);
        ShouldIsJumping(false);
    }

    [Test]
    //跳躍時不能再跳
    public void jump_cannot_jump_again()
    {
        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterMoveModel.CheckJump(1);

        ShouldReceiveJumpEvent(1);
        ShouldIsJumping(true);

        GivenIsJumpKeyDown(true);
        characterMoveModel.CheckJump(1);

        ShouldReceiveJumpEvent(1);

        characterMoveModel.TriggerFloor();

        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterMoveModel.CheckJump(1);

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

    //跳躍落地後可再跳
}