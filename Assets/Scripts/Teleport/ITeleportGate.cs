using UnityEngine;

public interface ITeleportGate
{
    void Teleport(IRigidbody target);
    Vector3 GetPos { get; }
}