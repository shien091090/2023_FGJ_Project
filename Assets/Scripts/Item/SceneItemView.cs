using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;
using Zenject;

public class SceneItemView : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private Animator anim_getItemEffect;

    [Inject] private IItemInventoryModel itemInventoryModel;
    [Inject] private ICharacterModel characterModel;
    [Inject] private IAudioManager audioManager;

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

        characterModel.OnCharacterDie -= InitSceneItem;
        characterModel.OnCharacterDie += InitSceneItem;
    }

    private void InitSceneItem()
    {
        gameObject.SetActive(true);
        GetAnim.Play("get_item_idle", 0);
    }

    private IEnumerator Cor_GetItemEffect()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_COLLECT_ITEM);
        GetAnim.Play("get_item_effect", 0);
        anim_getItemEffect.Play("get_item_shining_effect", 0);

        yield return new WaitForSeconds(1);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Player && gameObject.activeSelf)
        {
            if (itemInventoryModel.AlreadyHaveSpecificTypeItem(itemType))
                return;

            itemInventoryModel.CheckAddItem(itemType);
            GetMissingTexturePart.ClearMissingTexture();
            StartCoroutine(Cor_GetItemEffect());
        }
    }
}