public interface IColliderHandler
{
    public void ColliderTriggerEnter(ICollider col);
    public void ColliderTriggerExit(ICollider col);
    public void ColliderTriggerStay(ICollider col);
    void CollisionEnter(ICollision col);
    void CollisionExit(ICollision col);
}