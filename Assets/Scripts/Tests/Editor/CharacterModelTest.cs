using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.AdapterTools;
using UnityEngine;

public class CharacterModelTest
{
    private IMoveController moveController;
    private CharacterModel characterModel;
    private IKeyController keyController;
    private IRigidbody2DAdapter rigidbody;
    private IDeltaTimeGetter deltaTimeGetter;
    private IItemTriggerHandler itemTriggerHandler;
    private ICharacterSetting characterSetting;
    private ICharacterPresenter presenter;

    private Action afterDieAnimationCallback;
    private Action afterBackOriginCallback;
    private Action triggerInteractiveObjectEvent;
    private Action unTriggerInteractiveObjectEvent;
    private Action playEnterHouseEffectCallback;
    private Action playExitHouseEffectCallback;

    private static void GivenNextSavePointComponent(ISavePointView savePoint, ISavePointView savePointComponent)
    {
        savePoint.GetModel.GetNextSavePointView().Returns(savePointComponent);
    }

    [SetUp]
    public void Setup()
    {
        moveController = Substitute.For<IMoveController>();
        keyController = Substitute.For<IKeyController>();
        deltaTimeGetter = Substitute.For<IDeltaTimeGetter>();
        itemTriggerHandler = Substitute.For<IItemTriggerHandler>();

        InitCharacterSettingMock();
        InitPresenterMock();

        characterModel = new CharacterModel(moveController, keyController, deltaTimeGetter, itemTriggerHandler,
            characterSetting);

        characterModel.BindPresenter(presenter);

        triggerInteractiveObjectEvent = Substitute.For<Action>();
        characterModel.OnTriggerInteractiveObject += triggerInteractiveObjectEvent;

        unTriggerInteractiveObjectEvent = Substitute.For<Action>();
        characterModel.OnUnTriggerInteractiveObject += unTriggerInteractiveObjectEvent;
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
    //沒有按左右移動按鍵時, 不會移動
    public void not_move_when_not_press_key()
    {
        GivenHorizontalAxis(0);

        characterModel.CallUpdate();

        ShouldNotPlayMoveEffect();
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
        CallJumpKeyDown();

        ShouldCallJump(1);
        ShouldIsJumping(true);

        CallJumpKeyDown();

        ShouldCallJump(1);
    }

    [Test]
    //離地時不能跳
    public void can_not_jump_when_not_on_floor()
    {
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        CallJumpKeyDown();

        ShouldCallJump(0);
    }

    [Test]
    //跳躍力道設定為0時, 按跳躍時不會跳
    public void can_not_jump_when_jump_force_is_zero()
    {
        GivenJumpForce(0);

        CallJumpKeyDown();

        ShouldCallJump(0);
    }

    [Test]
    //跳躍落地後可再跳
    public void can_jump_when_back_to_floor()
    {
        CallJumpKeyDown();
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        ShouldCallJump(1);
        ShouldIsJumping(true);

        characterModel.CallUpdate();
        characterModel.CollisionEnter2D(CreateCollision(GameConst.GameObjectLayerType.Platform, true));

        ShouldIsJumping(false);

        CallJumpKeyDown();

        ShouldCallJump(2);
    }

    [Test]
    //跳躍後, 在跳躍延遲時間內觸發地板, 不可再跳
    public void can_not_jump_again_when_trigger_floor_in_delay_time()
    {
        GivenJumpDelay(0.7f);
        GivenDeltaTime(0.3f);

        CallJumpKeyDown(); //0s
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        ShouldIsJumping(true);
        ShouldCallJump(1);

        CallJumpKeyDown(); //0.3s
        characterModel.CollisionEnter2D(CreateCollision(GameConst.GameObjectLayerType.Platform, true));

        CallJumpKeyDown(); //0.6s

        ShouldIsJumping(false);
        ShouldCallJump(1);
    }

    [Test]
    //跳躍後, 在跳躍延遲時間過後觸發地板, 可再跳
    public void can_jump_again_when_trigger_floor_after_delay_time()
    {
        GivenJumpDelay(0.7f);
        GivenDeltaTime(0.6f);

        CallJumpKeyDown(); //0s
        characterModel.CollisionExit2D(CreateCollision(GameConst.GameObjectLayerType.Platform));

        ShouldCallJump(1);

        CallJumpKeyDown(); //0.6s
        characterModel.CollisionEnter2D(CreateCollision(GameConst.GameObjectLayerType.Platform, true));

        ShouldIsJumping(false);

        CallJumpKeyDown(); //1.2s 可跳躍
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

        CallInteractKeyDown();

        CurrentCharacterPosShouldBe(new Vector3(5, 10));
        ShouldPlayTeleportEffect();
    }

    [Test]
    //在跳躍狀態接觸傳送門時, 點擊互動按鍵後傳送
    public void teleport_when_touch_teleport_gate_and_click_interact_button_in_jumping_state()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate component = CreateTeleportGateComponent(teleportTargetPos: new Vector3(5, 10));
        GivenGetComponent(collider, component);

        CallJumpKeyDown();
        characterModel.ColliderTriggerEnter2D(collider);

        ShouldHaveTriggerTeleportGate(true);
        CurrentCharacterPosShouldBe(Vector3.zero);
        CurrentCharacterStateShouldBe(CharacterState.Jumping);

        CallInteractKeyDown();

        CurrentCharacterPosShouldBe(new Vector3(5, 10));
        ShouldPlayTeleportEffect();
    }

