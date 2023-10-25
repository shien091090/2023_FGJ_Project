using System;
using System.Collections;
using UnityEngine;

public class SceneItemView : MonoBehaviour
{
    [SerializeField] private ItemType itemType;

    private Animator anim;
    private MissingTexturePart missingTexturePart;

    private Animator GetAnim
    {
        get
        {
            if (anim == null)
                anim = GetComponent<Animator>();

            return anim;
        }
    }

    private MissingTexturePart GetMissingTexturePart
    {
        get
        {
            if (missingTexturePart == null)
                missingTexturePart = GetComponent<MissingTexturePart>();

            return missingTexturePart;
        }
    }

    private void Start()
    {
        InitSceneItem();
    }

    private void InitSceneItem()
    {
        gameObject.SetActive(true);
        GetAnim.Play("get_item_idle", 0);
    }

    private IEnumerator Cor_GetItemEffect()
    {
        GetAnim.Play("get_item_effect", 0);

        yield return new WaitForSeconds(1);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Player && gameObject.activeSelf)
        {
            if (ItemInventoryView.Instance.AlreadyHaveSpecificTypeItem(itemType))
                return;

            ItemInventoryView.Instance.AddItem(itemType);
            GetMissingTexturePart.ClearMissingTexture();
            StartCoroutine(Cor_GetItemEffect());
        }
    }
}