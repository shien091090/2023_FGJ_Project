using System;
using UnityEngine;

public class GameEventHandler : MonoBehaviour, ICharacterEventHandler
{
    private static GameEventHandler _instance;
    public event Action OnCharacterDie;

    public CharacterState CurrentCharacterState { get; private set; }
    public static GameEventHandler Instance => _instance;

    public void ChangeCurrentCharacterState(CharacterState state)
    {
        if (CurrentCharacterState == state)
            return;
        
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