using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerRecordModel
{
    private const string API_GET_RECORD = "api_get_record";
    private const string EMPTY_TEXT = "-";

    private readonly ServerCommunicator serverCommunicator;
    private List<PlayerRecord> playerRecordData;


    public PlayerRecordModel(ServerCommunicator serverCommunicator)
    {
        this.serverCommunicator = serverCommunicator;
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

    private void UpdatePlayerRecord(List<PlayerRecord> playerRecords)
    {
        if (playerRecords == null || playerRecords.Count == 0)
            return;

        playerRecordData = playerRecords.OrderBy(x => x.costTimeSeconds).ToList();
    }
}