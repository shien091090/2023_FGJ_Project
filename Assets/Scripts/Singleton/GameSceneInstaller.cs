using SNShien.Common.AudioTools;
using UnityEngine;

public class GameSceneInstaller : MonoBehaviour
{
    [SerializeField] private GameSettingScriptableObject gameSetting;
    [SerializeField] private ObjectPoolManager gameObjectPool;

    private CharacterModel characterModel;
    private CharacterMoveController characterMoveController;
    private CharacterKeyController characterKeyController;
    private TimeModel timeModel;
    private IItemTriggerHandler itemTriggerHandler;
    private BulletHandlerModel bulletHandlerModel;
    private MissingTextureManager missingTextureManager;
    private TileRemoverModel tileRemoverModel;
    private AfterimageEffectModel afterimageEffectModel;

    private void Init()
    {
        characterMoveController = new CharacterMoveController();
        characterKeyController = new CharacterKeyController();
        timeModel = new TimeModel();
        itemTriggerHandler = new ItemTriggerHandler();
        bulletHandlerModel = new BulletHandlerModel(itemTriggerHandler, characterModel, gameObjectPool, FmodAudioManager.Instance);
        missingTextureManager = new MissingTextureManager(gameSetting);
        tileRemoverModel = new TileRemoverModel(gameSetting);
        afterimageEffectModel = new AfterimageEffectModel(gameObjectPool, gameSetting, timeModel);
        characterModel = new CharacterModel(characterMoveController, characterKeyController, FmodAudioManager.Instance, timeModel, itemTriggerHandler, gameObjectPool,
            afterimageEffectModel);
        afterimageEffectModel.SetCharacter(characterModel);
    }

    private void Awake()
    {
        Init();
    }
}