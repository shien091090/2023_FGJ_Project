using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.AdapterTools;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

public class CharacterTest
{
    private IMoveController moveController;
    private CharacterModel characterModel;
    private IKeyController keyController;
    private IRigidbody2DAdapter rigidbody;
    private IAudioManager audioManager;
    private IDeltaTimeGetter deltaTimeGetter;
    private ICharacterView characterView;
    private IItemTriggerHandler itemTriggerHandler;
    private ICharacterSetting characterSetting;
    private ICharacterPresenter presenter;

    private Action characterViewWaitingCallback;
    private Action afterDieAnimationCallback;
    private Action afterBackOriginCallback;

    [SetUp]
    public void Setup()
    {
        moveController = Substitute.For<IMoveController>();
        keyController = Substitute.For<IKeyController>();
        audioManager = Substitute.For<IAudioManager>();
        deltaTimeGetter = Substitute.For<IDeltaTimeGetter>();

        characterView = Substitute.For<ICharacterView>();
        characterView.Waiting(Arg.Any<float>(), Arg.Do<Action>(callback =>
        {
            characterViewWaitingCallback = callback;
        }));

        itemTriggerHandler = Substitute.For<IItemTriggerHandler>();

        InitCharacterSettingMock();
        InitPresenterMock();

        characterModel = new CharacterModel(moveController, keyController, deltaTimeGetter, itemTriggerHandler,
            characterSetting);

        characterModel.BindPresenter(presenter);
    }

    [Test]
    [TestCase(0.5f, true)]
    [TestCase(1f, true)]
    [TestCase(-0.01f, false)]
    [TestCase(-0.9f, false)]
    //水平移動
    public void horizontal_move(float axisValue, bool expectedMoveRight)
    {
        GivenHorizontalAxis(axisValue);

        characterModel.CallUpdate();

        LastTranslateShouldBeRight(expectedMoveRight);
    }

    [Test]
    //沒有按跳躍按鍵時, 不會跳躍
    public void not_jump_when_not_press_key()
    {
        GivenIsJumpKeyDown(false);
        characterModel.CallUpdate();

        ShouldCallJump(0);
        ShouldIsJumping(false);
    }

    [Test]
    //跳躍時不能再跳
    public void can_not_jump_when_jumping()
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
    public void can_not_jump_when_not_on_floor()
    {
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallJump(0);
    }

    [Test]
    //跳躍力道設定為0時, 按跳躍時不會跳
    public void can_not_jump_when_jump_force_is_zero()
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
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        ShouldCallJump(1);
        ShouldIsJumping(true);

        characterModel.CallUpdate();
        characterModel.CollisionEnter2D(CreateCollision(GameConst.GameObjectLayerType.Platform, true));

        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();

