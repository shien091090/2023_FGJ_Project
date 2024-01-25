using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.AudioTools;
using SNShien.Common.NetworkTools;

public class PlayerRecordModel : IPlayerRecordModel
{
    private const string API_GET_RECORD = "api_get_record";
    private const string EMPTY_TEXT = "-";
    private const int PLAYER_RECORD_RANKING_LIMIT = 7;

    public bool IsViewOpening { get; private set; }

    private readonly ServerCommunicator serverCommunicator;
    private readonly ILoadingIndicatorModel loadingIndicatorModel;
    private readonly IAudioManager audioManager;
    private readonly IGlobalStateModel globalStateModel;

    private List<PlayerRecord> playerRecordData;
    private IPlayerRecordView view;
    private PlayerRecord ownRecord;

    public PlayerRecordModel(ServerCommunicator serverCommunicator, ILoadingIndicatorModel loadingIndicatorModel, IAudioManager audioManager,
        IGlobalStateModel globalStateModel)
    {
        this.serverCommunicator = serverCommunicator;
        this.loadingIndicatorModel = loadingIndicatorModel;
        this.audioManager = audioManager;
        this.globalStateModel = globalStateModel;

        playerRecordData = new List<PlayerRecord>();
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

    public void RequestOpen(bool isRequestRecord = false)
    {
        if (isRequestRecord)
            RequestGetPlayerRecord(OpenView);
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

    public void RequestGetPlayerRecord(Action callback = null)
    {
        serverCommunicator.CreatePostRequest(GameConst.URL_PLAYER_RECORD_API, API_GET_RECORD)
            .SendRequest<PlayerRecordResponse>((res) =>
            {
                if (res.statusCode == ServerResponse.STATUS_CODE_SUCCESS)
                    UpdatePlayerRecord(res.playerRecordList);

                callback?.Invoke();
            });
    }

    public void RequestAddPlayerRecord(int costTimeSeconds, Action callback = null)
    {
        serverCommunicator.CreatePostRequest(GameConst.URL_PLAYER_RECORD_API, "api_add_record")
            .AddParameter("playerName", globalStateModel.GetPlayerName)
            .AddParameter("costTimeSeonds", costTimeSeconds)
            .SendRequest<PlayerRecordResponse>((res) =>
            {
                if (res.statusCode == ServerResponse.STATUS_CODE_SUCCESS)
                {
                    SaveOwnRecordInfo(globalStateModel.GetPlayerName, costTimeSeconds);
                    UpdatePlayerRecord(res.playerRecordList);
                }

                callback?.Invoke();
            });
    }

    private void SaveOwnRecordInfo(string selfPlayerName, int costTimeSeconds)
    {
        ownRecord = new PlayerRecord
        {
            playerName = selfPlayerName,
            costTimeSeconds = costTimeSeconds
        };
    }

    private void UpdatePlayerRecord(List<PlayerRecord> playerRecords)
    {
        if (playerRecords == null || playerRecords.Count == 0)
            return;

        playerRecordData = playerRecords.OrderBy(x => x.costTimeSeconds).ToList();

        if (ownRecord != null)
        {
            PlayerRecord matchRecord = playerRecordData.FirstOrDefault(x => x.playerName == ownRecord.playerName && x.costTimeSeconds == ownRecord.costTimeSeconds);
            if (matchRecord != null)
                matchRecord.isOwnRecord = true;
        }
    }

    private void OpenView()
    {
        if (HaveOwnRecord())
            view.SetOwnRecordEffectPosition(playerRecordData.FindIndex(x => x.isOwnRecord));

        view.SetOwnRecordEffectActive(HaveOwnRecord());
        view.SetActive(true);
        view.UpdateView();

        IsViewOpening = true;
    }

    private bool HaveOwnRecord()
    {
        bool haveOwnRecord = playerRecordData.Any(x => x.isOwnRecord);
        if (haveOwnRecord == false)
            return false;

        int ownRecordIndex = playerRecordData.FindIndex(x => x.isOwnRecord);
        return ownRecordIndex < PLAYER_RECORD_RANKING_LIMIT;
    }

    private void OnRequestCompleted()
    {
        serverCommunicator.OnRequestCompleted -= OnRequestCompleted;

        loadingIndicatorModel.Close();
        OpenView();
    }
}