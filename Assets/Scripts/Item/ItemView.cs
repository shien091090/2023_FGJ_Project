using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour, IItem
{
    [SerializeField] private float useLimit;
    [SerializeField] private GameObject go_useTimesPanel;
    [SerializeField] private GameObject go_passTimePanel;
    [SerializeField] private Text txt_timer;
    [SerializeField] private Text txt_remainUseTimes;
    [SerializeField] private ItemType itemType;

    public ItemType ItemType => itemType;
    private RectTransform rectTransform;
    private ItemModel itemModel;

    public RectTransform GetRectTransform
    {
        get
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            return rectTransform;
        }
    }

    public event Action<IItem> OnItemUseCompleted;

    public void SetPos(Vector3 pos)
    {
        GetRectTransform.localPosition = pos;
        gameObject.SetActive(true);
    }

    public void RemoveItem()
    {
        gameObject.SetActive(false);
    }

    public void UseItem()
    {
        SetPanelType(ItemType);
        itemModel.UseItem();
    }

    private void Start()
    {
        itemModel = new ItemModel();
        switch (ItemType)
        {
            case ItemType.Protection:
                itemModel.SetPassTimeType(useLimit);
                break;

            case ItemType.Weapon:
                itemModel.SetUseTimesType((int)useLimit);
                break;

            case ItemType.Shoes:
                itemModel.SetUseTimesType((int)useLimit);
                break;
        }

        HideAllPanel();
        SetEventRegister();
    }

    private void Update()
    {
        itemModel.UpdateTimer(Time.deltaTime);
    }

    private void SetEventRegister()
    {
        itemModel.OnRefreshCurrentUseTimes -= RefreshCurrentUseTimes;
        itemModel.OnRefreshCurrentUseTimes += RefreshCurrentUseTimes;

        itemModel.OnItemUseComplete -= OnItemUseComplete;
        itemModel.OnItemUseComplete += OnItemUseComplete;

        itemModel.OnRefreshCurrentTimer -= RefreshCurrentTimer;
        itemModel.OnRefreshCurrentTimer += RefreshCurrentTimer;
    }

    private void SetPanelType(ItemType itemType)
    {
        go_useTimesPanel.SetActive(itemType == ItemType.Weapon || itemType == ItemType.Shoes);
        go_passTimePanel.SetActive(itemType == ItemType.Protection);
    }

    private void RefreshCurrentTimer(float timerValue)
    {
        txt_timer.text = timerValue.ToString("0.0");
    }

    private void RefreshCurrentUseTimes(int remainUseTimes)
    {
        txt_remainUseTimes.text = remainUseTimes.ToString();
    }

    private void HideAllPanel()
    {
        go_useTimesPanel.SetActive(false);
        go_passTimePanel.SetActive(false);
    }

    private void OnItemUseComplete()
    {
        OnItemUseCompleted?.Invoke(this);
    }
}