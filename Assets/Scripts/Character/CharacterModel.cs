using System;
using SNShien.Common.AdapterTools;
using UnityEngine;

public class CharacterModel : ICharacterModel
{
    public CharacterState CurrentCharacterState { get; private set; }

    private readonly IMoveController moveController;
    private readonly IKeyController keyController;
    private readonly IDeltaTimeGetter deltaTimeGetter;
    private readonly IItemTriggerHandler itemTriggerHandler;
    private readonly ICharacterSetting characterSetting;

    private ICharacterPresenter characterPresenter;
    private IRigidbody2DAdapter selfRigidbody;
    private float jumpTimer;
    private bool isCollideRightWall;
    private bool isCollideLeftWall;
    private bool isFreeze;
    private bool isProtected;

    public bool IsJumping { get; private set; }
    public bool HaveInteractGate => CurrentTriggerTeleportGate != null;
    public Vector3 RecordOriginPos { get; private set; }
    private bool HaveInteractSavePoint => CurrentTriggerSavePoint != null;
    private ITeleportGate CurrentTriggerTeleportGate { get; set; }
    private ISavePointView CurrentTriggerSavePoint { get; set; }
    private bool IsStayOnFloor { get; set; }

    public CharacterModel(IMoveController moveController, IKeyController keyController, IDeltaTimeGetter deltaTimeGetter, IItemTriggerHandler itemTriggerHandler,
        ICharacterSetting characterSetting)
    {
        this.moveController = moveController;
        this.keyController = keyController;
        this.deltaTimeGetter = deltaTimeGetter;
        this.itemTriggerHandler = itemTriggerHandler;
        this.characterSetting = characterSetting;

        RegisterEvent();
    }

    public event Action OnCharacterDie;

    public event Action<CharacterState> OnChangeCharacterState;
    public event Action OnTriggerInteractiveObject;
    public event Action OnUnTriggerInteractiveObject;

    public void ColliderTriggerExitWall(bool isRightWall)
    {
        if (isRightWall)
            isCollideRightWall = false;
        else
            isCollideLeftWall = false;
    }

    public void ColliderTriggerEnterWall(bool isRightWall)
    {
        if (isRightWall)
            isCollideRightWall = true;
        else
            isCollideLeftWall = true;
    }

    public void ColliderTriggerEnter2D(ICollider2DAdapter col)
    {
        switch (col.Layer)
        {
            case (int)GameConst.GameObjectLayerType.TeleportGate:
                {
                    ITeleportGate teleportGateComponent = col.GetComponent<ITeleportGate>();
                    if (teleportGateComponent != null)
                    {
                        CurrentTriggerTeleportGate = teleportGateComponent;
                        OnTriggerInteractiveObject?.Invoke();
                    }

                    break;
                }

            case (int)GameConst.GameObjectLayerType.SavePoint:
                {
                    if (CurrentCharacterState == CharacterState.IntoHouse)
                        return;

                    ISavePointView savePointComponent = col.GetComponent<ISavePointView>();
                    if (savePointComponent != null)
                    {
                        CurrentTriggerSavePoint = savePointComponent;
                        CurrentTriggerSavePoint.GetModel.ShowRecordStateHint();
                        OnTriggerInteractiveObject?.Invoke();
                    }

                    break;
                }
        }
    }

    public void ColliderTriggerExit2D(ICollider2DAdapter col)
    {
        switch (col.Layer)
        {
            case (int)GameConst.GameObjectLayerType.TeleportGate:
                {
                    if (col.GetComponent<ITeleportGate>() != null)
                    {
                        CurrentTriggerTeleportGate = null;
                        OnUnTriggerInteractiveObject?.Invoke();
                    }

                    break;
                }

            case (int)GameConst.GameObjectLayerType.SavePoint:
                {
                    if (col.GetComponent<ISavePointView>() != null &&
                        CurrentTriggerSavePoint != null &&
                        CurrentCharacterState != CharacterState.IntoHouse &&
                        isFreeze == false)
                    {
                        CurrentTriggerSavePoint.GetModel.HideAllUI();
                        CurrentTriggerSavePoint = null;
                        OnUnTriggerInteractiveObject?.Invoke();
                    }

                    break;
                }
        }
    }

