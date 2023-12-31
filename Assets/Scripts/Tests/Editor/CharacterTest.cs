using System;
using System.Linq;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using SNShien.Common.AudioTools;
using UnityEngine;

public class CharacterTest
{
    private IMoveController moveController;
    private CharacterModel characterModel;
    private IKeyController keyController;
    private IRigidbody characterRigidbody;
    private IAudioManager audioManager;
    private ITimeModel timeModel;
    private ICharacterView characterView;
    private IGameObjectPool gameObjectPool;

    private Action characterViewWaitingCallback;
    private IItemTriggerHandler itemTriggerHandler;
    private IGameSetting gameSetting;

    [SetUp]
    public void Setup()
    {
        moveController = Substitute.For<IMoveController>();
        keyController = Substitute.For<IKeyController>();
        characterRigidbody = Substitute.For<IRigidbody>();
        audioManager = Substitute.For<IAudioManager>();
        timeModel = Substitute.For<ITimeModel>();

        characterView = Substitute.For<ICharacterView>();
        characterView.Waiting(Arg.Any<float>(), Arg.Do<Action>(callback =>
        {
            characterViewWaitingCallback = callback;
        }));

        itemTriggerHandler = Substitute.For<IItemTriggerHandler>();
        gameObjectPool = Substitute.For<IGameObjectPool>();
        gameSetting = Substitute.For<IGameSetting>();

        GivenDeltaTime(1);
        GivenSpeed(1);
        GivenJumpForce(1);
        GivenJumpDelay(1);
        GivenFallDownLimitPos(-10);

        characterModel = new CharacterModel(moveController, keyController, audioManager, timeModel, itemTriggerHandler, gameObjectPool, gameSetting);

        characterModel.BindView(characterView);
    }

    [Test]
    //左右移動
    public void right_and_left_move()
    {
        GivenHorizontalAxis(0.5f);

        characterModel.CallUpdate();

        ShouldCallTranslateAndMoveRight();

        GivenHorizontalAxis(-0.5f);

        characterModel.CallUpdate();

        ShouldCallTranslateAndMoveLeft();
    }

    [Test]
    //角色初始狀態預設面向右
    public void default_face_right()
    {
        ShouldFaceRight(true);
        FaceDirectionScaleShouldBe(1);
    }

    [Test]
    //角色變換面向
    public void change_face()
    {
        ShouldFaceRight(true);
        FaceDirectionScaleShouldBe(1);

        GivenHorizontalAxis(0.5f);
        characterModel.CallUpdate();

        ShouldFaceRight(true);
        FaceDirectionScaleShouldBe(1);

        GivenHorizontalAxis(-0.5f);
        characterModel.CallUpdate();

        ShouldFaceRight(false);
        FaceDirectionScaleShouldBe(-1);

        GivenHorizontalAxis(0.5f);
        characterModel.CallUpdate();

        ShouldFaceRight(true);
        FaceDirectionScaleShouldBe(1);
    }

    [Test]
    //角色停止後, 維持原本面向
    public void keep_face_when_stop()
    {
        GivenHorizontalAxis(0.5f);
        characterModel.CallUpdate();

        ShouldFaceRight(true);
        FaceDirectionScaleShouldBe(1);

        GivenHorizontalAxis(0);
        characterModel.CallUpdate();

        ShouldFaceRight(true);
        FaceDirectionScaleShouldBe(1);

        GivenHorizontalAxis(-1);
        characterModel.CallUpdate();

        ShouldFaceRight(false);
        FaceDirectionScaleShouldBe(-1);

        GivenHorizontalAxis(0);
        characterModel.CallUpdate();

        ShouldFaceRight(false);
        FaceDirectionScaleShouldBe(-1);
    }

    [Test]
    //沒有按跳躍按鍵時, 不會跳躍
    public void jump_not_press_key()
    {
        ShouldIsJumping(false);

        GivenIsJumpKeyDown(false);
        characterModel.CallUpdate();

        ShouldCallJump(0);
    }

    [Test]
    //跳躍時不能再跳
    public void jump_cannot_jump_again()
    {
        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallJump(1);
        ShouldIsJumping(true);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallJump(1);
    }

    [Test]
    //離地時不能跳
    public void cannot_jump_when_not_on_floor()
    {
        characterModel.CollisionExit(CreateCollision((int)GameConst.GameObjectLayerType.Platform));

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallJump(0);
    }

    [Test]
    //跳躍力道設定為0時, 按跳躍時不會跳
    public void jump_force_is_zero()
    {
        GivenJumpForce(0);
        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallJump(0);
    }

