using System;
using UnityEngine;

public class MissingTextureManager : IMissingTextureManager
{
    private readonly IGameSetting gameSetting;

    private IMissingTextureManagerView view;
    private bool isGameCompleted;
    private int currentMissingTextureCount;

    public MissingTextureManager(IGameSetting gameSetting)
    {
        this.gameSetting = gameSetting;
    }

    public event Action OnMissingTextureAllClear;

    public void SubtractMissingTextureCount(int count = 1)
    {
        if (isGameCompleted)
            return;

        currentMissingTextureCount -= count;
        view.RefreshRemainPercentText(ConvertCurrentPercentText());
        view.RefreshProgress(GetProgressPercent());
        view.PlayClearAnimation();

        if (currentMissingTextureCount <= 0)
        {
            OnMissingTextureAllClear?.Invoke();
            isGameCompleted = true;
        }
    }

    public void BindView(MissingTextureManagerView view)
    {
        this.view = view;
        ResetGame();
    }

    public void ResetGame()
    {
        currentMissingTextureCount = gameSetting.TotalMissingTextureCount;
        isGameCompleted = false;
        view.RefreshRemainPercentText(ConvertCurrentPercentText());
        view.RefreshProgress(GetProgressPercent());
    }

    private float GetProgressPercent()
    {
        if (gameSetting.TotalMissingTextureCount <= 0)
            return 0;

        float percent = currentMissingTextureCount / (float)gameSetting.TotalMissingTextureCount;
        return percent;
    }

    private string ConvertCurrentPercentText()
    {
        if (gameSetting.TotalMissingTextureCount == 0)
            return "0";

        float percent = GetProgressPercent();
        float round = Mathf.Round(percent * 100);
        if (round == 100 && currentMissingTextureCount < gameSetting.TotalMissingTextureCount)
            return (round - 1).ToString();
        else if (round == 0 && currentMissingTextureCount > 0)
            return (round + 1).ToString();
        else
            return round.ToString();
    }
}