using System;
using UnityEngine;

public class ItemStateManager : MonoBehaviour
{
    private static ItemStateManager _instance;
    private event Action<ItemType> OnEndItemEffect;
    private event Action<ItemType> OnStartItemEffect;
    private event Action<ItemType> OnUseItemOneTime;
    public static ItemStateManager Instance => _instance;

    public void StartItemEffect(ItemType itemType)
    {
        Debug.Log($"StartItemEffect: {itemType}");
        OnStartItemEffect?.Invoke(itemType);
    }

    public void EndItemEffect(ItemType itemType)
    {
        Debug.Log($"EndItemEffect: {itemType}");
        OnEndItemEffect?.Invoke(itemType);
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public void TriggerItemOneTime(ItemType itemType)
    {
        Debug.Log($"TriggerItemOneTime: {itemType}");
        OnUseItemOneTime?.Invoke(itemType);
    }
}