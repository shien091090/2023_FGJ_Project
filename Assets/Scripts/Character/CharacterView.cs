using System;
using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;

public class CharacterView : MonoBehaviour, ICharacterView
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float superJumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float jumpDelaySeconds;
    [SerializeField] private float footRadius;
    [SerializeField] private float interactDistance;
    [SerializeField] private float fallDownLimitHeight;
    [SerializeField] private Transform footPoint;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject go_protectionEffect;
    [SerializeField] private ColliderComponent footColliderComponent;
    [SerializeField] private ColliderComponent rightColliderComponent;
    [SerializeField] private ColliderComponent leftColliderComponent;
    [SerializeField] private RigidBody2DComponent rigidBodyComponent;

    public float JumpForce => jumpForce;
    public float Speed => speed;
    public float JumpDelaySeconds => jumpDelaySeconds;
    public float InteractDistance => interactDistance;
    public Vector3 FootPointPosition => footPoint.position;
    public float FootRadius => footRadius;
    public float FallDownLimitPosY => fallDownLimitHeight;

    private SpriteRenderer spriteRenderer;
    private CharacterModel characterModel;
    private WallColliderHandler rightWallColliderHandler;
    private WallColliderHandler leftWallColliderHandler;

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

    public void SetSpriteFlipX(bool flipX)
    {
        GetSpriteRenderer.flipX = flipX;
    }

    public void Translate(Vector3 moveVector)
    {
        transform.Translate(moveVector);
    }

    private void Start()
    {
        characterModel = new CharacterModel(new CharacterMoveController(), new CharacterKeyController(), rigidBodyComponent, FmodAudioManager.Instance,
            new TimeModel());

        rightWallColliderHandler = new WallColliderHandler(true, characterModel);
        leftWallColliderHandler = new WallColliderHandler(false,characterModel);

        characterModel.InitView(this);
        footColliderComponent.InitHandler(characterModel);
        rightColliderComponent.InitHandler(rightWallColliderHandler);
        leftColliderComponent.InitHandler(leftWallColliderHandler);
        SetEventRegister();
    }

    private void Update()
    {
        characterModel.CallUpdate();
    }

    private void SetEventRegister()
    {
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
}