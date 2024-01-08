using System;

public interface IPlayerRecordModel
{
    bool IsViewOpening { get; }
    string GetRecordPlayerName(int rankNumber);
    string GetRecordCostTime(int rankNumber);
    void RequestGetPlayerRecord(Action callback = null);
    void RequestAddPlayerRecord(int costTimeSeconds, Action callback = null);
    void RequestOpen(bool isRequestRecord = false);
    void BindView(IPlayerRecordView view);
    void CloseView();
}