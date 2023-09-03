using System;
using UnityEngine;

public class MissingTextureManager : MonoBehaviour
{
    private static MissingTextureManager _instance;
    
    [SerializeField] private int totalMissingTextureCount;
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
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        ResetGame();
    }
}