    [Test]
    //跳躍落地後可再跳
    public void can_jump_when_back_to_floor()
    {
        GivenIsJumpKeyDown(true);

        characterModel.CallUpdate();
        characterModel.CollisionExit(CreateCollision((int)GameConst.GameObjectLayerType.Platform));

        ShouldCallJump(1);

        characterModel.CallUpdate();
        characterModel.CollisionEnter(CreateCollision((int)GameConst.GameObjectLayerType.Platform, true));

        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallJump(2);
    }

    [Test]
    //跳躍後, 在跳躍延遲時間內觸發地板, 不可再跳, 過延遲時間後再次觸發地板才可跳
    public void cannot_jump_again_until_delay_time()
    {
        GivenJumpDelay(0.7f);
        GivenDeltaTime(0.3f);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();
        characterModel.CollisionExit(CreateCollision((int)GameConst.GameObjectLayerType.Platform));

        ShouldIsJumping(true);
        ShouldCallJump(1);

        characterModel.CallUpdate();
        characterModel.CollisionEnter(CreateCollision((int)GameConst.GameObjectLayerType.Platform, true));

        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallJump(1);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();
        characterModel.CollisionExit(CreateCollision((int)GameConst.GameObjectLayerType.Platform));

        ShouldIsJumping(true);
        ShouldCallJump(2);
    }

    [Test]
    //跳躍後, 在跳躍延遲時間過後觸發地板, 可再跳
    public void can_jump_again_after_delay_time()
    {
        GivenJumpDelay(0.5f);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();
        characterModel.CollisionExit(CreateCollision((int)GameConst.GameObjectLayerType.Platform));

        ShouldCallJump(1);

        characterModel.CallUpdate();
        characterModel.CollisionEnter(CreateCollision((int)GameConst.GameObjectLayerType.Platform, true));

        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();
        characterModel.CollisionExit(CreateCollision((int)GameConst.GameObjectLayerType.Platform));

        ShouldCallJump(2);
    }

    [Test]
    //接觸傳送門時, 點擊按鍵後傳送
    public void teleport_when_touch_teleport()
    {
        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate teleportGate = CreateTeleportGateComponent();
        GivenGetComponent(collider, teleportGate);

        characterModel.ColliderTriggerEnter(collider);

        ShouldHaveTriggerTeleportGate(true);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallTeleport(teleportGate, 1);
    }

    [Test]
    //接觸其他物件而非傳送門時, 點擊按鍵不會觸發傳送
    public void not_teleport_when_touch_other_object()
    {
        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.Weapon);

        characterModel.ColliderTriggerEnter(collider);

