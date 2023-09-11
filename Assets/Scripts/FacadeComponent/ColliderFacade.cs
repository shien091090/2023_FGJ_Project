using UnityEngine;

public class ColliderFacade : ICollider
{
    public int Layer => collider.gameObject.layer;
    private readonly Collider2D collider;

    public ColliderFacade(Collider2D collider)
    {
        this.collider = collider;
    }

    public T GetComponent<T>()
    {
        return collider.GetComponent<T>();
    }
}