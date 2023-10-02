public class WallColliderHandler : IColliderHandler
{
    private readonly bool isRight;
    private readonly CharacterModel model;

    public WallColliderHandler(bool isRight, CharacterModel characterModel)
    {
        this.isRight = isRight;
        model = characterModel;
    }

    public void ColliderTriggerEnter(ICollider col)
    {
        model.ColliderTriggerEnterWall(isRight);
        model.ColliderTriggerEnter(col);
    }

    public void ColliderTriggerExit(ICollider col)
    {
        model.ColliderTriggerExitWall(isRight);
        model.ColliderTriggerEnter(col);
    }

    public void ColliderTriggerStay(ICollider col)
    {
        model.ColliderTriggerStay(col);
    }

    public void CollisionEnter(ICollision col)
    {
    }

    public void CollisionExit(ICollision col)
    {
    }
}