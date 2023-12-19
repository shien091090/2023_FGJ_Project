public class AfterimageEffectModel : IAfterimageEffectModel
{
    private static IAfterimageEffectModel _instance;
    private readonly ObjectPoolManager gameObjectPool;
    private readonly IGameSetting gameSetting;
    private readonly ITimeModel timeModel;

    private ICharacterModel characterModel;
    private float totalEffectTimer;
    private float frequencyEffectTimer;
    private bool isShowEffect;

    public static IAfterimageEffectModel Instance => _instance;

    public AfterimageEffectModel(ObjectPoolManager gameObjectPool, IGameSetting gameSetting, ITimeModel timeModel)
    {
        this.gameObjectPool = gameObjectPool;
        this.gameSetting = gameSetting;
        this.timeModel = timeModel;

        _instance = this;
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

    public void SetCharacter(ICharacterModel characterModel)
    {
        this.characterModel = characterModel;
    }
}