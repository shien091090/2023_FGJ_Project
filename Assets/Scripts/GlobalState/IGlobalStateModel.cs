public interface IGlobalStateModel
{
    bool IsPlayerNameInputted { get; }
    void SetPlayerName(string playerName);
}