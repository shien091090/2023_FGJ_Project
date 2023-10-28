using UnityEngine;

public interface ITeleportGate
{
    Vector3 GetPos { get; }
    Vector3 GetTargetPos { get; }
}