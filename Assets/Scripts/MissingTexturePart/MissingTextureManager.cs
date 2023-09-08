using UnityEngine;

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
        view.RefreshRemainCount(ConvertCurrentPercentText());

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
        view.RefreshRemainCount(ConvertCurrentPercentText());
    }

    private string ConvertCurrentPercentText()
    {
        if (totalMissingTextureCount == 0)
            return "0";

        float percent = currentMissingTextureCount / (float)totalMissingTextureCount;
        return Mathf.RoundToInt(percent * 100).ToString();
    }
}