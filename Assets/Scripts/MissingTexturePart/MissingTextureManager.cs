using System;
using UnityEngine;
using UnityEngine.UI;

public class MissingTextureManager : MonoBehaviour
{
    private static MissingTextureManager _instance;

    [SerializeField] private int totalMissingTextureCount;
    [SerializeField] private Text txt_remainCount;
    private bool isGameCompleted;

    public static event Action OnMissingTextureAllClear;

    public int GetTotalMissingTextureCount => totalMissingTextureCount;
    public static MissingTextureManager Instance => _instance;

    public int CurrentMissingTextureCount { get; set; }

    public void SubtractMissingTextureCount()
    {
        if (isGameCompleted)
            return;

        CurrentMissingTextureCount--;
        RefreshRemainCount();

        if (CurrentMissingTextureCount <= 0)
        {
            OnMissingTextureAllClear?.Invoke();
            isGameCompleted = true;
        }
    }

    public void ResetGame()
    {
        CurrentMissingTextureCount = totalMissingTextureCount;
        isGameCompleted = false;
        RefreshRemainCount();
    }

    private void RefreshRemainCount()
    {
        txt_remainCount.text = CurrentMissingTextureCount.ToString();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        ResetGame();
    }
}