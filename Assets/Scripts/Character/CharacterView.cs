using System;
using SNShien.Common.AudioTools;
using UnityEngine;

public class CharacterView : MonoBehaviour, ITransform
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float jumpDelaySeconds;
    [SerializeField] private Transform footPoint;
    [SerializeField] private float footRadius;
    [SerializeField] private TeleportComponent te1eportComponent;
    [SerializeField] private float fallDownToOriginPosTime;
    [SerializeField] private float interactDistance;

    public Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

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
        characterModel = new CharacterModel(new CharacterMoveController(), new CharacterKeyController(), te1eportComponent, this);
        characterModel.SetJumpDelay(jumpDelaySeconds);
        characterModel.SetFallDownTime(fallDownToOriginPosTime);
        characterModel.SetInteractDistance(interactDistance);

        SetEventRegister();
    }

    private void Update()
    {
        characterModel.UpdateJumpTimer(Time.deltaTime);
        characterModel.UpdateCheckJump(jumpForce);
        characterModel.UpdateMove(Time.deltaTime, speed);
        characterModel.UpdateFallDownTimer(Time.deltaTime);
        characterModel.UpdateCheckInteract();
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

    public void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Platform)
        {
            characterModel.ExitFloor();
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != (int)GameConst.GameObjectLayerType.TeleportGate)
            return;
        
        TeleportGateComponent teleportGateComponent = col.gameObject.GetComponent<TeleportGateComponent>();
        if (teleportGateComponent == null)
            return;

        characterModel.TriggerTeleportGate(teleportGateComponent);
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer != (int)GameConst.GameObjectLayerType.TeleportGate)
            return;

        TeleportGateComponent teleportGateComponent = col.gameObject.GetComponent<TeleportGateComponent>();
        if (teleportGateComponent == null)
            return;

        characterModel.ExitTeleportGate();
    }

    private void OnJump(float jumpForce)
    {
        FmodAudioManager.Instance.PlayOneShot("Jump");
        GetRigidbody.AddForce(new Vector2(0, jumpForce));
    }

    private void OnHorizontalMove(float moveValue)
    {
        transform.Translate(new Vector2(moveValue, 0));
    }
}