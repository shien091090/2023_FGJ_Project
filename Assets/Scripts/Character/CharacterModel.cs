using System;
using System.Linq;
using SNShien.Common.AudioTools;
using Unity.VisualScripting;
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
    public bool IsJumping { get; private set; }
    public bool HaveInteractGate => CurrentTriggerTeleportGate != null;
    public bool HaveInteractSavePoint => CurrentTriggerSavePoint != null;
    public Vector3 RecordOriginPos { get; private set; }
    public bool IsDying { get; private set; }
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
                    ISavePointView savePointComponent = col.GetComponent<ISavePointView>();
                    if (savePointComponent != null)
                    {
                        CurrentTriggerSavePoint = savePointComponent;
                        CurrentTriggerSavePoint.ShowRecordStateHint();
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
                    if (col.GetComponent<ISavePointView>() != null && CurrentTriggerSavePoint != null)
                    {
                        CurrentTriggerSavePoint.HideAllUI();
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
            characterEventHandler.ChangeCurrentCharacterState(CharacterState.Walking);
        }
    }

    public void CollisionExit(ICollision col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
        {
            IsStayOnFloor = false;
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
        IsDying = false;
        isProtected = false;
        IsStayOnFloor = true;
        RecordOriginPos = selfRigidbody.position;
        characterView.SetProtectionActive(false);
        characterView.SetActive(true);
        characterEventHandler.ChangeCurrentCharacterState(CharacterState.Walking);
    }

    public void CallUpdate()
    {
        if (IsDying)
            return;

        if (selfRigidbody.position.y <= characterView.FallDownLimitPosY)
        {
            BackToOrigin();
            return;
        }

        UpdateJumpTimer(timeModel.deltaTime);
        
        if (characterEventHandler.CurrentCharacterState != CharacterState.IntoHouse)
        {
            UpdateCheckJump(characterView.JumpForce);
            UpdateMove(timeModel.deltaTime, characterView.Speed);
        }

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
        if (IsDying)
            return;

        IsDying = true;
        characterEventHandler.ChangeCurrentCharacterState(CharacterState.Die);
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_DAMAGE);
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_DIE);
        characterView.Waiting(1.5f, () =>
        {
            characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_NORMAL);
            BackToOrigin();
            characterView.Waiting(0.5f, () =>
            {
                IsDying = false;
                isProtected = false;
                characterEventHandler.ChangeCurrentCharacterState(CharacterState.Walking);
                characterView.SetActive(true);
            });
        });
    }

    public void BackToOrigin()
    {
        IsDying = true;
        characterEventHandler.ChangeCurrentCharacterState(CharacterState.Die);
        Teleport(RecordOriginPos);
        characterView.Waiting(0.5f, () =>
        {
            IsDying = false;
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
        float horizontalAxis = moveController.GetHorizontalAxis();
        if ((horizontalAxis > 0 && isCollideRightWall) || (horizontalAxis < 0 && isCollideLeftWall))
            horizontalAxis = 0;

        float moveValue = horizontalAxis * deltaTime * speed;
        CheckChangeFaceDirection(moveValue);
        characterView.Translate(new Vector2(moveValue, 0));
    }

    private void UpdateCheckJump(float jumpForce)
    {
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

    private void UpdateCheckInteract()
    {
        if (!keyController.IsInteractKeyDown)
            return;

        if (selfRigidbody == null)
            return;

        if (HaveInteractGate)
            TriggerTeleportGate();
        else if (HaveInteractSavePoint)
            TriggerSavePoint();
    }

    private void Teleport(Vector3 targetPos)
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_TELEPORT);
        selfRigidbody.position = targetPos;
        selfRigidbody.velocity = Vector2.zero;
    }

    private void TriggerSavePoint()
    {
        CurrentTriggerSavePoint.Save();
        characterEventHandler.ChangeCurrentCharacterState(CharacterState.IntoHouse);
        characterView.SetActive(false);
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