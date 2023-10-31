public class GameConst
{
    public enum GameObjectLayerType
    {
        Platform = 6,
        Player = 7,
        TeleportGate = 8,
        Monster = 9,
        Weapon = 10,
        SavePoint = 11,
    }

    public const string AUDIO_KEY_DAMAGE = "Damage";
    public const string AUDIO_KEY_JUMP = "Jump";
    public const string AUDIO_KEY_TELEPORT = "Teleport";
    public const string AUDIO_KEY_SAVE_POINT = "SavePoint";

    public const string ANIMATION_KEY_CHARACTER_DIE = "character_die";
    public const string ANIMATION_KEY_CHARACTER_NORMAL = "character_normal";
    public const string ANIMATION_KEY_CHARACTER_ENTER_HOUSE = "character_enter_house";
    public const string ANIMATION_KEY_CHARACTER_EXIT_HOUSE = "character_exit_house";
}