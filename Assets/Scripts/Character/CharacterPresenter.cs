using System;
using SNShien.Common.AdapterTools;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

public class CharacterPresenter : ICharacterPresenter
{
    public Vector3 CurrentPos => selfRigidbody.position;
    public bool IsFaceRight { get; private set; }
    public IRigidbody2DAdapter GetRigidbody => characterView.GetRigidbody;
    private readonly ICharacterModel characterModel;
    private readonly IGameObjectPool gameObjectPool;
    private readonly IAudioManager audioManager;
    private readonly AfterimageEffectModel afterimageEffectModel;

    private ICharacterView characterView;
    private IRigidbody2DAdapter selfRigidbody;
    private bool isMoving;
    private bool isPlayWalkingAnimation;

    public CharacterPresenter(ICharacterModel characterModel, IGameObjectPool gameObjectPool, IAudioManager audioManager, IGameSetting gameSetting,
        IDeltaTimeGetter deltaTimeGetter)
    {
        this.characterModel = characterModel;
        this.gameObjectPool = gameObjectPool;
        this.audioManager = audioManager;
        afterimageEffectModel = new AfterimageEffectModel(gameObjectPool, gameSetting, deltaTimeGetter, this);
    }

    public void CallUpdate()
    {
        afterimageEffectModel.UpdateEffect();
    }

    public void BindView(ICharacterView view)
    {
        characterView = view;
        selfRigidbody = view.GetRigidbody;
        characterModel.BindPresenter(this);
        InitFaceDirection();
        InitState();
        RegisterEvent();
    }

    public void Jump()
    {
        gameObjectPool.SpawnGameObject(GameConst.PREFAB_NAME_JUMP_EFFECT, selfRigidbody.position);
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_JUMP);
    }

    public void PlayDieEffect(Action afterDieAnimationCallback, Action afterBackOriginCallback)
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_DAMAGE);
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_DIE);
        characterView.Waiting(1.5f, () =>
        {
            characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_NORMAL);
            afterDieAnimationCallback?.Invoke();
            characterView.Waiting(0.5f, () =>
            {
                afterBackOriginCallback?.Invoke();
                characterView.SetActive(true);
            });
        });
    }

    public void Teleport()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_TELEPORT);
    }

    public void PlayProtectionEffect()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_PROTECTION_BUFF);
        characterView.SetProtectionActive(true);
    }

    public void PlaySuperJumpEffect()
    {
        gameObjectPool.SpawnGameObject(GameConst.PREFAB_NAME_SUPER_JUMP_EFFECT, selfRigidbody.position);
        afterimageEffectModel.StartPlayEffect();
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_SUPER_JUMP);
    }

    public void PlayCollidePlatformEffect(bool isLanding)
    {
        if (isLanding)
            gameObjectPool.SpawnGameObject(GameConst.PREFAB_NAME_LANDING_EFFECT, selfRigidbody.position);

        afterimageEffectModel.ForceStop();
    }

    public void PlayBackToOriginEffect(Action callback)
    {
        characterView.Waiting(0.5f, () =>
        {
            callback?.Invoke();
        });
    }

    public void PlayerMoveEffect(float moveValue)
    {
        CheckChangeFaceDirection(moveValue);
        characterView.Translate(new Vector2(moveValue, 0));
        isMoving = moveValue != 0;

        ParseWalkingAnimation();
    }

    public void PlayExitHouseEffect(Action callback)
    {
        characterView.SetActive(true);
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_EXIT_HOUSE);
        characterView.Waiting(0.45f, () =>
        {
            callback?.Invoke();
        });
    }

    public void PlayEnterHouseEffect(Action callback)
    {
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_ENTER_HOUSE);
        characterView.Waiting(1, () =>
        {
            characterView.SetActive(false);
            callback?.Invoke();
        });
    }

    public void PlayProtectionEndEffect()
    {
        characterView.SetProtectionActive(false);
    }

    private void InitState()
    {
        characterView.PlayAnimation(GameConst.ANIMATION_KEY_CHARACTER_NORMAL);
        characterView.SetProtectionActive(false);
        characterView.SetActive(true);
    }

    private void InitFaceDirection()
    {
        IsFaceRight = true;
        characterView.SetFaceDirectionScale(1);
    }

    private void ParseWalkingAnimation()
    {
        if (isMoving && characterModel.CurrentCharacterState == CharacterState.Walking)
        {
            if (!isPlayWalkingAnimation)
            {
                isPlayWalkingAnimation = true;
                characterView.SetWalkAnimation(true);
                Debug.Log($"ParseWalkingAnimation: {isPlayWalkingAnimation}");
            }
        }
        else
        {
            if (isPlayWalkingAnimation)
            {
                isPlayWalkingAnimation = false;
                characterView.SetWalkAnimation(false);
                Debug.Log($"ParseWalkingAnimation: {isPlayWalkingAnimation}");
            }
        }
    }

    private void CheckChangeFaceDirection(float moveValue)
    {
        if (IsFaceRight && moveValue < 0)
        {
            IsFaceRight = false;
            characterView.SetFaceDirectionScale(-1);
        }
        else if (IsFaceRight == false && moveValue > 0)
        {
            IsFaceRight = true;
            characterView.SetFaceDirectionScale(1);
        }
    }

    private void RegisterEvent()
    {
        characterModel.OnChangeCharacterState -= OnChangeCharacterState;
        characterModel.OnChangeCharacterState += OnChangeCharacterState;
    }

    private void OnChangeCharacterState(CharacterState newCharacterState)
    {
        ParseWalkingAnimation();
    }
}