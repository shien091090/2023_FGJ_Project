using System;
using UnityEngine;
using UnityEngine.UI;

public class MissingTextureManagerView : MonoBehaviour, IMissingTextureManagerView
{
    private static MissingTextureManagerView _instance;

    [SerializeField] private int totalMissingTextureCount;
    [SerializeField] private Text txt_remainPercent;
    [SerializeField] private Slider sld_progress;

    private MissingTextureManager missingTextureManager;

    public event Action OnMissingTextureAllClear;

    public int GetTotalMissingTextureCount => totalMissingTextureCount;
    public static MissingTextureManagerView Instance => _instance;

    public void RefreshRemainPercentText(string remainPercentText)
    {
        txt_remainPercent.text = remainPercentText;
    }

    public void RefreshProgress(float progress)
    {
        sld_progress.value = progress;
    }

    public void SendMissingTextureAllClearEvent()
    {
        OnMissingTextureAllClear?.Invoke();
    }

    public void SubtractMissingTextureCount()
    {
        missingTextureManager.SubtractMissingTextureCount();
    }

    public void ResetGame()
    {
        missingTextureManager.ResetGame();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        missingTextureManager = new MissingTextureManager(totalMissingTextureCount, this);
    }
}