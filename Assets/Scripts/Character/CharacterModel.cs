using System;
using SNShien.Common.AudioTools;
using UnityEngine;

public class CharacterModel : IColliderHandler
{
    public bool isProtected;
    private readonly IMoveController moveController;
    private readonly IKeyController keyController;
    private readonly IRigidbody selfRigidbody;
    private readonly IAudioManager audioManager;
    private readonly ITimeModel timeModel;
    private readonly ICharacterEventHandler characterEventHandler;
    private ICharacterView characterView;
    private float jumpTimer;
    private bool isCollideRightWall;
    private bool isCollideLeftWall;
    private bool isFreeze;
    public bool IsJumping { get; private set; }
    public bool HaveInteractGate => CurrentTriggerTeleportGate != null;
    public bool HaveInteractSavePoint => CurrentTriggerSavePoint != null;
    public Vector3 RecordOriginPos { get; private set; }
    public bool IsFaceRight { get; private set; }
    private ITeleportGate CurrentTriggerTeleportGate { get; set; }
    private ISavePointView CurrentTriggerSavePoint { get; set; }
    private bool IsStayOnFloor { get; set; }

    public CharacterModel(IMoveController moveController, IKeyController keyController, IRigidbody characterRigidbody, IAudioManager audioManager,
        ITimeModel timeModel, ICharacterEventHandler characterEventHandler)
    {
        this.moveController = moveController;
        this.keyController = keyController;
        this.audioManager = audioManager;
        this.timeModel = timeModel;
        this.characterEventHandler = characterEventHandler;
        selfRigidbody = characterRigidbody;
    }

    public void ColliderTriggerEnter(ICollider col)
    {
        switch (col.Layer)
        {
            case (int)GameConst.GameObjectLayerType.TeleportGate:
                {
                    ITeleportGate teleportGateComponent = col.GetComponent<ITeleportGate>();
                    if (teleportGateComponent != null)
                        CurrentTriggerTeleportGate = teleportGateComponent;
                    break;
                }

            case (int)GameConst.GameObjectLayerType.SavePoint:
                {
                    if (characterEventHandler.CurrentCharacterState == CharacterState.IntoHouse)
                        return;

                    ISavePointView savePointComponent = col.GetComponent<ISavePointView>();
                    if (savePointComponent != null)
                    {
                        CurrentTriggerSavePoint = savePointComponent;
                        CurrentTriggerSavePoint.GetModel.ShowRecordStateHint();
                    }

                    break;
                }
        }
    }

    public void ColliderTriggerExit(ICollider col)
    {
        switch (col.Layer)
        {
            case (int)GameConst.GameObjectLayerType.TeleportGate:
                {
                    if (col.GetComponent<ITeleportGate>() != null)
                        CurrentTriggerTeleportGate = null;
                    break;
                }

            case (int)GameConst.GameObjectLayerType.SavePoint:
                {
                    if (col.GetComponent<ISavePointView>() != null &&
                        CurrentTriggerSavePoint != null &&
                        characterEventHandler.CurrentCharacterState != CharacterState.IntoHouse &&
                        isFreeze == false)
                    {
                        CurrentTriggerSavePoint.GetModel.HideAllUI();
                        CurrentTriggerSavePoint = null;
                    }

                    break;
                }
        }
    }