    [Test]
    //在死亡狀態接觸傳送門時, 點擊互動按鍵不做事
    public void not_teleport_when_touch_teleport_gate_and_click_interact_button_in_dead_state()
    {
        GivenGetComponent(
            CreateCollider(GameConst.GameObjectLayerType.Monster),
            CreateMonster(MonsterState.Normal));

        characterModel.ColliderTriggerStay2D(CreateCollider(GameConst.GameObjectLayerType.Monster));

        CurrentCharacterStateShouldBe(CharacterState.Die);

        ICollider2DAdapter teleportGateCollider = CreateCollider(GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate teleportGateComponent = CreateTeleportGateComponent(teleportTargetPos: new Vector3(5, 10));
        GivenGetComponent(teleportGateCollider, teleportGateComponent);

        characterModel.ColliderTriggerEnter2D(teleportGateCollider);

        ShouldHaveTriggerTeleportGate(true);

        CallInteractKeyDown();

        CurrentCharacterPosShouldBe(Vector3.zero);
        ShouldNotPlayTeleportEffect();
    }

    [Test]
    //觸發傳送門後, 角色速度歸零
    public void character_velocity_should_be_zero_when_teleport()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.TeleportGate);
        ITeleportGate component = CreateTeleportGateComponent(teleportTargetPos: new Vector3(5, 10));
        GivenGetComponent(collider, component);
        GivenVelocity(new Vector2(15, 0));

        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();

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

        CallInteractKeyDown();

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

        CallInteractKeyDown();

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

        CallInteractKeyDown();

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

