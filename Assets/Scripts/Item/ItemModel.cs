using System;

public class ItemModel
{
    private readonly ItemType itemType;
    private float useLimit;
    private ItemUseType itemUseType;
    private int currentUseTimes;
    private float currentTimer;
    private bool startUsePassTimeItem;
    private bool isUsed;

    public event Action OnItemUseComplete;
    public event Action<int> OnRefreshCurrentUseTimes;

    public event Action<float> OnRefreshCurrentTimer;
    // public event Action<ItemType> OnUseItemOneTime;

    public ItemModel(ItemType itemType)
    {
        this.itemType = itemType;
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
            OnItemUseComplete?.Invoke();
        }
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

    public void UseItem()
    {
        if (isUsed)
            return;

        if (itemUseType == ItemUseType.PassTime)
        {
            startUsePassTimeItem = true;
            return;
        }

        currentUseTimes--;

        // OnUseItemOneTime?.Invoke(itemType);
        OnRefreshCurrentUseTimes?.Invoke(currentUseTimes);

        if (currentUseTimes <= 0)
        {
            isUsed = true;
            OnItemUseComplete?.Invoke();
        }
    }
}