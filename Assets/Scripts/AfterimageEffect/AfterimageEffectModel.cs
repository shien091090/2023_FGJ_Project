using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

public class AfterimageEffectModel : IAfterimageEffectModel
{
    private readonly IGameObjectPool gameObjectPool;
    private readonly IGameSetting gameSetting;
    private readonly IDeltaTimeGetter deltaTimeGetter;
    private readonly ICharacterModel characterModel;

    private float totalEffectTimer;
    private float frequencyEffectTimer;
    private bool isShowEffect;

    public AfterimageEffectModel(IGameObjectPool gameObjectPool, IGameSetting gameSetting, IDeltaTimeGetter deltaTimeGetter, CharacterModel characterModel)
    {
        this.gameObjectPool = gameObjectPool;
        this.gameSetting = gameSetting;
        this.deltaTimeGetter = deltaTimeGetter;
        this.characterModel = characterModel;
    }

    public void UpdateEffect()
    {
        if (isShowEffect == false)
            return;

        totalEffectTimer += deltaTimeGetter.deltaTime;
        frequencyEffectTimer += deltaTimeGetter.deltaTime;
        if (totalEffectTimer >= gameSetting.AutoDisappearSeconds)
        {
            isShowEffect = false;
            return;
        }

        if (frequencyEffectTimer >= gameSetting.SpawnEffectFrequency)
        {
            gameObjectPool.SpawnGameObject("AfterimageEffect", characterModel.CurrentPos, new Vector3(characterModel.IsFaceRight ?
                1 :
                -1, 1, 1));
            frequencyEffectTimer = 0;
        }
    }

    public void StartPlayEffect()
    {
        totalEffectTimer = 0;
        frequencyEffectTimer = 0;
        isShowEffect = true;
    }

    public void ForceStop()
    {
        totalEffectTimer = 0;
        frequencyEffectTimer = 0;
        isShowEffect = false;
    }
}