    public void ColliderTriggerStay(ICollider col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Monster && isProtected == false)
        {
            IMonsterView monsterView = col.GetComponent<IMonsterView>();
            if (monsterView == null || monsterView.CurrentState == MonsterState.Normal)
                Die();
        }
    }

    public void CollisionEnter(ICollision col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
        {
            IsStayOnFloor = true;
            IsJumping = false;
            if (characterEventHandler.CurrentCharacterState != CharacterState.Die &&
                characterEventHandler.CurrentCharacterState != CharacterState.IntoHouse)
                characterEventHandler.ChangeCurrentCharacterState(CharacterState.Walking);
        }
    }

    public void CollisionExit(ICollision col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
        {
            IsStayOnFloor = false;
            if (characterEventHandler.CurrentCharacterState != CharacterState.Die &&
                characterEventHandler.CurrentCharacterState != CharacterState.IntoHouse)
                characterEventHandler.ChangeCurrentCharacterState(CharacterState.Jumping);
        }
    }

    public void InitView(ICharacterView view)
    {
        characterView = view;
        InitState();
        InitFaceDirection();
    }

    private void InitFaceDirection()
    {
        IsFaceRight = true;
        // characterView.SetSpriteFlipX(false);
        characterView.SetFaceDirectionScale(1);
    }

    private void InitState()
    {
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_NORMAL);
        jumpTimer = characterView.JumpDelaySeconds;
        isProtected = false;
        IsStayOnFloor = true;
        RecordOriginPos = selfRigidbody.position;
        characterView.SetProtectionActive(false);
        characterView.SetActive(true);
        characterEventHandler.ChangeCurrentCharacterState(CharacterState.Walking);
    }

    public void CallUpdate()
    {
        if (characterEventHandler.CurrentCharacterState == CharacterState.Die)
            return;

        if (isFreeze)
            return;

        if (selfRigidbody.position.y <= characterView.FallDownLimitPosY)
        {
            BackToOrigin();
            return;
        }

        UpdateJumpTimer(timeModel.deltaTime);
        UpdateCheckSavePointTeleport();
        UpdateCheckJump(characterView.JumpForce);
        UpdateMove(timeModel.deltaTime, characterView.Speed);
        UpdateCheckInteract();
    }

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

    public void Jump(float jumpForce)
    {
        if (IsJumping || IsStayOnFloor == false)
            return;

        if (jumpForce == 0)
            return;

        jumpTimer = 0;
        IsJumping = true;
        characterEventHandler.ChangeCurrentCharacterState(CharacterState.Jumping);
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_JUMP);
        selfRigidbody.AddForce(new Vector2(0, jumpForce));
    }

    public void Die()
    {
        if (characterEventHandler.CurrentCharacterState == CharacterState.Die)
            return;

        characterEventHandler.ChangeCurrentCharacterState(CharacterState.Die);
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_DAMAGE);
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_DIE);
        characterView.Waiting(1.5f, () =>
        {
            characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_NORMAL);
            BackToOrigin();
            characterView.Waiting(0.5f, () =>
            {
                isProtected = false;
                characterEventHandler.ChangeCurrentCharacterState(CharacterState.Walking);
                characterView.SetActive(true);
            });
        });
    }

    public void BackToOrigin()
    {
        characterEventHandler.ChangeCurrentCharacterState(CharacterState.Die);
        Teleport(RecordOriginPos);
        characterView.Waiting(0.5f, () =>
        {
            characterEventHandler.ChangeCurrentCharacterState(CharacterState.Walking);
        });
    }

    private void CheckChangeFaceDirection(float moveValue)
    {
        if (IsFaceRight && moveValue < 0)
        {
            IsFaceRight = false;
            // characterView.SetSpriteFlipX(true);
            characterView.SetFaceDirectionScale(-1);
        }
        else if (IsFaceRight == false && moveValue > 0)
        {
            IsFaceRight = true;
            // characterView.SetSpriteFlipX(false);
            characterView.SetFaceDirectionScale(1);
        }
    }

    private void UpdateMove(float deltaTime, float speed)
    {
        if (characterEventHandler.CurrentCharacterState == CharacterState.IntoHouse)
            return;

        float horizontalAxis = moveController.GetHorizontalAxis();
        if ((horizontalAxis > 0 && isCollideRightWall) || (horizontalAxis < 0 && isCollideLeftWall))
            horizontalAxis = 0;

        float moveValue = horizontalAxis * deltaTime * speed;
        CheckChangeFaceDirection(moveValue);
        characterView.Translate(new Vector2(moveValue, 0));
    }

    private void UpdateCheckJump(float jumpForce)
    {
        if (characterEventHandler.CurrentCharacterState == CharacterState.IntoHouse)
            return;

        if (jumpTimer < characterView.JumpDelaySeconds)
            return;

        if (keyController.IsJumpKeyDown)
            Jump(jumpForce);
    }

    private void UpdateJumpTimer(float deltaTime)
    {
        if (jumpTimer >= characterView.JumpDelaySeconds)
            return;

        jumpTimer = Math.Min(jumpTimer + deltaTime, characterView.JumpDelaySeconds);
    }

    private void UpdateCheckSavePointTeleport()
    {
        if (characterEventHandler.CurrentCharacterState != CharacterState.IntoHouse ||
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

        if (characterEventHandler.CurrentCharacterState == CharacterState.IntoHouse)
        {
            isFreeze = true;
            characterView.SetActive(true);
            characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_EXIT_HOUSE);
            characterView.Waiting(0.45f, () =>
            {
                characterEventHandler.ChangeCurrentCharacterState(CharacterState.Walking);
                CurrentTriggerSavePoint.GetModel.HideInteractHint();
                isFreeze = false;
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

    private void Teleport(Vector3 targetPos)
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_TELEPORT);
        selfRigidbody.position = targetPos;
        selfRigidbody.velocity = Vector2.zero;
    }

    private void TriggerSavePoint()
    {
        isFreeze = true;
        CurrentTriggerSavePoint.GetModel.Save();
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_ENTER_HOUSE);
        characterView.Waiting(1, () =>
        {
            characterEventHandler.ChangeCurrentCharacterState(CharacterState.IntoHouse);
            characterView.SetActive(false);
            CurrentTriggerSavePoint.GetModel.ShowInteractHint();
            isFreeze = false;
        });
    }

    private void TriggerTeleportGate()
    {
        float distance = Vector3.Distance(selfRigidbody.position, CurrentTriggerTeleportGate.GetPos);
        if (distance > characterView.InteractDistance)
        {
            CurrentTriggerTeleportGate = null;
            return;
        }

        if (HaveInteractGate)
            Teleport(CurrentTriggerTeleportGate.GetTargetPos);
    }
}