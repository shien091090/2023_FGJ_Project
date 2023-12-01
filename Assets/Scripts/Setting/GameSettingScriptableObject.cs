using UnityEngine;

[CreateAssetMenu]
public class GameSettingScriptableObject : ScriptableObject, IGameSetting
{
    [SerializeField] private int totalMissingTextureCount;
    [SerializeField] private int upRange;
    [SerializeField] private int downRange;

    public int TotalMissingTextureCount => totalMissingTextureCount;
    public int UpRange => upRange;
    public int DownRange => downRange;
}