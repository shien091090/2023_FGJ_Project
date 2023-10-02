using System.Collections.Generic;
using UnityEngine;

public interface ICollision
{
    int Layer { get; }
    List<Vector2> ContactPoints { get; }
    T GetComponent<T>();
    bool CheckPhysicsOverlapCircle(Vector3 point, float radius, GameConst.GameObjectLayerType layerMask);
}