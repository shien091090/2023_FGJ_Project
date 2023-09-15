using System;
using SNShien.Common.AudioTools;
using UnityEngine;

public class CharacterModel : IColliderHandler
{
    public bool isProtected;
    private readonly IMoveController moveController;
    private readonly IKeyController keyController;
    private readonly ITeleport teleport;
    private readonly IRigidbody selfRigidbody;
    private readonly IAudioManager audioManager;
    private readonly ITimeModel timeModel;
    private ICharacterView characterView;
    private float jumpTimer;
    public bool IsJumping { get; private set; }
    public bool HaveInteractGate => CurrentTriggerTeleportGate != null;
    public Vector3 RecordOriginPos { get; private set; }
    public bool IsDying { get; set; }
    private ITeleportGate CurrentTriggerTeleportGate { get; set; }
    private bool IsStayOnFloor { get; set; }
    private bool IsFaceRight { get; set; }

    public CharacterModel(IMoveController moveController, IKeyController keyController, ITeleport teleport, IRigidbody characterRigidbody, IAudioManager audioManager,
        ITimeModel timeModel)
    {
        this.moveController = moveController;
        this.keyController = keyController;
        this.teleport = teleport;
        this.audioManager = audioManager;
        this.timeModel = timeModel;
        selfRigidbody = characterRigidbody;
    }

    public void ColliderTriggerEnter(ICollider col)
    {
        if (col.Layer != (int)GameConst.GameObjectLayerType.TeleportGate)
            return;

        ITeleportGate teleportGateComponent = col.GetComponent<ITeleportGate>();
        if (teleportGateComponent == null)
            return;

        CurrentTriggerTeleportGate = teleportGateComponent;
    }

    public void ColliderTriggerExit(ICollider col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.TeleportGate)
        {
            ITeleportGate teleportGateComponent = col.GetComponent<ITeleportGate>();
            if (teleportGateComponent == null)
                return;

            CurrentTriggerTeleportGate = null;
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
    }

    private void InitState()
    {
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_NORMAL);
        jumpTimer = characterView.JumpDelaySeconds;
        IsDying = false;
        isProtected = false;
        IsFaceRight = true;
        IsStayOnFloor = true;
        RecordOriginPos = selfRigidbody.position;
        characterView.SetProtectionActive(false);
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
        UpdateCheckJump(characterView.JumpForce);
        UpdateMove(timeModel.deltaTime, characterView.Speed);
        UpdateCheckInteract();
    }

    public void Jump(float jumpForce)
    {
        if (IsJumping || IsStayOnFloor == false)
            return;

        if (jumpForce == 0)
            return;

        jumpTimer = 0;
        IsJumping = true;
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_JUMP);
        selfRigidbody.AddForce(new Vector2(0, jumpForce));
    }

    public void Die()
    {
        if (IsDying)
            return;

        IsDying = true;
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_DAMAGE);
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_DIE);
        characterView.Waiting(1.5f, () =>
        {
            characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_NORMAL);
            teleport.BackToOrigin();
            characterView.Waiting(0.5f, () =>
            {
                IsDying = false;
                isProtected = false;
            });
        });
    }

    public void BackToOrigin()
    {
        IsDying = true;
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_TELEPORT);
        selfRigidbody.position = RecordOriginPos;
        selfRigidbody.velocity = Vector2.zero;
        characterView.Waiting(0.5f, () =>
        {
            IsDying = false;
        });
    }

    private void CheckChangeDirection(float moveValue)
    {
        if (IsFaceRight && moveValue < 0)
        {
            IsFaceRight = false;
            characterView.SetSpriteFlix(true);
        }
        else if (IsFaceRight == false && moveValue > 0)
        {
            IsFaceRight = true;
            characterView.SetSpriteFlix(false);
        }
    }

    private void UpdateMove(float deltaTime, float speed)
    {
        float moveValue = moveController.GetHorizontalAxis() * deltaTime * speed;
        CheckChangeDirection(moveValue);
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

        if (HaveInteractGate == false)
            return;

        float distance = Vector3.Distance(selfRigidbody.position, CurrentTriggerTeleportGate.GetPos);
        if (distance > characterView.InteractDistance)
        {
            CurrentTriggerTeleportGate = null;
            return;
        }

        if (HaveInteractGate)
            CurrentTriggerTeleportGate.Teleport(selfRigidbody);
    }
}