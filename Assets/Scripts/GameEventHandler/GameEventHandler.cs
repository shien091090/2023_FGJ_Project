using System;
using UnityEngine;

public class GameEventHandler : MonoBehaviour, ICharacterEventHandler
{
    private static GameEventHandler _instance;
    public event Action OnCharacterDie;
    public static GameEventHandler Instance => _instance;

    public void TriggerDieEvent()
    {
        OnCharacterDie?.Invoke();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
}