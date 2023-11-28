using UnityEngine;

public class StartSceneInstaller : MonoBehaviour
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
}