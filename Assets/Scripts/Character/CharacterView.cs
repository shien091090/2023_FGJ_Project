using System;
using System.Collections;
using SNShien.Common.AdapterTools;
using UnityEngine;
using Zenject;

public class CharacterView : MonoBehaviour, ICharacterView
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject go_protectionEffect;
    [SerializeField] private Collider2DAdapterComponent footColliderComponent;
    [SerializeField] private Collider2DAdapterComponent rightColliderComponent;
    [SerializeField] private Collider2DAdapterComponent leftColliderComponent;
    [SerializeField] private RigidBody2DAdapterComponent rigidBodyComponent;
    [SerializeField] private Transform tf_faceDirection;

    [Inject] private ICharacterModel characterModel;
    [Inject] private ICharacterPresenter characterPresenter;

    public IRigidbody2DAdapter GetRigidbody => rigidBodyComponent;
    
    private SpriteRenderer spriteRenderer;
    private WallColliderHandler rightWallColliderHandler;
    private WallColliderHandler leftWallColliderHandler;

    public void SetFaceDirectionScale(int scale)
    {
        tf_faceDirection.localScale = new Vector3(scale, 1, 1);
    }

    public void SetActive(bool isActive)
    {
        tf_faceDirection.gameObject.SetActive(isActive);
    }

    public void SetWalkAnimation(bool isWaling)
    {
        anim.SetBool("isWalking", isWaling);
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
        rightWallColliderHandler = new WallColliderHandler(true, characterModel);
        leftWallColliderHandler = new WallColliderHandler(false, characterModel);

        footColliderComponent.InitHandler(characterModel);
        rightColliderComponent.InitHandler(rightWallColliderHandler);
        leftColliderComponent.InitHandler(leftWallColliderHandler);

        characterPresenter.BindView(this);
    }

    private void Update()
    {
        characterModel.CallUpdate();
        characterPresenter.CallUpdate();
    }

    private IEnumerator Cor_WaitingCoroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}