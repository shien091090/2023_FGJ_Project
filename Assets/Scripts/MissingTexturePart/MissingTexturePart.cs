using System;
using UnityEngine;

public class MissingTexturePart : MonoBehaviour
{
    [SerializeField] private GameObject go_missingTexture;

    public void Start()
    {
        SetMissingTextureActive(true);
    }

    public void ClearMissingTexture()
    {
        if (go_missingTexture.activeSelf == false)
            return;

        SetMissingTextureActive(false);
        MissingTextureManager.Instance.SubtractMissingTextureCount();
    }

    private void SetMissingTextureActive(bool isActive)
    {
        go_missingTexture.SetActive(isActive);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Player ||
            col.gameObject.layer == (int)GameConst.GameObjectLayerType.Weapon)
        {
            ClearMissingTexture();
        }
    }
}