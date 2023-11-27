using SNShien.Common.AudioTools;
using UnityEngine;

public class GameEventHandler : MonoBehaviour
{
    private static GameEventHandler _instance;

    private CharacterModel characterModel;
    private CharacterMoveController characterMoveController;
    private CharacterKeyController characterKeyController;
    private TimeModel timeModel;
    private IItemTriggerHandler itemTriggerHandler;
    private BulletHandlerModel bulletHandlerModel;

    public static GameEventHandler Instance => _instance;

    private void Init()
    {
        characterMoveController = new CharacterMoveController();
        characterKeyController = new CharacterKeyController();
        timeModel = new TimeModel();
        itemTriggerHandler = new ItemTriggerHandler();
        characterModel = new CharacterModel(characterMoveController, characterKeyController, FmodAudioManager.Instance, timeModel, itemTriggerHandler);
        bulletHandlerModel = new BulletHandlerModel(itemTriggerHandler, characterModel);
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        Init();
    }
}