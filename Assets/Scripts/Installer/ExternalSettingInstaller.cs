using UnityEngine;
using Zenject;

[CreateAssetMenu]
public class ExternalSettingInstaller : ScriptableObjectInstaller
{
    [SerializeField] private GameSettingScriptableObject gameSetting;
    [SerializeField] private CharacterSettingScriptableObject characterSetting;

    public override void InstallBindings()
    {
        Container.Bind<IGameSetting>().FromInstance(gameSetting);
        Container.Bind<ICharacterSetting>().FromInstance(characterSetting);
    }
}