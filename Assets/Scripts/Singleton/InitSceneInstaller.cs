using System;
using SNShien.Common.AudioTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneInstaller : MonoBehaviour
{
    [SerializeField] private ServerCommunicator serverCommunicator;
    [SerializeField] private FmodAudioManager fmodAudioManager;

    private PlayerRecordModel playerRecordModel;
    private LoadingIndicatorModel loadingIndicatorModel;

    private void Init()
    {
        loadingIndicatorModel = new LoadingIndicatorModel();
        playerRecordModel = new PlayerRecordModel(serverCommunicator, loadingIndicatorModel, fmodAudioManager);
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        GotoStartScene();
    }

    private void GotoStartScene()
    {
        SceneManager.LoadScene("StartScene", LoadSceneMode.Additive);
    }
}