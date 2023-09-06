using System;
using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;

public class CharacterView : MonoBehaviour, ITransform
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float superJumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float jumpDelaySeconds;
    [SerializeField] private Transform footPoint;
    [SerializeField] private float footRadius;
    [SerializeField] private TeleportComponent teleportComponent;
    [SerializeField] private float fallDownToOriginPosTime;
    [SerializeField] private float interactDistance;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject go_protectionEffect;

    public Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private CharacterModel characterModel;
    private bool isFaceRight;
    private bool isDying;
    private bool isProtected;

    public SpriteRenderer GetSpriteRenderer
    {
        get
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            return spriteRenderer;
        }
    }

    private Rigidbody2D GetRigidbody
    {
        get
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody2D>();

            return rigidbody;
        }
    }

    private void Start()
    {
        characterModel = new CharacterModel(new CharacterMoveController(), new CharacterKeyController(), teleportComponent, this);
        characterModel.SetJumpDelay(jumpDelaySeconds);
        characterModel.SetFallDownTime(fallDownToOriginPosTime);
        characterModel.SetInteractDistance(interactDistance);

        SetEventRegister();
        InitState();
    }

    private void Update()
    {
        if (isDying)
            return;

        characterModel.UpdateJumpTimer(Time.deltaTime);
        characterModel.UpdateCheckJump(jumpForce);
        characterModel.UpdateMove(Time.deltaTime, speed);
        // characterModel.UpdateFallDownTimer(Time.deltaTime);
        characterModel.UpdateCheckInteract();
    }

    private void InitState()
    {
        anim.Play("character_normal", 0);
        isDying = false;
        isProtected = false;
        isFaceRight = true;
        SetProtectionActive(false);
    }

    private void SetProtectionActive(bool isActive)
    {
        go_protectionEffect.SetActive(isActive);
    }

    private void SetEventRegister()
    {
        characterModel.OnHorizontalMove -= OnHorizontalMove;
        characterModel.OnHorizontalMove += OnHorizontalMove;

        characterModel.OnJump -= OnJump;
        characterModel.OnJump += OnJump;

        ItemStateManager.Instance.OnUseItemOneTime -= OnUseItemOneTime;
        ItemStateManager.Instance.OnUseItemOneTime += OnUseItemOneTime;

        ItemStateManager.Instance.OnStartItemEffect -= OnStartItemEffect;
        ItemStateManager.Instance.OnStartItemEffect += OnStartItemEffect;

        ItemStateManager.Instance.OnEndItemEffect -= OnEndItemEffect;
        ItemStateManager.Instance.OnEndItemEffect += OnEndItemEffect;
    }

    private void CheckChangeDirection(float moveValue)
    {
        if (isFaceRight && moveValue < 0)
        {
            isFaceRight = false;
            GetSpriteRenderer.flipX = true;
        }
        else if (isFaceRight == false && moveValue > 0)
        {
            isFaceRight = true;
            GetSpriteRenderer.flipX = false;
        }
    }

    private void Die()
    {
        if (isDying)
            return;

        StartCoroutine(Cor_Die());
    }

    private IEnumerator Cor_Die()
    {
        isDying = true;
        FmodAudioManager.Instance.PlayOneShot("Damage");
        anim.Play("character_die", 0);

        yield return new WaitForSeconds(1.5f);

        anim.Play("character_normal", 0);
        teleportComponent.BackToOrigin();
        isDying = false;
        isProtected = false;
    }

    private void OnEndItemEffect(ItemType itemType)
    {
        if (itemType == ItemType.Protection)
        {
            isProtected = false;
            SetProtectionActive(false);
        }
    }

    private void OnStartItemEffect(ItemType itemType)
    {
        if (itemType == ItemType.Protection)
        {
            isProtected = true;
            SetProtectionActive(true);
        }
    }

    private void OnUseItemOneTime(ItemType itemType)
    {
        if (itemType == ItemType.Shoes)
            characterModel.Jump(superJumpForce);
    }

    public void OnCollisionStay2D(Collision2D col)
    {
        bool isOnFloor = Physics2D.OverlapCircle(footPoint.position, footRadius, LayerMask.GetMask(GameConst.GameObjectLayerType.Platform.ToString()));

        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Platform && isOnFloor)
            characterModel.TriggerFloor();
    }

    public void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Platform)
        {
            characterModel.ExitFloor();
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Monster && isProtected == false)
        {
            MonsterView monsterView = col.gameObject.GetComponent<MonsterView>();
            if (monsterView == null || monsterView.CurrentState == MonsterState.Normal)
                Die();

            return;
        }

        if (col.gameObject.layer != (int)GameConst.GameObjectLayerType.TeleportGate)
            return;

        TeleportGateComponent teleportGateComponent = col.gameObject.GetComponent<TeleportGateComponent>();
        if (teleportGateComponent == null)
            return;

        characterModel.TriggerTeleportGate(teleportGateComponent);
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer != (int)GameConst.GameObjectLayerType.TeleportGate)
            return;

        TeleportGateComponent teleportGateComponent = col.gameObject.GetComponent<TeleportGateComponent>();
        if (teleportGateComponent == null)
            return;

        characterModel.ExitTeleportGate();
    }

    private void OnJump(float jumpForce)
    {
        FmodAudioManager.Instance.PlayOneShot("Jump");
        GetRigidbody.AddForce(new Vector2(0, jumpForce));
    }

    private void OnHorizontalMove(float moveValue)
    {
        CheckChangeDirection(moveValue);
        transform.Translate(new Vector2(moveValue, 0));
    }
}