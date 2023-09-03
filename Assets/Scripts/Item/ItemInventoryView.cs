using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemInventoryView : MonoBehaviour
{
    [SerializeField] private RectTransform[] slotPosArray;
    [SerializeField] private ItemSettingScriptableObject itemSetting;
    [SerializeField] private Transform itemHolder;

    private ItemInventoryModel itemInventoryModel;
    private List<ItemView> itemViewList;

    private void Start()
    {
        itemInventoryModel = new ItemInventoryModel(new CharacterKeyController());
        itemInventoryModel.SetSlotLimit(slotPosArray.Select(x => x.localPosition).ToArray());
        itemViewList = new List<ItemView>();
    }

    private void Update()
    {
        itemInventoryModel.UpdateCheckUseItem();
    }

    public void AddItem(ItemType itemType)
    {
        if (itemInventoryModel.AlreadyHaveSpecificTypeItem(itemType))
            return;

        ItemView itemView = GetItemObject(itemType);
        itemInventoryModel.AddItem(itemView);
    }

    private ItemView GetItemObject(ItemType itemType)
    {
        ItemView resultObj = itemViewList.FirstOrDefault(x => x.ItemType == itemType);
        if (resultObj == null)
        {
            GameObject itemPrefab = itemSetting.GetItemPrefab(itemType);
            GameObject itemObj = Instantiate(itemPrefab, itemHolder);
            resultObj = itemObj.GetComponent<ItemView>();
        }

        return resultObj;
    }
}