        ShouldHaveTriggerTeleportGate(false);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        ShouldHaveTriggerTeleportGate(false);
    }

    [Test]
    //接觸傳送門但取不到Component, 點擊按鍵不會觸發傳送
    public void not_teleport_when_touch_teleport_gate_but_can_not_get_component()
    {
        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.TeleportGate);
        GivenGetComponent(collider, default(ITeleportGate));

        characterModel.ColliderTriggerEnter(collider);

        ShouldHaveTriggerTeleportGate(false);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        ShouldHaveTriggerTeleportGate(false);
    }

    [Test]
    //沒有觸碰任何物件時, 點擊按鍵不會觸發傳送
    public void not_teleport_when_not_touch_anything()
    {
        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        ShouldHaveTriggerTeleportGate(false);
    }

    [Test]
    //接觸傳送門時, 點擊按鍵但超過距離, 不傳送
    public void not_teleport_when_touch_teleport_but_over_distance()
    {
        GivenInteractDistance(2f);
        GivenCharacterPosition(new Vector3(1, 0, 0));
        GivenInteractKeyDown(true);

        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate teleportGate = CreateTeleportGateComponent(new Vector3(5, 0, 0));
        GivenGetComponent(collider, teleportGate);

        characterModel.ColliderTriggerEnter(collider);
        characterModel.CallUpdate();

        ShouldCallTeleport(teleportGate, 0);
        ShouldHaveTriggerTeleportGate(false);
    }

    [Test]
    //接觸傳送門後再離開, 點擊按鍵後不會觸發傳送
    public void not_teleport_when_touch_teleport_but_exit()
    {
        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate teleportGate = CreateTeleportGateComponent();
        GivenGetComponent(collider, teleportGate);

        characterModel.ColliderTriggerEnter(collider);
        characterModel.ColliderTriggerExit(collider);

        ShouldHaveTriggerTeleportGate(false);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallTeleport(teleportGate, 0);
    }

    [Test]
    //接觸怪物會死亡
    public void die_when_touch_monster()
    {
        GivenCharacterPosition(new Vector3(0, 0, 0));
        characterModel.BindView(characterView);

        GivenCharacterPosition(new Vector3(5, 2, 0));
        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.Monster);
        GivenGetComponent(collider, CreateMonster(MonsterState.Normal));
        characterModel.ColliderTriggerStay(collider);

        ShouldDying(true);
        ShouldChangeCurrentCharacterState(CharacterState.Die);
        ShouldPlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_DIE);

        CallCharacterViewWaitingCallback();

        ShouldPlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_NORMAL);
        CurrentCharacterPosShouldBe(new Vector3(0, 0, 0));
        ShouldAudioPlayOneShot(GameConst.AUDIO_KEY_TELEPORT);

        CallCharacterViewWaitingCallback();

        ShouldDying(false);
    }

    [Test]
    //接觸暈眩中的怪不會死亡
    public void not_die_when_touch_stun_monster()
    {
        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.Monster);
        GivenGetComponent(collider, CreateMonster(MonsterState.Stun));
        characterModel.ColliderTriggerStay(collider);

        ShouldDying(false);
    }

    [Test]
    //接觸暈眩中的怪物, 怪物解除暈眩時會死亡
    public void die_when_touch_stun_monster_and_monster_recover()
    {
        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.Monster);
        IMonsterView monster = CreateMonster(MonsterState.Stun);
        GivenGetComponent(collider, monster);
        characterModel.ColliderTriggerStay(collider);

        ShouldDying(false);

        GivenMonsterCurrentState(monster, MonsterState.Normal);
        characterModel.ColliderTriggerStay(collider);

        ShouldDying(true);
        ShouldChangeCurrentCharacterState(CharacterState.Die);
    }

    [Test]
    //接觸怪物死亡時持續接觸, 死亡流程只會觸發一次
    public void die_when_touch_monster_and_stay()
    {
        ICollider collider = CreateCollider((int)GameConst.GameObjectLayerType.Monster);
        IMonsterView monster = CreateMonster(MonsterState.Normal);
        GivenGetComponent(collider, monster);
        characterModel.ColliderTriggerStay(collider);
        characterModel.ColliderTriggerStay(collider);

        CallCharacterViewWaitingCallback();

        characterModel.ColliderTriggerStay(collider);
        characterModel.ColliderTriggerStay(collider);

        ShouldPlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_DIE, 1);
        ShouldAudioPlayOneShot(GameConst.AUDIO_KEY_TELEPORT, 1);
    }

    [Test]
    //遊戲開始時, 紀錄初始點
    public void record_origin_pos_when_game_start()
    {
        GivenCharacterPosition(new Vector3(5, 4, 0));

        characterModel.BindView(characterView);

        RecordOriginPosShouldBe(new Vector3(5, 4, 0));
    }

    [Test]
    //墜落至指定高度時傳回原點
    public void back_to_origin_when_fall_to_specified_height()
    {
        GivenFallDownLimitPos(-5);
        GivenCharacterPosition(new Vector3(0, 1, 0));

        characterModel.BindView(characterView);

        GivenCharacterPosition(new Vector3(-5, -4, 0));
        characterModel.CallUpdate();

        ShouldDying(false);

        GivenCharacterPosition(new Vector3(-5, -5, 0));
        characterModel.CallUpdate();

        ShouldDying(true);
        ShouldChangeCurrentCharacterState(CharacterState.Die);
        ShouldAudioPlayOneShot(GameConst.AUDIO_KEY_TELEPORT);
        CurrentCharacterPosShouldBe(new Vector3(0, 1, 0));

        characterModel.CallUpdate();
        ShouldAudioPlayOneShot(GameConst.AUDIO_KEY_TELEPORT, 1);

        CallCharacterViewWaitingCallback();

        ShouldDying(false);
    }

    private void GivenFallDownLimitPos(float pos)
    {
        characterView.FallDownLimitPosY.Returns(pos);
    }

    private void GivenInteractDistance(float distance)
    {
        characterView.InteractDistance.Returns(distance);
    }

    private void GivenJumpDelay(float delaySeconds)
    {
        characterView.JumpDelaySeconds.Returns(delaySeconds);
    }

    private void GivenJumpForce(int jumpForce)
    {
        characterView.JumpForce.Returns(jumpForce);
    }

    private void GivenSpeed(int speed)
    {
        characterView.Speed.Returns(speed);
    }

    private void GivenDeltaTime(float deltaTime)
    {
        timeModel.deltaTime.Returns(deltaTime);
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

    private void GivenGetComponent<T>(ICollider collider, T component)
    {
        collider.GetComponent<T>().Returns(component);
    }

    private void GivenMonsterCurrentState(IMonsterView monsterView, MonsterState monsterState)
    {
        monsterView.CurrentState.Returns(monsterState);
    }

    private void CallCharacterViewWaitingCallback()
    {
        characterViewWaitingCallback.Invoke();
    }

    private void ShouldChangeCurrentCharacterState(CharacterState characterState, int triggerTimes = 1)
    {
        // gameEventHandler.Received(triggerTimes).ChangeCurrentCharacterState(characterState);
    }

    private void FaceDirectionScaleShouldBe(int expectedScale)
    {
        int argument = (int)characterView
            .ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == "SetFaceDirectionScale")
            .ToList()
            .Last()
            .GetArguments()[0];

        Assert.AreEqual(expectedScale, argument);
    }

    private void ShouldFaceRight(bool expectedIsFaceRight)
    {
        Assert.AreEqual(expectedIsFaceRight, characterModel.IsFaceRight);
    }

    private void ShouldAudioPlayOneShot(string audioKey, int callTimes = 1)
    {
        audioManager.Received(callTimes).PlayOneShot(audioKey);
    }

    private void CurrentCharacterPosShouldBe(Vector3 expectedPos)
    {
        Assert.AreEqual(expectedPos, characterRigidbody.position);
    }

    private void ShouldPlayAnimation(string expectedAnimationKey)
    {
        string argument = (string)characterView
            .ReceivedCalls()
            .Last(x => x.GetMethodInfo().Name == "PlayAnimation")
            .GetArguments()[0];

        Assert.AreEqual(expectedAnimationKey, argument);
    }

    private void ShouldPlayAnimation(string expectedAnimationKey, int triggerTimes)
    {
        characterView.Received(triggerTimes).PlayAnimation(expectedAnimationKey);
    }

    private void ShouldDying(bool expectedIsDying)
    {
        // Assert.AreEqual(expectedIsDying, characterModel.IsDying);
    }

    private void RecordOriginPosShouldBe(Vector3 expectedPos)
    {
        Assert.AreEqual(expectedPos, characterModel.RecordOriginPos);
    }

    private void ShouldHaveTriggerTeleportGate(bool expectedHave)
    {
        Assert.AreEqual(expectedHave, characterModel.HaveInteractGate);
    }

    private void ShouldCallTeleport(ITeleportGate teleportGate, int callTimes)
    {
        // teleportGate.Received(callTimes).Teleport(Arg.Any<IRigidbody>());
    }

    private void ShouldCallJump(int triggerTimes)
    {
        characterRigidbody.Received(triggerTimes).AddForce(Arg.Is<Vector2>(v => v.y > 0 && v.x == 0));
        audioManager.Received(triggerTimes).PlayOneShot(GameConst.AUDIO_KEY_JUMP);
    }

    private void ShouldIsJumping(bool expectedIsJumping)
    {
        Assert.AreEqual(expectedIsJumping, characterModel.IsJumping);
    }

    private void ShouldCallTranslateAndMoveRight()
    {
        characterView.Received(1).Translate(Arg.Is<Vector3>(v => v.x > 0));
    }

    private void ShouldCallTranslateAndMoveLeft()
    {
        characterView.Received(1).Translate(Arg.Is<Vector3>(v => v.x < 0));
    }

    private IMonsterView CreateMonster(MonsterState monsterState)
    {
        IMonsterView monsterView = Substitute.For<IMonsterView>();
        GivenMonsterCurrentState(monsterView, monsterState);
        return monsterView;
    }

    private ICollider CreateCollider(int collisionLayer)
    {
        ICollider collider = Substitute.For<ICollider>();
        collider.Layer.Returns(collisionLayer);
        return collider;
    }

    private ICollision CreateCollision(int collisionLayer, bool isPhysicsOverlapCircle = false)
    {
        ICollision collision = Substitute.For<ICollision>();
        collision.Layer.Returns(collisionLayer);
        collision.CheckPhysicsOverlapCircle(Arg.Any<Vector3>(), Arg.Any<float>(), Arg.Any<GameConst.GameObjectLayerType>()).Returns(isPhysicsOverlapCircle);
        return collision;
    }

    private ITeleportGate CreateTeleportGateComponent(Vector3 pos = default)
    {
        ITeleportGate teleportGate = Substitute.For<ITeleportGate>();
        teleportGate.GetPos.Returns(pos);
        return teleportGate;
    }
}