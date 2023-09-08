using System;
using UnityEngine;
using UnityEngine.UI;

public class MissingTextureManagerView : MonoBehaviour, IMissingTextureManagerView
{
    private static MissingTextureManagerView _instance;

    [SerializeField] private int totalMissingTextureCount;
    [SerializeField] private Text txt_remainCount;

    private MissingTextureManager missingTextureManager;

    public event Action OnMissingTextureAllClear;

    public int GetTotalMissingTextureCount => totalMissingTextureCount;
    public static MissingTextureManagerView Instance => _instance;

    public void RefreshRemainCount(string remainCountText)
    {
        txt_remainCount.text = remainCountText;
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