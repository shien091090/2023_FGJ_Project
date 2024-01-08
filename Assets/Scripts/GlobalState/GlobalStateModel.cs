public class GlobalStateModel : IGlobalStateModel
{
    public bool IsPlayerNameInputted => string.IsNullOrEmpty(playerName) == false;
    public string GetPlayerName => playerName;
    private string playerName;

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }
}