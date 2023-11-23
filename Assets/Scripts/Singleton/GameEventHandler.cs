using SNShien.Common.AudioTools;
using UnityEngine;

public class GameEventHandler : MonoBehaviour
{
    private static GameEventHandler _instance;

    private CharacterModel characterModel;
    private CharacterMoveController characterMoveController;
    private CharacterKeyController characterKeyController;
    private TimeModel timeModel;

    public static GameEventHandler Instance => _instance;

    private void Init()
    {
        characterMoveController = new CharacterMoveController();
        characterKeyController = new CharacterKeyController();
        timeModel = new TimeModel();
        characterModel = new CharacterModel(characterMoveController, characterKeyController, FmodAudioManager.Instance, timeModel);
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        Init();
    }
}