using System.Collections.Generic;
using UnityEngine;

public class ItemSettingScriptableObject : ScriptableObject
{
    [SerializeField] private List<ItemSettingUnit> itemSettingUnits;

    public GameObject GetItemPrefab(ItemType itemType)
    {
        foreach (ItemSettingUnit unit in itemSettingUnits)
        {
            if (unit.ItemType == itemType)
                return unit.Prefab;
        }

        return null;
    }
}