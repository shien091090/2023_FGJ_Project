using System;

public class ItemModel
{
    private readonly ItemUseType itemUseType;
    private readonly float useLimit;
    private int currentUseTimes;
    private float currentTimer;
    private bool startUsePassTimeItem;

    public event Action OnItemUseComplete;
    public event Action<int> OnRefreshCurrentUseTimes;
    public event Action<float> OnRefreshCurrentTimer;

    public ItemModel(ItemUseType itemUseType, float useLimit)
    {
        this.itemUseType = itemUseType;
        this.useLimit = useLimit;
        currentUseTimes = (int)useLimit;
        currentTimer = useLimit;
        startUsePassTimeItem = false;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (itemUseType == ItemUseType.UseTimes || startUsePassTimeItem == false)
            return;

        currentTimer = Math.Max(currentTimer - deltaTime, 0);

        OnRefreshCurrentTimer?.Invoke(currentTimer);

        if (currentTimer <= 0)
            OnItemUseComplete?.Invoke();
    }

    public void UseItem()
    {
        if (itemUseType == ItemUseType.PassTime)
        {
            startUsePassTimeItem = true;
            return;
        }

        currentUseTimes--;

        OnRefreshCurrentUseTimes?.Invoke(currentUseTimes);

        if (currentUseTimes <= 0)
            OnItemUseComplete?.Invoke();
    }
}