public interface IColliderHandler
{
    void ColliderTriggerEnter(ICollider col);
    void ColliderTriggerExit(ICollider col);
    void CollisionEnter(ICollision col);
    void CollisionExit(ICollision col);
}