        ShouldCallJump(2);
    }

    [Test]
    //跳躍後, 在跳躍延遲時間內觸發地板, 不可再跳
    public void can_not_jump_again_when_trigger_floor_in_delay_time()
    {
        GivenJumpDelay(0.7f);
        GivenDeltaTime(0.3f);
        GivenIsJumpKeyDown(true);

        characterModel.CallUpdate(); //0s
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        ShouldIsJumping(true);
        ShouldCallJump(1);

        characterModel.CallUpdate(); //0.3s
        characterModel.CollisionEnter2D(CreateCollision(GameConst.GameObjectLayerType.Platform, true));

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate(); //0.6s

        ShouldIsJumping(false);
        ShouldCallJump(1);
    }

    [Test]
    //跳躍後, 在跳躍延遲時間過後觸發地板, 可再跳
    public void can_jump_again_when_trigger_floor_after_delay_time()
    {
        GivenJumpDelay(0.7f);
        GivenDeltaTime(0.6f);
        GivenIsJumpKeyDown(true);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate(); //0s
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        ShouldCallJump(1);

        characterModel.CallUpdate(); //0.6s
        characterModel.CollisionEnter2D(CreateCollision(GameConst.GameObjectLayerType.Platform, true));

        ShouldIsJumping(false);

        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate(); //1.2s 可跳躍
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        ShouldCallJump(2);
    }

    [Test]
    //在走路狀態接觸傳送門時, 點擊互動按鍵後傳送
    public void teleport_when_touch_teleport_and_click_interact_button_in_walking_state()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate component = CreateTeleportGateComponent(teleportTargetPos: new Vector3(5, 10));
        GivenGetComponent(collider, component);

        characterModel.ColliderTriggerEnter2D(collider);

        ShouldHaveTriggerTeleportGate(true);
        CurrentCharacterPosShouldBe(Vector3.zero);
        CurrentCharacterStateShouldBe(CharacterState.Walking);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        CurrentCharacterPosShouldBe(new Vector3(5, 10));
        ShouldPlayTeleportEffect();
    }
    
    //在跳躍狀態接觸傳送門時, 點擊互動按鍵後傳送
    //在死亡狀態接觸傳送門時, 點擊互動按鍵不做事

    [Test]
    //觸發傳送門後, 角色速度歸零
    public void character_velocity_should_be_zero_when_teleport()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate component = CreateTeleportGateComponent(teleportTargetPos: new Vector3(5, 10));
        GivenGetComponent(collider, component);
        GivenVelocity(new Vector2(15, 0));

        characterModel.ColliderTriggerEnter2D(collider);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        CharacterVelocityShouldBe(Vector2.zero);
    }

    [Test]
    //接觸傳送門但取不到Component, 點擊按鍵不會觸發傳送
    public void not_teleport_when_touch_teleport_gate_and_get_null_component()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.TeleportGate);
        GivenGetComponent(collider, default(ITeleportGate));

        characterModel.ColliderTriggerEnter2D(collider);

        ShouldHaveTriggerTeleportGate(false);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        ShouldHaveTriggerTeleportGate(false);
        ShouldNotPlayTeleportEffect();
    }

    [Test]
    //接觸傳送門時, 點擊按鍵但超過距離, 不傳送
    public void not_teleport_when_touch_teleport_but_over_distance()
    {
        GivenInteractDistance(2f);
        GivenCharacterPosition(new Vector3(1, 0));

        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate teleportGate = CreateTeleportGateComponent(new Vector3(5, 0));
        GivenGetComponent(collider, teleportGate);
        characterModel.ColliderTriggerEnter2D(collider);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        ShouldNotPlayTeleportEffect();
        ShouldHaveTriggerTeleportGate(false);
    }

    [Test]
    //接觸傳送門後再離開, 點擊按鍵後不會觸發傳送
    public void not_teleport_when_touch_teleport_then_exit()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate teleportGate = CreateTeleportGateComponent();
        GivenGetComponent(collider, teleportGate);

        characterModel.ColliderTriggerEnter2D(collider);
        characterModel.ColliderTriggerExit2D(collider);

        ShouldHaveTriggerTeleportGate(false);

        GivenInteractKeyDown(true);
        characterModel.CallUpdate();

        ShouldNotPlayTeleportEffect();
    }

    [Test]
    //接觸怪物會死亡
    public void die_when_touch_monster()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.Monster);
        GivenGetComponent(collider, CreateMonster(MonsterState.Normal));
        characterModel.ColliderTriggerStay2D(collider);

        CurrentCharacterStateShouldBe(CharacterState.Die);
        ShouldPlayDieEffect();
    }

    [Test]
    //死亡後返回初始位置
    public void back_to_origin_position_when_die()
    {
        GivenCharacterPosition(new Vector3(10, 20));

        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.Monster);
        GivenGetComponent(collider, CreateMonster(MonsterState.Normal));
        characterModel.ColliderTriggerStay2D(collider);

        CurrentCharacterStateShouldBe(CharacterState.Die);
        CurrentCharacterPosShouldBe(new Vector3(10, 20));

        CallAfterDieAnimationCallback();

        CurrentCharacterPosShouldBe(Vector3.zero);
    }

    [Test]
    //死亡返回初始位置後, 切換狀態為Walking
    public void change_state_to_walking_after_die_and_back_to_origin_position()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.Monster);
        GivenGetComponent(collider, CreateMonster(MonsterState.Normal));
        characterModel.ColliderTriggerStay2D(collider);

        CurrentCharacterStateShouldBe(CharacterState.Die);

        CallAfterBackOriginCallback();

        CurrentCharacterStateShouldBe(CharacterState.Walking);
    }

    [Test]
    //接觸暈眩中的怪不會死亡
    public void not_die_when_touch_stun_monster()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.Monster);
        GivenGetComponent(collider, CreateMonster(MonsterState.Stun));
        characterModel.ColliderTriggerStay2D(collider);

        CurrentCharacterStateShouldBe(CharacterState.Walking);
    }

    [Test]
    //接觸暈眩中的怪物, 怪物解除暈眩時會死亡
    public void die_when_touch_stun_monster_and_monster_recover()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.Monster);
        IMonsterView monster = CreateMonster(MonsterState.Stun);
        GivenGetComponent(collider, monster);
        characterModel.ColliderTriggerStay2D(collider);

        CurrentCharacterStateShouldBe(CharacterState.Walking);

        GivenMonsterCurrentState(monster, MonsterState.Normal);
        characterModel.ColliderTriggerStay2D(collider);

        CurrentCharacterStateShouldBe(CharacterState.Die);
    }

    [Test]
    //接觸怪物死亡時持續接觸, 死亡流程只會觸發一次
    public void die_only_one_time_when_continue_stay_touch_monster()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.Monster);
        GivenGetComponent(collider, CreateMonster(MonsterState.Normal));
        characterModel.ColliderTriggerStay2D(collider);

        ShouldPlayDieEffect(1);
        
        characterModel.ColliderTriggerStay2D(collider);
        
        ShouldPlayDieEffect(1);
        
        CallAfterDieAnimationCallback();
        characterModel.ColliderTriggerStay2D(collider);
        
        ShouldPlayDieEffect(1);
    }

    [Test]
    //墜落至指定高度時傳回原點
    public void back_to_origin_when_fall_to_specified_height()
    {
        GivenFallDownLimitPos(-5);
        GivenCharacterPosition(new Vector3(0, -4, 0));
        GivenVelocity(new Vector2(0, -1));
        
        characterModel.CallUpdate();

        CurrentCharacterStateShouldBe(CharacterState.Walking);
        ShouldNotPlayTeleportEffect();
        CurrentCharacterPosShouldBe(new Vector3(0, -4, 0));
        CharacterVelocityShouldBe(new Vector2(0, -1));

        GivenCharacterPosition(new Vector3(0, -5, 0));
        characterModel.CallUpdate();

        CurrentCharacterStateShouldBe(CharacterState.Die);
        ShouldPlayTeleportEffect();
        CurrentCharacterPosShouldBe(Vector3.zero);
        CharacterVelocityShouldBe(Vector3.zero);
    }
    
    //接觸儲存點時, 顯示儲存點提示
    //接觸儲存點再離開, 隱藏儲存點提示
    //在一般狀態接觸儲存點時, 點擊互動按鍵後儲存位置並進入房屋
    //在跳躍狀態接觸儲存點時, 點擊互動按鍵不做事
    //在死亡狀態接觸儲存點時, 點擊互動按鍵不做事
    //進入房屋時角色速度歸零
    //進入房屋時, 不可移動
    //進入房屋時, 不可跳躍
    //進入房屋時, 觸碰怪物不會死亡
    //接觸儲存點但取不到Component, 點擊按鍵不做事
    //接觸儲存點時, 點擊按鍵但超過距離, 不做事
    //接觸儲存點後再離開, 點擊按鍵不會觸發儲存點
    //進入房屋後, 再次按下互動按鍵, 離開房屋
    //進入房屋後, 若沒有紀錄其他儲存點, 不可在房屋之間傳送
    //進入房屋後, 若有紀錄其他儲存點, 可在房屋之間傳送, 傳送後維持進入房屋狀態

    private void InitCharacterSettingMock()
    {
        characterSetting = Substitute.For<ICharacterSetting>();
        GivenDeltaTime(1);
        GivenSpeed(1);
        GivenJumpForce(1);
        GivenJumpDelay(1);
        GivenFallDownLimitPos(-10);
    }

    private void InitPresenterMock()
    {
        presenter = Substitute.For<ICharacterPresenter>();

        rigidbody = Substitute.For<IRigidbody2DAdapter>();
        presenter.GetRigidbody.Returns(rigidbody);

        presenter.When(x => x.PlayDieEffect(Arg.Any<Action>(), Arg.Any<Action>())).Do(callInfo =>
        {
            afterDieAnimationCallback = (Action)callInfo.Args()[0];
            afterBackOriginCallback = (Action)callInfo.Args()[1];
        });
    }

    private void GivenVelocity(Vector2 velocity)
    {
        rigidbody.velocity = velocity;
    }

    private void GivenFallDownLimitPos(float pos)
    {
        characterSetting.FallDownLimitPosY.Returns(pos);
    }

    private void GivenInteractDistance(float distance)
    {
        characterSetting.InteractDistance.Returns(distance);
    }

    private void GivenJumpDelay(float delaySeconds)
    {
        characterSetting.JumpDelaySeconds.Returns(delaySeconds);
    }

    private void GivenJumpForce(int jumpForce)
    {
        characterSetting.JumpForce.Returns(jumpForce);
    }

    private void GivenSpeed(int speed)
    {
        characterSetting.Speed.Returns(speed);
    }

    private void GivenDeltaTime(float deltaTime)
    {
        deltaTimeGetter.deltaTime.Returns(deltaTime);
    }

    private void GivenCharacterPosition(Vector3 pos)
    {
        rigidbody.position.Returns(pos);
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

    private void GivenGetComponent<T>(ICollider2DAdapter collider, T component)
    {
        collider.GetComponent<T>().Returns(component);
    }

    private void GivenMonsterCurrentState(IMonsterView monsterView, MonsterState monsterState)
    {
        monsterView.CurrentState.Returns(monsterState);
    }

    private void CallAfterBackOriginCallback()
    {
        afterBackOriginCallback.Invoke();
    }

    private void CallAfterDieAnimationCallback()
    {
        afterDieAnimationCallback.Invoke();
    }

    private void CallCharacterViewWaitingCallback()
    {
        characterViewWaitingCallback.Invoke();
    }

    private void ShouldPlayDieEffect(int expectedCallTimes = 1)
    {
        presenter.Received(expectedCallTimes).PlayDieEffect(Arg.Any<Action>(), Arg.Any<Action>());
    }

    private void CharacterVelocityShouldBe(Vector2 expectedVelocity)
    {
        Assert.AreEqual(expectedVelocity, rigidbody.velocity);
    }

    private void CurrentCharacterStateShouldBe(CharacterState expectedState)
    {
        Assert.AreEqual(expectedState, characterModel.CurrentCharacterState);
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
        // Assert.AreEqual(expectedIsFaceRight, characterModel.IsFaceRight);
    }

    private void ShouldAudioPlayOneShot(string audioKey, int callTimes = 1)
    {
        audioManager.Received(callTimes).PlayOneShot(audioKey);
    }

    private void CurrentCharacterPosShouldBe(Vector3 expectedPos)
    {
        Assert.AreEqual(expectedPos, rigidbody.position);
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

    private void RecordOriginPosShouldBe(Vector3 expectedPos)
    {
        Assert.AreEqual(expectedPos, characterModel.RecordOriginPos);
    }

    private void ShouldHaveTriggerTeleportGate(bool expectedHave)
    {
        Assert.AreEqual(expectedHave, characterModel.HaveInteractGate);
    }

    private void ShouldNotPlayTeleportEffect()
    {
        presenter.DidNotReceive().PlayTeleportEffect();
    }

    private void ShouldPlayTeleportEffect(int callTimes = 1)
    {
        presenter.Received(callTimes).PlayTeleportEffect();
    }

    private void ShouldCallJump(int triggerTimes)
    {
        rigidbody.Received(triggerTimes).AddForce(Arg.Is<Vector2>(v => v.y > 0 && v.x == 0));
    }

    private void ShouldIsJumping(bool expectedIsJumping)
    {
        Assert.AreEqual(expectedIsJumping, characterModel.IsJumping);
    }

    private void LastTranslateShouldBeRight(bool isRight)
    {
        float argument = (float)presenter
            .ReceivedCalls()
            .Last(x => x.GetMethodInfo().Name == "PlayerMoveEffect")
            .GetArguments()[0];

        if (isRight)
            Assert.IsTrue(argument > 0);
        else
            Assert.IsTrue(argument < 0);
    }

    private IMonsterView CreateMonster(MonsterState monsterState)
    {
        IMonsterView monsterView = Substitute.For<IMonsterView>();
        GivenMonsterCurrentState(monsterView, monsterState);
        return monsterView;
    }

    private ICollider2DAdapter CreateCollider(GameConst.GameObjectLayerType collisionLayer)
    {
        ICollider2DAdapter collider = Substitute.For<ICollider2DAdapter>();
        collider.Layer.Returns((int)collisionLayer);
        return collider;
    }

    private ICollision2DAdapter CreateCollision(GameConst.GameObjectLayerType collisionLayer, bool isPhysicsOverlapCircle = false)
    {
        ICollision2DAdapter collision = Substitute.For<ICollision2DAdapter>();
        collision.Layer.Returns((int)collisionLayer);
        collision.CheckPhysicsOverlapCircle(Arg.Any<Vector3>(), Arg.Any<float>(), Arg.Any<GameConst.GameObjectLayerType>()).Returns(isPhysicsOverlapCircle);
        return collision;
    }

    private ITeleportGate CreateTeleportGateComponent(Vector3 pos = default, Vector3 teleportTargetPos = default)
    {
        ITeleportGate teleportGate = Substitute.For<ITeleportGate>();
        teleportGate.GetPos.Returns(pos);
        teleportGate.GetTargetPos.Returns(teleportTargetPos);
        return teleportGate;
    }
}