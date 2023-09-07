using SNShien.Common.AudioTools;
using UnityEngine;

public class TeleportGateComponent : MonoBehaviour, ITeleportGate
{
    [SerializeField] private Vector3 teleportPos;
    public Vector3 GetPos => transform.position;

    public void Teleport(IRigidbody target)
    {
        FmodAudioManager.Instance.PlayOneShot("Teleport");
        target.position = teleportPos;
        target.velocity = Vector2.zero;
    }
}