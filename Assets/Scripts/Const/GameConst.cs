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
    public const string ANIMATION_KEY_TUTORIAL_HIDE = "tutorial_hide";
    public const string ANIMATION_KEY_TUTORIAL_START = "tutorial_fade_in";
    public const string ANIMATION_KEY_TUTORIAL_ENTER_NEXT = "tutorial_enter_next";
    public const string ANIMATION_KEY_TUTORIAL_ENTER_GAME = "tutorial_enter_game_scene";
    public const string ANIMATION_KEY_TUTORIAL_SWITCH_BACK = "tutorial_switch_back";
    public const string ANIMATION_KEY_TUTORIAL_SWITCH_FORWARD = "tutorial_switch_forward";
}