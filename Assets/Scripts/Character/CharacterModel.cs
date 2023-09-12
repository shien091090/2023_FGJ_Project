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
    private ICharacterView characterView;

    private float jumpTimer;
    private ITimeModel timeModel;

    private ITeleportGate CurrentTriggerTeleportGate { get; set; }
    public bool IsJumping { get; private set; }
    private bool IsStayOnFloor { get; set; }
    public bool HaveInteractGate => CurrentTriggerTeleportGate != null;
    private bool IsDying { get; set; }
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
        if (col.Layer == (int)GameConst.GameObjectLayerType.Monster && isProtected == false)
        {
            IMonsterView monsterView = col.GetComponent<IMonsterView>();
            if (monsterView == null || monsterView.CurrentState == MonsterState.Normal)
                Die();

            return;
        }

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

    public void CollisionEnter(ICollision col)
    {
        bool isOnFloor = col.CheckPhysicsOverlapCircle(characterView.FootPointPosition, characterView.FootRadius, GameConst.GameObjectLayerType.Platform);
        Debug.Log($"CollisionEnter, isOnFloor: {isOnFloor}, col.Layer: {col.Layer}");
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform && isOnFloor)
        {
            IsStayOnFloor = true;
            IsJumping = false;
            Debug.Log($"isStayOnFloor: {IsStayOnFloor}, isJumping: {IsJumping}");
        }
    }

    public void CollisionExit(ICollision col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
        {
            Debug.Log("CollisionExit");
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
        characterView.PlayAnimation("character_normal");
        jumpTimer = characterView.JumpDelaySeconds;
        IsDying = false;
        isProtected = false;
        IsFaceRight = true;
        IsStayOnFloor = true;
        characterView.SetProtectionActive(false);
    }

    public void CallUpdate()
    {
        if (IsDying)
            return;

        UpdateJumpTimer(timeModel.deltaTime);
        UpdateCheckJump(characterView.JumpForce);
        UpdateMove(timeModel.deltaTime, characterView.Speed);
        UpdateCheckInteract();
    }

    public void Jump(float jumpForce)
    {
        if (IsJumping || IsStayOnFloor == false)
        {
            Debug.Log($"Jumping: {IsJumping}, StayOnFloor: {IsStayOnFloor}");
            return;
        }

        if (jumpForce == 0)
        {
            Debug.Log("JumpForce is 0");
            return;
        }

        jumpTimer = 0;
        IsJumping = true;
        audioManager.PlayOneShot("Jump");
        selfRigidbody.AddForce(new Vector2(0, jumpForce));
    }

    public void Die()
    {
        if (IsDying)
            return;

        IsDying = true;
        audioManager.PlayOneShot("Damage");
        characterView.PlayAnimation("character_die");
        characterView.Waiting(1.5f, () =>
        {
            characterView.PlayAnimation("character_normal");
            teleport.BackToOrigin();
            characterView.Waiting(0.5f, () =>
            {
                IsDying = false;
                isProtected = false;
            });
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
        {
            Debug.Log($"jumpTimer: {jumpTimer}, characterView.JumpDelaySeconds: {characterView.JumpDelaySeconds}");
            return;
        }

        if (keyController.IsJumpKeyDown)
        {
            Debug.Log("Jump");
            Jump(jumpForce);
        }
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