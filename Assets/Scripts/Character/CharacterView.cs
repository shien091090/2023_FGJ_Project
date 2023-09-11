using System;
using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;

public class CharacterView : MonoBehaviour, IRigidbody, ICharacterView
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float superJumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float jumpDelaySeconds;
    [SerializeField] private Transform footPoint;
    [SerializeField] private float footRadius;
    [SerializeField] private TeleportComponent teleportComponent;
    [SerializeField] private float interactDistance;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject go_protectionEffect;

    public float JumpForce => jumpForce;
    public float Speed => speed;
    public float JumpDelaySeconds => jumpDelaySeconds;
    public float InteractDistance => interactDistance;

    public Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector2 velocity
    {
        get => GetRigidbody.velocity;
        set => GetRigidbody.velocity = value;
    }

    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    private CharacterModel characterModel;

    // private bool isProtected;
    public bool IsFaceRight { get; private set; }

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

    public void PlayAnimation(string animationKey)
    {
        anim.Play(animationKey, 0, 0);
    }


    public void SetProtectionActive(bool isActive)
    {
        go_protectionEffect.SetActive(isActive);
    }

    public void Waiting(float seconds, Action callback)
    {
        StartCoroutine(Cor_WaitingCoroutine(seconds, callback));
    }

    public void SetSpriteFlix(bool flipX)
    {
        GetSpriteRenderer.flipX = flipX;
    }

    public void Translate(Vector3 moveVector)
    {
        transform.Translate(moveVector);
    }

    public void AddForce(Vector2 forceVector, ForceMode2D forceMode = ForceMode2D.Force)
    {
        GetRigidbody.AddForce(forceVector, forceMode);
    }

    private void Start()
    {
        characterModel = new CharacterModel(new CharacterMoveController(), new CharacterKeyController(), teleportComponent, this, FmodAudioManager.Instance,
            new TimeModel());
        characterModel.InitView(this);
        // characterModel.SetJumpDelay(JumpDelaySeconds);
        // characterModel.SetInteractDistance(InteractDistance);

        SetEventRegister();
    }

    private void Update()
    {
        characterModel.CallUpdate();
    }

    private void SetEventRegister()
    {
        // characterModel.OnHorizontalMove -= OnHorizontalMove;
        // characterModel.OnHorizontalMove += OnHorizontalMove;

        // characterModel.OnJump -= OnJump;
        // characterModel.OnJump += OnJump;

        ItemStateManager.Instance.OnUseItemOneTime -= OnUseItemOneTime;
        ItemStateManager.Instance.OnUseItemOneTime += OnUseItemOneTime;

        ItemStateManager.Instance.OnStartItemEffect -= OnStartItemEffect;
        ItemStateManager.Instance.OnStartItemEffect += OnStartItemEffect;

        ItemStateManager.Instance.OnEndItemEffect -= OnEndItemEffect;
        ItemStateManager.Instance.OnEndItemEffect += OnEndItemEffect;
    }

    private IEnumerator Cor_WaitingCoroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }


    private void OnEndItemEffect(ItemType itemType)
    {
        if (itemType == ItemType.Protection)
        {
            characterModel.isProtected = false;
            SetProtectionActive(false);
        }
    }

    private void OnStartItemEffect(ItemType itemType)
    {
        if (itemType == ItemType.Protection)
        {
            characterModel.isProtected = true;
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
        characterModel.ColliderTriggerEnter(new ColliderFacade(col));
        // if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Monster && characterModel.isProtected == false)
        // {
        //     IMonsterView monsterView = col.gameObject.GetComponent<IMonsterView>();
        //     if (monsterView == null || monsterView.CurrentState == MonsterState.Normal)
        //         characterModel.Die();
        //
        //     return;
        // }
        //
        // if (col.gameObject.layer != (int)GameConst.GameObjectLayerType.TeleportGate)
        //     return;
        //
        // TeleportGateComponent teleportGateComponent = col.gameObject.GetComponent<TeleportGateComponent>();
        // if (teleportGateComponent == null)
        //     return;
        //
        // characterModel.TriggerTeleportGate(teleportGateComponent);
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

    public void OnJump(float jumpForce)
    {
        FmodAudioManager.Instance.PlayOneShot("Jump");
        GetRigidbody.AddForce(new Vector2(0, jumpForce));
    }
}