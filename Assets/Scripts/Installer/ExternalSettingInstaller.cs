using SNShien.Common.AudioTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu]
public class ExternalSettingInstaller : ScriptableObjectInstaller
{
    [SerializeField] private GameSettingScriptableObject gameSetting;

    public override void InstallBindings()
    {
        Container.Bind<IGameSetting>().FromInstance(gameSetting);
    }
}