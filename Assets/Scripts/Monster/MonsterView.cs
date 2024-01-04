using System;
using SNShien.Common.AudioTools;
using UnityEngine;
using Zenject;

public class MonsterView : MonoBehaviour, IMonsterView
{
    private const string ANIM_KEY_STUN_FORMAT = "monster_{0}_stun";
    private const string ANIM_KEY_NORMAL_FORMAT = "monster_{0}_movable";

    [SerializeField] private GameObject go_stunStateEffect;
    [SerializeField] private Animator anim_hitEffect;
    [SerializeField] private string monsterType;
    [SerializeField] private float keepStunTime;
    
    [Inject]private ICharacterModel characterModel;
    
    public MonsterState CurrentState => monsterModel.CurrentState;

    private MonsterModel monsterModel;
    private Animator anim;
    private MissingTexturePart missingTexturePart;

    public string GetStunAnimKey => string.Format(ANIM_KEY_STUN_FORMAT, monsterType);
    public string GetMovableAnimKey => string.Format(ANIM_KEY_NORMAL_FORMAT, monsterType);

    private MissingTexturePart GetMissingTexturePart
    {
        get
        {
            if (missingTexturePart == null)
                missingTexturePart = GetComponent<MissingTexturePart>();

            return missingTexturePart;
        }
    }

    private Animator GetAnim
    {
        get
        {
            if (anim == null)
                anim = GetComponent<Animator>();

            return anim;
        }
    }

    private void Update()
    {
        monsterModel.UpdateStunTimer(Time.deltaTime);
    }

    private void Start()
    {
        monsterModel = new MonsterModel(keepStunTime);

        monsterModel.OnChangeState -= OnChangeState;
        monsterModel.OnChangeState += OnChangeState;

        OnChangeState(monsterModel.CurrentState);
    }

    private void SetStunEffectActive(bool isActive)
    {
        go_stunStateEffect.SetActive(isActive);
    }

    private void RefreshAnimationState(MonsterState state)
    {
        switch (state)
        {
            case MonsterState.Normal:
                GetAnim.Play(GetMovableAnimKey, 0);
                break;

            case MonsterState.Stun:
                GetAnim.Play(GetStunAnimKey, 0);
                break;
        }
    }

    private void OnChangeState(MonsterState state)
    {
        SetStunEffectActive(state == MonsterState.Stun);
        RefreshAnimationState(state);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Weapon)
        {
            monsterModel.BeAttack();

            float distance = Vector2.Distance(transform.position, characterModel.CurrentPos);
            Debug.Log("distance: " + distance + "");
            if (distance <= 10)
            {
                FmodAudioManager.Instance.PlayOneShot(GameConst.AUDIO_KEY_HIT_MONSTER);
                bool isHitFromLeft = col.transform.localRotation.z > 0;
                anim_hitEffect.transform.localScale = new Vector3(isHitFromLeft ?
                    -1 :
                    1, 1, 1);
                anim_hitEffect.Play("hit_effect", 0, 0);
            }

            GetMissingTexturePart.ClearMissingTexture();
        }
    }
}