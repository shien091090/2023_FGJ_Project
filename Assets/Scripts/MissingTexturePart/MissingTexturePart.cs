using System;
using UnityEngine;

public class MissingTexturePart : MonoBehaviour
{
    [SerializeField] private GameObject go_missingTexture;

    public void Start()
    {
        SetMissingTextureActive(true);
    }

    private void SetMissingTextureActive(bool isActive)
    {
        go_missingTexture.SetActive(isActive);
    }

    private void ClearMissingTexture()
    {
        if (go_missingTexture.activeSelf == false)
            return;
        
        SetMissingTextureActive(false);
        MissingTextureManager.Instance.SubtractMissingTextureCount();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Player)
        {
            ClearMissingTexture();
        }
    }
}