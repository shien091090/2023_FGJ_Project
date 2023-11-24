using System;
using UnityEngine;

public class ItemModel
{
    private readonly ItemType itemType;
    private readonly ICharacterModel characterModel;
    private readonly IItemTriggerHandler itemTriggerHandler;

    private float useLimit;
    private ItemUseType itemUseType;
    private int currentUseTimes;
    private float currentTimer;
    private bool startUsePassTimeItem;
    private bool isUsed;

    public event Action OnItemUseComplete;
    public event Action<int> OnRefreshCurrentUseTimes;
    public event Action<float> OnRefreshCurrentTimer;
    public event Action<ItemType> OnUseItemOneTime;
    public event Action<ItemType> OnStartItemEffect;
    public event Action<ItemType> OnEndItemEffect;

    public ItemModel(ItemType itemType, ICharacterModel characterModel, IItemTriggerHandler itemTriggerHandler)
    {
        this.itemType = itemType;
        this.characterModel = characterModel;
        this.itemTriggerHandler = itemTriggerHandler;
    }

    public void SetUseTimesType(int useTimes)
    {
        itemUseType = ItemUseType.UseTimes;
        currentUseTimes = useTimes;
        currentTimer = 0;
        startUsePassTimeItem = false;
        isUsed = false;
    }

    public void SetPassTimeType(float timerSetting)
    {
        itemUseType = ItemUseType.PassTime;
        currentUseTimes = 0;
        currentTimer = timerSetting;
        startUsePassTimeItem = false;
        isUsed = false;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (itemUseType == ItemUseType.UseTimes || startUsePassTimeItem == false || isUsed)
            return;

        currentTimer = Math.Max(currentTimer - deltaTime, 0);

        OnRefreshCurrentTimer?.Invoke(currentTimer);

        if (currentTimer <= 0)
        {
            isUsed = true;
            itemTriggerHandler.EndItemEffect(itemType);
            OnItemUseComplete?.Invoke();
        }
    }

    public void UseItem()
    {
        if (isUsed || characterModel.CurrentCharacterState == CharacterState.Die)
            return;

        if (itemUseType == ItemUseType.PassTime)
            UsePassTimeTypeItem();
        else
            UseOneTimeTypeItem();
    }

    private void UsePassTimeTypeItem()
    {
        if (startUsePassTimeItem)
            return;

        startUsePassTimeItem = true;
        itemTriggerHandler.StartItemEffect(itemType);
    }

    private void UseOneTimeTypeItem()
    {
        if (itemType == ItemType.Shoes && characterModel.CurrentCharacterState == CharacterState.Jumping)
            return;

        currentUseTimes--;

        itemTriggerHandler.TriggerItemOneTime(itemType);
        OnRefreshCurrentUseTimes?.Invoke(currentUseTimes);

        if (currentUseTimes > 0)
            return;

        isUsed = true;
        OnItemUseComplete?.Invoke();
    }
}