    [Test]
    //接觸儲存點時, 顯示儲存點提示
    public void show_save_point_hint_when_touch_save_point()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        ShouldShowRecordStateHint(savePoint);
        ShouldHaveTriggerSavePoint(true);
        ShouldSendTriggerInteractiveObjectEvent(1);
    }

    [Test]
    //接觸儲存點再離開, 隱藏儲存點提示
    public void hide_save_point_hint_when_touch_save_point_then_exit()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);
        characterModel.ColliderTriggerExit2D(collider);

        ShouldSavePointHideAllUI(savePoint);
        ShouldHaveTriggerSavePoint(false);
        ShouldSendUnTriggerInteractiveObjectEvent(1);
    }

    [Test]
    //在一般狀態接觸儲存點時, 點擊互動按鍵後儲存位置並進入房屋
    public void save_position_and_into_house_when_touch_save_point_and_click_interact_button_in_normal_state()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();

        CurrentCharacterStateShouldBe(CharacterState.Walking);
        ShouldSaveCurrentPoint(savePoint);

        CallPlayEnterHouseEffectCallback();

        CurrentCharacterStateShouldBe(CharacterState.IntoHouse);
    }

    [Test]
    //在跳躍狀態接觸儲存點時, 點擊互動按鍵不做事
    public void do_nothing_when_touch_save_point_and_click_interact_button_in_jumping_state()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallJumpKeyDown();

        CurrentCharacterStateShouldBe(CharacterState.Jumping);

        CallInteractKeyDown();

        ShouldNotSaveCurrentPoint(savePoint);
    }

    [Test]
    //在死亡狀態接觸儲存點時, 點擊互動按鍵不做事
    public void do_nothing_when_touch_save_point_and_click_interact_button_in_dead_state()
    {
        characterModel.ColliderTriggerStay2D(CreateCollider(GameConst.GameObjectLayerType.Monster));

        CurrentCharacterStateShouldBe(CharacterState.Die);

        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();

        ShouldNotSaveCurrentPoint(savePoint);
    }

    [Test]
    //進入房屋時角色速度歸零
    public void character_velocity_should_be_zero_when_into_house()
    {
        GivenVelocity(new Vector2(15, 0));

        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();

        CharacterVelocityShouldBe(Vector2.zero);
    }

    [Test]
    //進入房屋時, 不可移動
    public void can_not_move_when_into_house()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();

        GivenHorizontalAxis(1);
        characterModel.CallUpdate();

        ShouldNotPlayMoveEffect();
    }

    [Test]
    //進入房屋時, 不可跳躍
    public void can_not_jump_when_into_house()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();
        CallJumpKeyDown();

        ShouldCallJump(0);
        ShouldIsJumping(false);
    }

    [Test]
    //進入房屋時, 觸碰怪物不會死亡
    public void not_die_when_touch_monster_in_house()
    {
        ICollider2DAdapter savePointCollider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(savePointCollider, savePoint);
        characterModel.ColliderTriggerEnter2D(savePointCollider);

        CallInteractKeyDown();
        CallPlayEnterHouseEffectCallback();

        CurrentCharacterStateShouldBe(CharacterState.IntoHouse);

        ICollider2DAdapter monsterCollider = CreateCollider(GameConst.GameObjectLayerType.Monster);
        GivenGetComponent(monsterCollider, CreateMonster(MonsterState.Normal));
        characterModel.ColliderTriggerStay2D(monsterCollider);

        CurrentCharacterStateShouldBe(CharacterState.IntoHouse);
        ShouldNotPlayDieEffect();
    }

    [Test]
    //接觸儲存點但取不到Component, 點擊按鍵不做事
    public void do_nothing_when_touch_save_point_and_get_null_component()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        GivenGetComponent(collider, default(ISavePointView));
        characterModel.ColliderTriggerEnter2D(collider);

        ShouldHaveTriggerSavePoint(false);

        CallInteractKeyDown();

        ShouldNotPlayEnterHouseEffect();
    }

    [Test]
    //接觸儲存點時, 點擊按鍵但超過距離, 不做事
    public void do_nothing_when_touch_save_point_but_over_distance()
    {
        GivenInteractDistance(2f);
        GivenCharacterPosition(new Vector3(1, 0));

        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent(new Vector3(3.1f, 0));
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();

        ShouldNotPlayEnterHouseEffect();
        ShouldHaveTriggerSavePoint(false);
    }

    [Test]
    //接觸儲存點後再離開, 點擊按鍵不會觸發儲存點
    public void do_nothing_when_touch_save_point_then_exit()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);
        characterModel.ColliderTriggerExit2D(collider);

        ShouldHaveTriggerSavePoint(false);

        CallInteractKeyDown();

        ShouldNotPlayEnterHouseEffect();
    }

    [Test]
    //進入房屋後, 再次按下互動按鍵, 離開房屋
    public void leave_house_when_click_interact_button_after_into_house()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent();
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();
        CallPlayEnterHouseEffectCallback();

        CurrentCharacterStateShouldBe(CharacterState.IntoHouse);

        CallInteractKeyDown();
        CallPlayExitHouseEffectCallback();

        CurrentCharacterStateShouldBe(CharacterState.Walking);
    }

    [Test]
    //進入房屋後, 若沒有紀錄其他儲存點, 不可在房屋之間傳送
    public void can_not_teleport_between_house_when_no_other_save_point()
    {
        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent(haveNextSavePoint: false, havePreviousSavePoint: false);
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();
        CallPlayEnterHouseEffectCallback();

        CurrentCharacterStateShouldBe(CharacterState.IntoHouse);

        CallRightKeyDown();

        ShouldNotPlayTeleportEffect();

        CallLeftKeyDown();

        ShouldNotPlayTeleportEffect();
    }

    [Test]
    //進入房屋後, 若有紀錄其他儲存點, 可在房屋之間傳送, 傳送後維持進入房屋狀態
    public void can_teleport_between_house_when_have_other_save_point()
    {
        GivenCharacterPosition(new Vector3(0, 0, 0));

        ICollider2DAdapter collider = CreateCollider(GameConst.GameObjectLayerType.SavePoint);
        ISavePointView savePoint = CreateSavePointComponent(haveNextSavePoint: true, havePreviousSavePoint: true);
        GivenNextSavePointComponent(savePoint, CreateSavePointComponent(savePos: new Vector3(100, 0, 0)));
        GivenGetComponent(collider, savePoint);
        characterModel.ColliderTriggerEnter2D(collider);

        CallInteractKeyDown();
        CallPlayEnterHouseEffectCallback();

        CurrentCharacterStateShouldBe(CharacterState.IntoHouse);
        CurrentCharacterPosShouldBe(new Vector3(0, 0, 0));

        CallRightKeyDown();

        CurrentCharacterPosShouldBe(new Vector3(100, 0, 0));
        ShouldPlayTeleportEffect();
        CurrentCharacterStateShouldBe(CharacterState.IntoHouse);
    }

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
        afterDieAnimationCallback = null;
        afterBackOriginCallback = null;
        playEnterHouseEffectCallback = null;
        playExitHouseEffectCallback = null;

        presenter = Substitute.For<ICharacterPresenter>();

        rigidbody = Substitute.For<IRigidbody2DAdapter>();
        presenter.GetRigidbody.Returns(rigidbody);

        presenter.When(x => x.PlayDieEffect(Arg.Any<Action>(), Arg.Any<Action>())).Do(callInfo =>
        {
            afterDieAnimationCallback = (Action)callInfo.Args()[0];
            afterBackOriginCallback = (Action)callInfo.Args()[1];
        });

        presenter.When(x => x.PlayEnterHouseEffect(Arg.Any<Action>())).Do(callInfo =>
        {
            playEnterHouseEffectCallback = (Action)callInfo.Args()[0];
        });

        presenter.When(x => x.PlayExitHouseEffect(Arg.Any<Action>())).Do(callInfo =>
        {
            playExitHouseEffectCallback = (Action)callInfo.Args()[0];
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

    private void GivenLeftKeyDown(bool isKeyDown)
    {
        keyController.IsLeftKeyDown.Returns(isKeyDown);
    }

    private void GivenRightKeyDown(bool isKeyDown)
    {
        keyController.IsRightKeyDown.Returns(isKeyDown);
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

    private void CallLeftKeyDown()
    {
        GivenLeftKeyDown(true);
        GivenRightKeyDown(false);
        characterModel.CallUpdate();
        GivenLeftKeyDown(false);
    }

    private void CallRightKeyDown()
    {
        GivenRightKeyDown(true);
        GivenLeftKeyDown(false);
        characterModel.CallUpdate();
        GivenRightKeyDown(false);
    }

    private void CallJumpKeyDown()
    {
        GivenIsJumpKeyDown(true);
        characterModel.CallUpdate();
        GivenIsJumpKeyDown(false);
    }

    private void CallInteractKeyDown()
    {
        GivenInteractKeyDown(true);
        characterModel.CallUpdate();
        GivenInteractKeyDown(false);
    }

    private void CallPlayExitHouseEffectCallback()
    {
        playExitHouseEffectCallback.Invoke();
    }

    private void CallPlayEnterHouseEffectCallback()
    {
        playEnterHouseEffectCallback.Invoke();
    }

    private void CallAfterBackOriginCallback()
    {
        afterBackOriginCallback.Invoke();
    }

    private void CallAfterDieAnimationCallback()
    {
        afterDieAnimationCallback.Invoke();
    }

    private void ShouldNotPlayEnterHouseEffect()
    {
        presenter.DidNotReceive().PlayEnterHouseEffect(Arg.Any<Action>());
    }

    private void ShouldNotSaveCurrentPoint(ISavePointView savePoint)
    {
        savePoint.GetModel.DidNotReceive().Save();
    }

    private void ShouldSaveCurrentPoint(ISavePointView savePoint, int expectedCallTimes = 1)
    {
        savePoint.GetModel.Received(expectedCallTimes).Save();
    }

    private void ShouldSendUnTriggerInteractiveObjectEvent(int expectedTriggerTimes)
    {
        unTriggerInteractiveObjectEvent.Received(expectedTriggerTimes).Invoke();
    }

    private void ShouldSavePointHideAllUI(ISavePointView savePoint)
    {
        savePoint.GetModel.Received().HideAllUI();
    }

    private void ShouldSendTriggerInteractiveObjectEvent(int expectedTriggerTimes)
    {
        triggerInteractiveObjectEvent.Received(expectedTriggerTimes).Invoke();
    }

    private void ShouldHaveTriggerSavePoint(bool expectedHave)
    {
        Assert.AreEqual(expectedHave, characterModel.HaveInteractSavePoint);
    }

    private void ShouldShowRecordStateHint(ISavePointView savePointView)
    {
        savePointView.GetModel.Received().ShowRecordStateHint();
    }

    private void ShouldNotPlayDieEffect()
    {
        presenter.DidNotReceive().PlayDieEffect(Arg.Any<Action>(), Arg.Any<Action>());
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

    private void CurrentCharacterPosShouldBe(Vector3 expectedPos)
    {
        Assert.AreEqual(expectedPos, rigidbody.position);
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
        if (triggerTimes == 0)
            rigidbody.DidNotReceive().AddForce(Arg.Any<Vector2>(), Arg.Any<ForceMode2D>());
        else
            rigidbody.Received(triggerTimes).AddForce(Arg.Is<Vector2>(v => v.y > 0 && v.x == 0));
    }

    private void ShouldIsJumping(bool expectedIsJumping)
    {
        Assert.AreEqual(expectedIsJumping, characterModel.IsJumping);
    }

    private void ShouldNotPlayMoveEffect()
    {
        presenter.DidNotReceive().PlayerMoveEffect(Arg.Any<float>());
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

    private ISavePointView CreateSavePointComponent(Vector3 pos = default, Vector3 savePos = default, bool haveNextSavePoint = false, bool havePreviousSavePoint = false)
    {
        ISavePointView savePoint = Substitute.For<ISavePointView>();
        savePoint.GetPos.Returns(pos);
        savePoint.SavePointPos.Returns(savePos);
        ISavePointModel savePointModel = Substitute.For<ISavePointModel>();
        savePointModel.HaveNextSavePoint().Returns(haveNextSavePoint);
        savePointModel.HavePreviousSavePoint().Returns(havePreviousSavePoint);
        savePoint.GetModel.Returns(savePointModel);
        return savePoint;
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
        collision.CheckPhysicsOverlapCircle(Arg.Any<Vector3>(), Arg.Any<float>(), Arg.Any<string>()).Returns(isPhysicsOverlapCircle);
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