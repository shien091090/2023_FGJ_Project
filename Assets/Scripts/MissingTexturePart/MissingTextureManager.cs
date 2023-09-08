public class MissingTextureManager
{
    private readonly int totalMissingTextureCount;
    private readonly IMissingTextureManagerView view;
    private bool isGameCompleted;
    private int currentMissingTextureCount;

    public MissingTextureManager(int totalMissingTextureCount, IMissingTextureManagerView view)
    {
        this.totalMissingTextureCount = totalMissingTextureCount;
        this.view = view;
        ResetGame();
    }

    public void SubtractMissingTextureCount()
    {
        if (isGameCompleted)
            return;

        currentMissingTextureCount--;
        view.RefreshRemainCount(currentMissingTextureCount.ToString());

        if (currentMissingTextureCount <= 0)
        {
            view.SendMissingTextureAllClearEvent();
            isGameCompleted = true;
        }
    }

    public void ResetGame()
    {
        currentMissingTextureCount = totalMissingTextureCount;
        isGameCompleted = false;
        view.RefreshRemainCount(currentMissingTextureCount.ToString());
    }
}