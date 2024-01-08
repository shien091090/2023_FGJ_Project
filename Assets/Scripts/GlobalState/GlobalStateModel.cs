public class GlobalStateModel : IGlobalStateModel
{
    public bool IsPlayerNameInputted => string.IsNullOrEmpty(playerName) == false;
    private string playerName;

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }
}