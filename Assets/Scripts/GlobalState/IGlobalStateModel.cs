public interface IGlobalStateModel
{
    bool IsPlayerNameInputted { get; }
    string GetPlayerName { get; }
    void SetPlayerName(string playerName);
}