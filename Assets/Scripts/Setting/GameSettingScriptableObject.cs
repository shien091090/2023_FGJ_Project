using UnityEngine;

public class GameSettingScriptableObject : ScriptableObject, IGameSetting
{
    [SerializeField] private int totalMissingTextureCount;
    [SerializeField] private int upRange;
    [SerializeField] private int downRange;
    [SerializeField] private float autoDisappearAfterimageSeconds;
    [SerializeField] private float spawnAfterimageEffectFrequency;

    public float AutoDisappearSeconds => autoDisappearAfterimageSeconds;
    public float SpawnEffectFrequency => spawnAfterimageEffectFrequency;
    public int TotalMissingTextureCount => totalMissingTextureCount;
    public int UpRange => upRange;
    public int DownRange => downRange;
}