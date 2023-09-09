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

    public void SubtractMissingTextureCount(int count = 1)
    {
        if (isGameCompleted)
            return;

        currentMissingTextureCount -= count;
        view.RefreshRemainPercentText(ConvertCurrentPercentText());
        view.RefreshProgress(GetProgressPercent());

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
        view.RefreshRemainPercentText(ConvertCurrentPercentText());
        view.RefreshProgress(GetProgressPercent());
    }

    private float GetProgressPercent()
    {
        if (totalMissingTextureCount <= 0)
            return 0;

        float percent = currentMissingTextureCount / (float)totalMissingTextureCount;
        return percent;
    }

    private string ConvertCurrentPercentText()
    {
        if (totalMissingTextureCount == 0)
            return "0";

        float percent = GetProgressPercent();
        float round = Mathf.Round(percent * 100);
        if (round == 100 && currentMissingTextureCount < totalMissingTextureCount)
            return (round - 1).ToString();
        else
            return round.ToString();
    }
}