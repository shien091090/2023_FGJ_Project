using SNShien.Common.AudioTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class InitSceneInstaller : MonoInstaller 
{
    [SerializeField] private ServerCommunicator serverCommunicator;
    [SerializeField] private FmodAudioManager fmodAudioManager;

    public override void InstallBindings()
    {
        Container.Bind<ServerCommunicator>().FromInstance(serverCommunicator);
        Container.Bind<IAudioManager>().FromInstance(fmodAudioManager);
        Container.Bind<ILoadingIndicatorModel>().To<LoadingIndicatorModel>().AsSingle();
        Container.Bind<IPlayerRecordModel>().To<PlayerRecordModel>().AsSingle();
        Container.Bind<IGlobalStateModel>().To<GlobalStateModel>().AsSingle();
    }

    private void Start()
    {
        GotoStartScene();
    }

    private void GotoStartScene()
    {
        SceneManager.LoadScene("StartScene", LoadSceneMode.Additive);
    }
}
