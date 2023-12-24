using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemInventoryView : MonoBehaviour, IItemInventoryView
{
    [SerializeField] private RectTransform[] slotPosArray;
    [SerializeField] private ItemSettingScriptableObject itemSetting;
    [SerializeField] private Transform itemHolder;

    private IItemInventoryModel itemInventoryModel;
    private List<ItemView> itemViewList;

    private void Start()
    {
        itemInventoryModel = ItemInventoryModel.Instance;
        itemInventoryModel.SetSlotLimit(slotPosArray.Select(x => x.localPosition).ToArray());
        itemInventoryModel.BindView(this);
        
        itemViewList = new List<ItemView>();
    }

    private void Update()
    {
        itemInventoryModel.UpdateCheckUseItem();
    }

    public bool AlreadyHaveSpecificTypeItem(ItemType itemType)
    {
        return itemInventoryModel.AlreadyHaveSpecificTypeItem(itemType);
    }

    public IItem GetItemObject(ItemType itemType)
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

    [ContextMenu("Get Protection Item")]
    private void GetProtectionItem()
    {
        itemInventoryModel.CheckAddItem(ItemType.Protection);
    }

    [ContextMenu("Get Weapon Item")]
    private void GetWeaponItem()
    {
        itemInventoryModel.CheckAddItem(ItemType.Weapon);
    }

    [ContextMenu("Get Shoes Item")]
    private void GetShoesItem()
    {
        itemInventoryModel.CheckAddItem(ItemType.Shoes);
    }
}