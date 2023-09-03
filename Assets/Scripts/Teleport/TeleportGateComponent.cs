using SNShien.Common.AudioTools;
using UnityEngine;

public class TeleportGateComponent : MonoBehaviour, ITeleportGate
{
    [SerializeField] private Vector3 teleportPos;
    public Vector3 GetPos => transform.position;

    public void Teleport(ITransform target)
    {
        FmodAudioManager.Instance.PlayOneShot("Teleport");
        target.position = teleportPos;
    }
}