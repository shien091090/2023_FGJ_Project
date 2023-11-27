using UnityEngine;

public class BulletHandlerModel
{
    private static BulletHandlerModel _instance;

    private readonly IItemTriggerHandler itemTriggerHandler;
    private readonly ICharacterModel characterModel;
    private IBulletHandlerView view;

    public static BulletHandlerModel Instance => _instance;

    public BulletHandlerModel(IItemTriggerHandler itemTriggerHandler, ICharacterModel characterModel)
    {
        this.itemTriggerHandler = itemTriggerHandler;
        this.characterModel = characterModel;

        _instance = this;

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

        GameObject bulletObject = view.GetBulletObject();
        bulletObject.transform.position = characterModel.CurrentPos;
        bulletObject.SetActive(true);
        bulletObject.transform.rotation = Quaternion.Euler(0, 0, characterModel.IsFaceRight ?
            -90 :
            90);
    }
}