public interface IGameSetting
{
    float AutoDisappearSeconds { get; }
    float SpawnEffectFrequency { get; }
    int TotalMissingTextureCount { get; }
    int UpRange { get; }
    int DownRange { get; }
}