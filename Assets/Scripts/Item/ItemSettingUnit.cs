using UnityEngine;

[System.Serializable]
public class ItemSettingUnit
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private GameObject prefab;

    public ItemType ItemType => itemType;
    public GameObject Prefab => prefab;
}