using System;
using NSubstitute;
using NUnit.Framework;

public class CharacterMoveTest
{
    private Action<float> horizontalMoveEvent;
    private IMoveController moveController;
    private CharacterMoveModel characterMoveModel;

    [SetUp]
    public void Setup()
    {
        moveController = Substitute.For<IMoveController>();
        characterMoveModel = new CharacterMoveModel(moveController);

        horizontalMoveEvent = Substitute.For<Action<float>>();
        characterMoveModel.OnHorizontalMove += horizontalMoveEvent;
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

    private void GivenHorizontalAxis(float axisValue)
    {
        moveController.GetHorizontalAxis().Returns(axisValue);
    }

    private void ShouldReceiveHorizontalMoveEvent(float expectedLogic)
    {
        horizontalMoveEvent.Received(1).Invoke(expectedLogic);
    }

    //跳躍時不能再跳
    //跳躍落地後可再跳
}