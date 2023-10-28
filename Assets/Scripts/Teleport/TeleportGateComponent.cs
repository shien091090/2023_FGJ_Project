using UnityEngine;

public class TeleportGateComponent : MonoBehaviour, ITeleportGate
{
    [SerializeField] private Vector3 teleportPos;
    public Vector3 GetPos => transform.position;
    public Vector3 GetTargetPos => teleportPos;
}