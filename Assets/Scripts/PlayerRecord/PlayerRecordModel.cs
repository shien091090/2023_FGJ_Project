using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.AudioTools;

public class PlayerRecordModel
{
    private const string API_GET_RECORD = "api_get_record";
    private const string EMPTY_TEXT = "-";

    private static PlayerRecordModel _instance;

    private readonly ServerCommunicator serverCommunicator;
    private readonly LoadingIndicatorModel loadingIndicatorModel;
    private readonly IAudioManager audioManager;

    private List<PlayerRecord> playerRecordData;
    private IPlayerRecordView view;

    public static PlayerRecordModel Instance => _instance;
    public bool IsViewOpening { get; private set; }

    public PlayerRecordModel(ServerCommunicator serverCommunicator, LoadingIndicatorModel loadingIndicatorModel, IAudioManager audioManager)
    {
        this.serverCommunicator = serverCommunicator;
        this.loadingIndicatorModel = loadingIndicatorModel;
        this.audioManager = audioManager;

        playerRecordData = new List<PlayerRecord>();

        _instance = this;
    }

    public string GetRecordPlayerName(int rankNumber)
    {
        return rankNumber >= playerRecordData.Count ?
            EMPTY_TEXT :
            playerRecordData[rankNumber].playerName;
    }

    public string GetRecordCostTime(int rankNumber)
    {
        if (rankNumber >= playerRecordData.Count)
            return EMPTY_TEXT;
        else
        {
            int costTimeSeconds = playerRecordData[rankNumber].costTimeSeconds;
            int minutes = costTimeSeconds / 60;
            int seconds = costTimeSeconds % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }

    public void RequestPlayerRecord(Action callback = null)
    {
        serverCommunicator.CreatePostRequest(API_GET_RECORD)
            .SendRequest<PlayerRecordResponse>((res) =>
            {
                if (res.statusCode == ServerResponse.STATUS_CODE_SUCCESS)
                    UpdatePlayerRecord(res.playerRecordList);

                callback?.Invoke();
            });
    }

    public void RequestOpen(bool isRequestRecord = false)
    {
        if (isRequestRecord)
            RequestPlayerRecord(OpenView);
        else
        {
            if (serverCommunicator.IsWaitingResponse)
            {
                loadingIndicatorModel.Open();
                serverCommunicator.OnRequestCompleted -= OnRequestCompleted;
                serverCommunicator.OnRequestCompleted += OnRequestCompleted;
            }
            else
                OpenView();
        }
    }

    public void BindView(IPlayerRecordView view)
    {
        this.view = view;
    }

    public void CloseView()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_BUTTON_CLICK);
        view.SetActive(false);
        IsViewOpening = false;
    }

    private void UpdatePlayerRecord(List<PlayerRecord> playerRecords)
    {
        if (playerRecords == null || playerRecords.Count == 0)
            return;

        playerRecordData = playerRecords.OrderBy(x => x.costTimeSeconds).ToList();
    }

    private void OpenView()
    {
        view.SetActive(true);
        view.UpdateView();
        IsViewOpening = true;
    }

    private void OnRequestCompleted()
    {
        serverCommunicator.OnRequestCompleted -= OnRequestCompleted;

        loadingIndicatorModel.Close();
        OpenView();
    }
}