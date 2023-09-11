using UnityEngine;

public interface ICollision
{
    int Layer { get; }
    T GetComponent<T>();
    bool CheckPhysicsOverlapCircle(Vector3 point, float radius, GameConst.GameObjectLayerType layerMask);
}