using System;
using UnityEngine;

public class ItemStateManager : MonoBehaviour
{
    private static ItemStateManager _instance;
    public event Action<ItemType> OnEndItemEffect;
    public event Action<ItemType> OnStartItemEffect;
    public event Action<ItemType> OnUseItemOneTime;
    public static ItemStateManager Instance => _instance;

    public void StartItemEffect(ItemType itemType)
    {
        OnStartItemEffect?.Invoke(itemType);
    }

    public void EndItemEffect(ItemType itemType)
    {
        OnEndItemEffect?.Invoke(itemType);
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public void TriggerItemOneTime(ItemType itemType)
    {
        OnUseItemOneTime?.Invoke(itemType);
    }
}