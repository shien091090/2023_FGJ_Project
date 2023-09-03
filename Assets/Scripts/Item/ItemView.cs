using System;
using UnityEngine;

public class ItemView : MonoBehaviour, IItem
{
    public ItemType ItemType { get; }
    private RectTransform rectTransform;

    public RectTransform GetRectTransform
    {
        get
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            return rectTransform;
        }
    }

    public event Action<IItem> OnItemUsed;

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
    }
}