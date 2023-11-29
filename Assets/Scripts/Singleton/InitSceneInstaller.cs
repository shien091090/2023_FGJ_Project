using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneInstaller : MonoBehaviour
{
    [SerializeField] private ServerCommunicator serverCommunicator;

    private PlayerRecordModel playerRecordModel;
    private LoadingIndicatorModel loadingIndicatorModel;

    private void Init()
    {
        loadingIndicatorModel = new LoadingIndicatorModel();
        playerRecordModel = new PlayerRecordModel(serverCommunicator, loadingIndicatorModel);
    }

    private void Awake()
    {
        Init();
        playerRecordModel.RequestPlayerRecord();
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