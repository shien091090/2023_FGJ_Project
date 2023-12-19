public class AfterimageEffectModel : IAfterimageEffectModel
{
    private readonly IGameObjectPool gameObjectPool;
    private readonly IGameSetting gameSetting;
    private readonly ITimeModel timeModel;
    private readonly ICharacterModel characterModel;

    private float totalEffectTimer;
    private float frequencyEffectTimer;
    private bool isShowEffect;

    public AfterimageEffectModel(IGameObjectPool gameObjectPool, IGameSetting gameSetting, ITimeModel timeModel, CharacterModel characterModel)
    {
        this.gameObjectPool = gameObjectPool;
        this.gameSetting = gameSetting;
        this.timeModel = timeModel;
        this.characterModel = characterModel;
    }

    public void UpdateEffect()
    {
        if (isShowEffect == false)
            return;

        totalEffectTimer += timeModel.deltaTime;
        frequencyEffectTimer += timeModel.deltaTime;
        if (totalEffectTimer >= gameSetting.AutoDisappearSeconds)
        {
            isShowEffect = false;
            return;
        }

        if (frequencyEffectTimer >= gameSetting.SpawnEffectFrequency)
        {
            gameObjectPool.SpawnGameObject("AfterimageEffect", characterModel.CurrentPos, characterModel.IsFaceRight ?
                FaceDirection.Right :
                FaceDirection.Left);

            frequencyEffectTimer = 0;
        }
    }

    public void StartPlayEffect()
    {
        totalEffectTimer = 0;
        frequencyEffectTimer = 0;
        isShowEffect = true;
    }
}