    public void ColliderTriggerStay2D(ICollider2DAdapter col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Monster && isProtected == false)
        {
            IMonsterView monsterView = col.GetComponent<IMonsterView>();
            if (monsterView == null || monsterView.CurrentState == MonsterState.Normal)
                Die();
        }
    }

    public void CollisionEnter2D(ICollision2DAdapter col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
        {
            characterPresenter.PlayCollidePlatformEffect(IsStayOnFloor == false);

            IsStayOnFloor = true;
            IsJumping = false;

            if (CurrentCharacterState != CharacterState.Die &&
                CurrentCharacterState != CharacterState.IntoHouse)
                ChangeCurrentCharacterState(CharacterState.Walking);
        }
    }

    public void CollisionExit2D(ICollision2DAdapter col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
        {
            IsStayOnFloor = false;
            if (CurrentCharacterState != CharacterState.Die &&
                CurrentCharacterState != CharacterState.IntoHouse)
                ChangeCurrentCharacterState(CharacterState.Jumping);
        }
    }

    public void CallUpdate()
    {
        if (CurrentCharacterState == CharacterState.Die)
            return;

        if (isFreeze)
            return;

        if (selfRigidbody.position.y <= characterSetting.FallDownLimitPosY)
        {
            BackToOrigin();
            return;
        }

        UpdateJumpTimer(deltaTimeGetter.deltaTime);
        UpdateCheckSavePointTeleport();
        UpdateCheckJump(characterSetting.JumpForce);
        UpdateMove(deltaTimeGetter.deltaTime, characterSetting.Speed);
        UpdateCheckInteract();
        characterPresenter.CallUpdate();
    }

    public void BindPresenter(ICharacterPresenter presenter)
    {
        characterPresenter = presenter;

        selfRigidbody = characterPresenter.GetRigidbody;
        jumpTimer = characterSetting.JumpDelaySeconds;
        isProtected = false;
        IsStayOnFloor = true;
        RecordOriginPos = selfRigidbody.position;

        ChangeCurrentCharacterState(CharacterState.Walking);
    }

    public void BackToOrigin()
    {
        ChangeCurrentCharacterState(CharacterState.Die);
        Teleport(RecordOriginPos);
        characterPresenter.PlayBackToOriginEffect(() =>
        {
            ChangeCurrentCharacterState(CharacterState.Walking);
        });
    }

    private bool CheckCanJump(float jumpForce)
    {
        if (IsJumping || IsStayOnFloor == false)
            return false;

        if (jumpForce == 0)
            return false;

        return true;
    }

    private void UpdateMove(float deltaTime, float speed)
    {
        if (CurrentCharacterState == CharacterState.IntoHouse)
            return;

        float horizontalAxis = moveController.GetHorizontalAxis();
        if ((horizontalAxis > 0 && isCollideRightWall) || (horizontalAxis < 0 && isCollideLeftWall))
            horizontalAxis = 0;

        float moveValue = horizontalAxis * deltaTime * speed;
        characterPresenter.PlayerMoveEffect(moveValue);
    }

    private void UpdateCheckJump(float jumpForce)
    {
        if (CurrentCharacterState == CharacterState.IntoHouse)
            return;

        if (jumpTimer < characterSetting.JumpDelaySeconds)
            return;

        if (keyController.IsJumpKeyDown && CheckCanJump(jumpForce))
        {
            Jump(jumpForce);
            characterPresenter.Jump();
        }
    }

    private void UpdateJumpTimer(float deltaTime)
    {
        if (jumpTimer >= characterSetting.JumpDelaySeconds)
            return;

        jumpTimer = Math.Min(jumpTimer + deltaTime, characterSetting.JumpDelaySeconds);
    }

    private void UpdateCheckSavePointTeleport()
    {
        if (CurrentCharacterState != CharacterState.IntoHouse ||
            HaveInteractSavePoint == false)
            return;

        ISavePointView targetPointView = null;
        if (keyController.IsRightKeyDown && CurrentTriggerSavePoint.GetModel.HaveNextSavePoint())
            targetPointView = CurrentTriggerSavePoint.GetModel.GetNextSavePointView();
        else if (keyController.IsLeftKeyDown && CurrentTriggerSavePoint.GetModel.HavePreviousSavePoint())
            targetPointView = CurrentTriggerSavePoint.GetModel.GetPreviousSavePointView();

        if (targetPointView == null)
            return;

        Teleport(targetPointView.SavePointPos);
        CurrentTriggerSavePoint.GetModel.HideAllUI();
        CurrentTriggerSavePoint = targetPointView;
        CurrentTriggerSavePoint.GetModel.ShowRecordStateHint();
        CurrentTriggerSavePoint.GetModel.ShowInteractHint();
    }

    private void UpdateCheckInteract()
    {
        if (!keyController.IsInteractKeyDown)
            return;

        if (selfRigidbody == null)
            return;

        if (CurrentCharacterState == CharacterState.IntoHouse)
        {
            isFreeze = true;
            characterPresenter.PlayExitHouseEffect(() =>
            {
                ChangeCurrentCharacterState(CharacterState.Walking);
                CurrentTriggerSavePoint.GetModel.HideInteractHint();
                isFreeze = false;
                OnTriggerInteractiveObject?.Invoke();
            });
        }
        else
        {
            if (HaveInteractGate)
                TriggerTeleportGate();
            else if (HaveInteractSavePoint)
                TriggerSavePoint();
        }
    }

    private void Jump(float jumpForce)
    {
        jumpTimer = 0;
        IsJumping = true;
        ChangeCurrentCharacterState(CharacterState.Jumping);
        selfRigidbody.AddForce(new Vector2(0, jumpForce));
    }

    private void ChangeCurrentCharacterState(CharacterState state)
    {
        if (CurrentCharacterState == state)
            return;

        Debug.Log($"ChangeCurrentCharacterState: {state}");

        CurrentCharacterState = state;
        OnChangeCharacterState?.Invoke(state);

        if (state == CharacterState.Die)
            OnCharacterDie?.Invoke();
    }

    private void Die()
    {
        if (CurrentCharacterState == CharacterState.Die)
            return;

        ChangeCurrentCharacterState(CharacterState.Die);
        characterPresenter.PlayDieEffect(() =>
        {
            BackToOrigin();
        }, () =>
        {
            isProtected = false;
            ChangeCurrentCharacterState(CharacterState.Walking);
        });
    }

    private void RegisterEvent()
    {
        itemTriggerHandler.OnEndItemEffect -= OnEndItemEffect;
        itemTriggerHandler.OnEndItemEffect += OnEndItemEffect;

        itemTriggerHandler.OnStartItemEffect -= OnStartItemEffect;
        itemTriggerHandler.OnStartItemEffect += OnStartItemEffect;

        itemTriggerHandler.OnUseItemOneTime -= OnUseItemOneTime;
        itemTriggerHandler.OnUseItemOneTime += OnUseItemOneTime;
    }

    private void Teleport(Vector3 targetPos)
    {
        characterPresenter.Teleport();
        selfRigidbody.position = targetPos;
        selfRigidbody.velocity = Vector2.zero;
    }

    private void TriggerSavePoint()
    {
        isFreeze = true;
        CurrentTriggerSavePoint.GetModel.Save();
        characterPresenter.PlayEnterHouseEffect(() =>
        {
            ChangeCurrentCharacterState(CharacterState.IntoHouse);
            CurrentTriggerSavePoint.GetModel.ShowInteractHint();
            isFreeze = false;
        });
    }

    private void TriggerTeleportGate()
    {
        float distance = Vector3.Distance(selfRigidbody.position, CurrentTriggerTeleportGate.GetPos);
        if (distance > characterSetting.InteractDistance)
        {
            CurrentTriggerTeleportGate = null;
            return;
        }

        if (HaveInteractGate)
            Teleport(CurrentTriggerTeleportGate.GetTargetPos);
    }

    private void OnEndItemEffect(ItemType itemType)
    {
        if (itemType == ItemType.Protection)
        {
            isProtected = false;
            characterPresenter.PlayProtectionEndEffect();
        }
    }

    private void OnStartItemEffect(ItemType itemType)
    {
        if (itemType == ItemType.Protection)
        {
            isProtected = true;
            characterPresenter.PlayProtectionEffect();
        }
    }

    private void OnUseItemOneTime(ItemType itemType)
    {
        if (itemType == ItemType.Shoes && CheckCanJump(characterSetting.SuperJumpForce))
        {
            Jump(characterSetting.SuperJumpForce);
            characterPresenter.PlaySuperJumpEffect();
        }
    }
}