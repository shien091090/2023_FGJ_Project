using UnityEngine;

[CreateAssetMenu]
public class CharacterSettingScriptableObject : ScriptableObject, ICharacterSetting
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float superJumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float jumpDelaySeconds;
    [SerializeField] private float interactDistance;
    [SerializeField] private float fallDownLimitPosY;

    public float JumpForce => jumpForce;
    public float SuperJumpForce => superJumpForce;
    public float Speed => speed;
    public float JumpDelaySeconds => jumpDelaySeconds;
    public float InteractDistance => interactDistance;
    public float FallDownLimitPosY => fallDownLimitPosY;
}