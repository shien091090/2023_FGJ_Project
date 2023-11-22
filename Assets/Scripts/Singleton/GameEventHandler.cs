using System;
using UnityEngine;

public class GameEventHandler : MonoBehaviour, IGameEventHandler
{
    private static GameEventHandler _instance;
    public CharacterState CurrentCharacterState { get; private set; }
    public event Action OnCharacterDie;
    public static GameEventHandler Instance => _instance;

    public void ChangeCurrentCharacterState(CharacterState state)
    {
        if (CurrentCharacterState == state)
            return;

        Debug.Log($"ChangeCurrentCharacterState: {state}");
        CurrentCharacterState = state;

        if (state == CharacterState.Die)
            OnCharacterDie?.Invoke();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
}