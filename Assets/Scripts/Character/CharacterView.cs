using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float jumpDelaySeconds;
    [SerializeField] private Transform footPoint;
    [SerializeField] private float footRadius;

    private Rigidbody2D rigidbody;
    private CharacterModel characterModel;

    private Rigidbody2D GetRigidbody
    {
        get
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody2D>();

            return rigidbody;
        }
    }

    private void Start()
    {
        characterModel = new CharacterModel(new CharacterMoveController(), new CharacterKeyController(), new TeleportManager());
        characterModel.SetJumpDelay(jumpDelaySeconds);

        SetEventRegister();
    }

    private void Update()
    {
        characterModel.UpdateJumpTimer(Time.deltaTime);
        characterModel.UpdateCheckJump(jumpForce);
        characterModel.UpdateMove(Time.deltaTime, speed);
    }

    private void SetEventRegister()
    {
        characterModel.OnHorizontalMove -= OnHorizontalMove;
        characterModel.OnHorizontalMove += OnHorizontalMove;

        characterModel.OnJump -= OnJump;
        characterModel.OnJump += OnJump;
    }

    public void OnCollisionStay2D(Collision2D col)
    {
        bool isOnFloor = Physics2D.OverlapCircle(footPoint.position, footRadius, LayerMask.GetMask(GameConst.GameObjectLayerType.Platform.ToString()));

        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Platform && isOnFloor)
            characterModel.TriggerFloor();
    }

    private void OnJump(float jumpForce)
    {
        GetRigidbody.AddForce(new Vector2(0, jumpForce));
    }

    private void OnHorizontalMove(float moveValue)
    {
        transform.Translate(new Vector2(moveValue, 0));
    }
}