using System;
using System.Collections;
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
    [SerializeField] private Transform tf_faceDirection;

    public float JumpForce => jumpForce;
    public float Speed => speed;
    public float SuperJumpForce => superJumpForce;
    public float JumpDelaySeconds => jumpDelaySeconds;
    public float InteractDistance => interactDistance;
    public float FallDownLimitPosY => fallDownLimitHeight;
    public IRigidbody GetRigidbody => rigidBodyComponent;

    private SpriteRenderer spriteRenderer;
    private CharacterModel characterModel;
    private WallColliderHandler rightWallColliderHandler;
    private WallColliderHandler leftWallColliderHandler;
    private IAfterimageEffectModel afterimageEffectModel;

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

    public void SetFaceDirectionScale(int scale)
    {
        tf_faceDirection.localScale = new Vector3(scale, 1, 1);
    }

    public void SetActive(bool isActive)
    {
        tf_faceDirection.gameObject.SetActive(isActive);
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

    public void Translate(Vector3 moveVector)
    {
        transform.Translate(moveVector);
    }

    private void Start()
    {
        characterModel = CharacterModel.Instance;
        afterimageEffectModel = AfterimageEffectModel.Instance;

        rightWallColliderHandler = new WallColliderHandler(true, characterModel);
        leftWallColliderHandler = new WallColliderHandler(false, characterModel);

        characterModel.BindView(this);
        footColliderComponent.InitHandler(characterModel);
        rightColliderComponent.InitHandler(rightWallColliderHandler);
        leftColliderComponent.InitHandler(leftWallColliderHandler);
    }

    private void Update()
    {
        characterModel.CallUpdate();
        afterimageEffectModel.UpdateEffect();
    }

    public void SetSpriteFlipX(bool flipX)
    {
        GetSpriteRenderer.flipX = flipX;
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