public class WallColliderHandler : IColliderHandler
{
    private readonly bool isRight;
    private readonly ICharacterModel model;

    public WallColliderHandler(bool isRight, ICharacterModel characterModel)
    {
        this.isRight = isRight;
        model = characterModel;
    }

    public void ColliderTriggerEnter(ICollider col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
            model.ColliderTriggerEnterWall(isRight);

        model.ColliderTriggerEnter(col);
    }

    public void ColliderTriggerExit(ICollider col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
            model.ColliderTriggerExitWall(isRight);

        model.ColliderTriggerExit(col);
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