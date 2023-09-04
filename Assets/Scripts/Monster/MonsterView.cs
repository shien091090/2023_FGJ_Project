using System;
using UnityEngine;

public class MonsterView : MonoBehaviour
{
    private const string ANIM_KEY_STUN_FORMAT = "monster_{0}_stun";
    private const string ANIM_KEY_NORMAL_FORMAT = "monster_{0}_movable";

    [SerializeField] private GameObject go_stunStateEffect;
    [SerializeField] private string monsterType;
    [SerializeField] private float keepStunTime;

    private MonsterModel monsterModel;
    private Animator anim;

    public string GetStunAnimKey => string.Format(ANIM_KEY_STUN_FORMAT, monsterType);
    public string GetMovableAnimKey => string.Format(ANIM_KEY_NORMAL_FORMAT, monsterType);

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
        }
    }
}