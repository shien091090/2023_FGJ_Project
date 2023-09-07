using System;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

public class CharacterTest
{
    private Action<float> horizontalMoveEvent;
    private IMoveController moveController;
    private CharacterModel characterModel;
    private Action<float> jumpEvent;
    private IKeyController keyController;
    private ITeleport teleport;
    private IRigidbody characterRigidbody;

    private static void GivenTeleportGatePos(ITeleportGate teleportGate, Vector3 pos)
    {
        teleportGate.GetPos.Returns(pos);
    }

    [SetUp]
    public void Setup()
    {
        moveController = Substitute.For<IMoveController>();
        keyController = Substitute.For<IKeyController>();
        teleport = Substitute.For<ITeleport>();
        characterRigidbody = Substitute.For<IRigidbody>();

        characterModel = new CharacterModel(moveController, keyController, teleport, characterRigidbody);

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

    // [Test]
    // //墜落一定時間後, 傳送回原點
    // public void fall_down_and_teleport()
    // {
    //     characterModel.SetFallDownTime(1f);
    //
    //     characterModel.ExitFloor();
    //     characterModel.UpdateFallDownTimer(0.5f);
    //
    //     ShouldCallBackToOrigin(0);
    //
    //     characterModel.UpdateFallDownTimer(0.5f);
    //
    //     ShouldCallBackToOrigin(1);
    // }

    // [Test]
    // //墜落一段時間後落地, 不會傳送回原點
    // public void fall_down_and_trigger_floor()
    // {
    //     characterModel.SetFallDownTime(1f);
    //
    //     characterModel.ExitFloor();
    //     characterModel.UpdateFallDownTimer(0.5f);
    //
    //     ShouldCallBackToOrigin(0);
    //
    //     characterModel.TriggerFloor();
    //     characterModel.UpdateFallDownTimer(0.5f);
    //
    //     ShouldCallBackToOrigin(0);
    // }

    // [Test]
    // //墜落一段時間後落地, 再墜落時會重算回原點時間
    // public void fall_down_and_trigger_floor_and_fall_down_again()
    // {
    //     characterModel.SetFallDownTime(1f);
    //
    //     characterModel.ExitFloor();
    //     characterModel.UpdateFallDownTimer(0.5f);
    //
    //     ShouldCallBackToOrigin(0);
    //
    //     characterModel.TriggerFloor();
    //     characterModel.UpdateFallDownTimer(0.5f);
    //
    //     ShouldCallBackToOrigin(0);
    //
    //     characterModel.ExitFloor();
    //     characterModel.UpdateFallDownTimer(0.5f);
    //
    //     ShouldCallBackToOrigin(0);
    //
    //     characterModel.UpdateFallDownTimer(0.5f);
    //
    //     ShouldCallBackToOrigin(1);
    // }

    // [Test]
    // //墜落傳送回原點後, 不會繼續觸發回原點
    // public void fall_down_and_teleport_and_not_trigger_again()
    // {
    //     characterModel.SetFallDownTime(1f);
    //
    //     characterModel.ExitFloor();
    //     characterModel.UpdateFallDownTimer(1f);
    //
    //     ShouldCallBackToOrigin(1);
    //
    //     characterModel.UpdateFallDownTimer(1f);
    //
    //     ShouldCallBackToOrigin(1);
    // }

    [Test]
    //接觸傳送門時, 點擊按鍵後傳送
    public void teleport_when_touch_teleport()
    {
        GivenInteractKeyDown(true);

        ITeleportGate teleportGate = Substitute.For<ITeleportGate>();
        characterModel.TriggerTeleportGate(teleportGate);
        characterModel.UpdateCheckInteract();

        ShouldCallTeleport(teleportGate, 1);
    }

    [Test]
    //接觸傳送門時, 點擊按鍵但超過距離, 不傳送
    public void not_teleport_when_touch_teleport_but_over_distance()
    {
        GivenInteractKeyDown(true);

        GivenCharacterPosition(new Vector3(1, 0, 0));
        characterModel.SetInteractDistance(2f);

        ITeleportGate teleportGate = Substitute.For<ITeleportGate>();
        GivenTeleportGatePos(teleportGate, new Vector3(5, 0, 0));

        characterModel.TriggerTeleportGate(teleportGate);
        characterModel.UpdateCheckInteract();

        ShouldCallTeleport(teleportGate, 0);
        ShouldHaveTriggerTeleportGate(false);
    }

    [Test]
    //沒有接觸傳送門時, 點擊按鍵不會觸發傳送
    public void not_teleport_when_not_touch_teleport()
    {
        GivenInteractKeyDown(true);

        characterModel.UpdateCheckInteract();

        ShouldHaveTriggerTeleportGate(false);
    }

    private void GivenCharacterPosition(Vector3 pos)
    {
        characterRigidbody.position.Returns(pos);
    }

    private void GivenInteractKeyDown(bool isKeyDown)
    {
        keyController.IsInteractKeyDown.Returns(isKeyDown);
    }

    private void GivenIsJumpKeyDown(bool isKeyDown)
    {
        keyController.IsJumpKeyDown.Returns(isKeyDown);
    }

    private void GivenHorizontalAxis(float axisValue)
    {
        moveController.GetHorizontalAxis().Returns(axisValue);
    }

    private void ShouldHaveTriggerTeleportGate(bool expectedHave)
    {
        Assert.AreEqual(expectedHave, characterModel.HaveInteractGate);
    }

    private void ShouldCallTeleport(ITeleportGate teleportGate, int callTimes)
    {
        teleportGate.Received(callTimes).Teleport(Arg.Any<IRigidbody>());
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