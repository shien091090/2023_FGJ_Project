using UnityEngine;

public class ColliderAdapter : ICollider
{
    public int Layer => collider.gameObject.layer;
    private readonly Collider2D collider;

    public ColliderAdapter(Collider2D collider)
    {
        this.collider = collider;
    }

    public T GetComponent<T>()
    {
        return collider.GetComponent<T>();
    }
}