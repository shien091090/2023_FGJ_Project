using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private ObjectPoolManager gameObjectPool;

    public override void InstallBindings()
    {
        Container.Bind<IMoveController>().To<CharacterMoveController>().AsSingle();
        Container.Bind<IKeyController>().To<CharacterKeyController>().AsSingle();
        Container.Bind<IItemInventoryModel>().To<ItemInventoryModel>().AsSingle();
        Container.Bind<IDeltaTimeGetter>().To<DeltaTimeGetter>().AsSingle();
        Container.Bind<IItemTriggerHandler>().To<ItemTriggerHandler>().AsSingle();
        Container.Bind<ICharacterModel>().To<CharacterModel>().AsSingle();
        Container.Bind<ITileRemoverModel>().To<TileRemoverModel>().AsSingle();
        Container.Bind<IBulletHandlerModel>().To<BulletHandlerModel>().AsSingle();
        Container.Bind<IMissingTextureManager>().To<MissingTextureManager>().AsSingle();
        Container.Bind<ITimerModel>().To<TimerModel>().AsSingle();
        Container.Bind<IGameObjectPool>().FromInstance(gameObjectPool);
    }
}