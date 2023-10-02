using UnityEngine;

public interface ICollider
{
    int Layer { get; }
    Vector3 Position { get; }
    T GetComponent<T>();
}