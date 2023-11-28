using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerRecordModel
{
    private const string API_GET_RECORD = "api_get_record";
    private const string EMPTY_TEXT = "-";

    private static PlayerRecordModel _instance;

    private readonly ServerCommunicator serverCommunicator;
    private readonly LoadingIndicatorModel loadingIndicatorModel;

    private List<PlayerRecord> playerRecordData;
    private IPlayerRecordView view;

    public static PlayerRecordModel Instance => _instance;

    public PlayerRecordModel(ServerCommunicator serverCommunicator, LoadingIndicatorModel loadingIndicatorModel)
    {
        this.serverCommunicator = serverCommunicator;
        this.loadingIndicatorModel = loadingIndicatorModel;
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

    public void Open(bool isRequestRecord = false)
    {
        if (isRequestRecord)
            RequestPlayerRecord(view.UpdateView);
        else
        {
            if (serverCommunicator.IsWaitingResponse)
            {
                loadingIndicatorModel.Open();
                serverCommunicator.OnRequestCompleted -= OnRequestCompleted;
                serverCommunicator.OnRequestCompleted += OnRequestCompleted;
            }
            else
                view.UpdateView();
        }
    }

    public void BindView(IPlayerRecordView view)
    {
        this.view = view;
    }

    private void UpdatePlayerRecord(List<PlayerRecord> playerRecords)
    {
        if (playerRecords == null || playerRecords.Count == 0)
            return;

        playerRecordData = playerRecords.OrderBy(x => x.costTimeSeconds).ToList();
    }

    private void OnRequestCompleted()
    {
        serverCommunicator.OnRequestCompleted -= OnRequestCompleted;

        loadingIndicatorModel.Close();
        view.UpdateView();
    }
}