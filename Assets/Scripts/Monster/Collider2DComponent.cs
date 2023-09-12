using UnityEngine;

public class ColliderComponent : MonoBehaviour
{
    private IColliderHandler handler;
    private ColliderHandleType handleType;

    public void OnCollisionEnter2D(Collision2D col)
    {
    }

    public void OnCollisionExit2D(Collision2D col)
    {
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
    }

    public void OnTriggerExit2D(Collider2D col)
    {
    }

    public void InitHandler(IColliderHandler handler, ColliderHandleType handleType)
    {
        this.handler = handler;
        this.handleType = handleType;
    }
}
