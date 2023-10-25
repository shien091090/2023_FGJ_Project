using UnityEngine;

public class GameEventHandler : MonoBehaviour, ICharacterEventHandler
{
    private static GameEventHandler _instance;
    public static GameEventHandler Instance => _instance;

    public void TriggerDieEvent()
    {
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
}