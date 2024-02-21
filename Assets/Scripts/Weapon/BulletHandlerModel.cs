using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

public class BulletHandlerModel : IBulletHandlerModel
{
    private readonly IItemTriggerHandler itemTriggerHandler;
    private readonly ICharacterPresenter characterPresenter;
    private readonly IGameObjectPool gameObjectPool;
    private readonly IAudioManager audioManager;
    private IBulletHandlerView view;

    public BulletHandlerModel(IItemTriggerHandler itemTriggerHandler, ICharacterPresenter characterPresenter, IGameObjectPool gameObjectPool, IAudioManager audioManager)
    {
        this.itemTriggerHandler = itemTriggerHandler;
        this.characterPresenter = characterPresenter;
        this.gameObjectPool = gameObjectPool;
        this.audioManager = audioManager;

        RegisterEvent();
    }

    public void BindView(BulletHandlerView view)
    {
        this.view = view;
    }

    private void RegisterEvent()
    {
        itemTriggerHandler.OnUseItemOneTime -= OnUseItemOneTime;
        itemTriggerHandler.OnUseItemOneTime += OnUseItemOneTime;
    }

    private void OnUseItemOneTime(ItemType itemType)
    {
        if (itemType != ItemType.Weapon)
            return;

        gameObjectPool.SpawnGameObject(GameConst.PREFAB_NAME_BULLET_SHOOT_EFFECT, characterPresenter.CurrentPos, new Vector3(characterPresenter.IsFaceRight ?
            1 :
            -1, 1, 1));

        audioManager.PlayOneShot(GameConst.AUDIO_KEY_GUN_SHOT);
        GameObject bulletObject = view.GetBulletObject();
        bulletObject.transform.position = characterPresenter.CurrentPos;
        bulletObject.SetActive(true);
        bulletObject.transform.rotation = Quaternion.Euler(0, 0, characterPresenter.IsFaceRight ?
            -90 :
            90);
    }
}