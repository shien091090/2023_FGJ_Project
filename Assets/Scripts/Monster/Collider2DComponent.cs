using UnityEngine;

public class ColliderComponent : MonoBehaviour
{
    [SerializeField] private ColliderHandleType handleType;
    private IColliderHandler handler;

    public void InitHandler(IColliderHandler handler)
    {
        this.handler = handler;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (handleType == ColliderHandleType.Collision)
            handler?.CollisionEnter(new CollisionAdapter(col));
    }

    public void OnCollisionExit2D(Collision2D col)
    {
        if (handleType == ColliderHandleType.Collision)
            handler?.CollisionExit(new CollisionAdapter(col));
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (handleType == ColliderHandleType.Trigger)
            handler?.ColliderTriggerEnter(new ColliderAdapter(col));
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (handleType == ColliderHandleType.Trigger)
            handler?.ColliderTriggerExit(new ColliderAdapter(col));
    }
}