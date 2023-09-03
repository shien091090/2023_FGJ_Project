using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float jumpDelaySeconds;
    [SerializeField] private Transform footPoint;
    [SerializeField] private float footRadius;

    private Rigidbody2D rigidbody;
    private CharacterMoveModel characterMoveModel;

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
        characterMoveModel = new CharacterMoveModel(new CharacterMoveController(), new CharacterKeyController());
        characterMoveModel.SetJumpDelay(jumpDelaySeconds);

        SetEventRegister();
    }

    private void Update()
    {
        characterMoveModel.UpdateJumpTimer(Time.deltaTime);
        characterMoveModel.UpdateCheckJump(jumpForce);
        characterMoveModel.UpdateMove(Time.deltaTime, speed);
    }

    private void SetEventRegister()
    {
        characterMoveModel.OnHorizontalMove -= OnHorizontalMove;
        characterMoveModel.OnHorizontalMove += OnHorizontalMove;

        characterMoveModel.OnJump -= OnJump;
        characterMoveModel.OnJump += OnJump;
    }

    public void OnCollisionStay2D(Collision2D col)
    {
        bool isOnFloor = Physics2D.OverlapCircle(footPoint.position, footRadius, LayerMask.GetMask(GameConst.GameObjectLayerType.Platform.ToString()));

        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Platform && isOnFloor)
            characterMoveModel.TriggerFloor();
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