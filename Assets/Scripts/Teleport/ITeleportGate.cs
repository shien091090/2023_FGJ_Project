using UnityEngine;

public interface ITeleportGate
{
    void Teleport(ITransform target);
    Vector3 GetPos { get; }
}