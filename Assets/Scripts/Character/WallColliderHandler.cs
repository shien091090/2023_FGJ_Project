using SNShien.Common.AdapterTools;

public class WallColliderHandler : ICollider2DHandler
{
    private readonly bool isRight;
    private readonly ICharacterModel model;

    public WallColliderHandler(bool isRight, ICharacterModel characterModel)
    {
        this.isRight = isRight;
        model = characterModel;
    }

    public void ColliderTriggerEnter2D(ICollider2DAdapter col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
            model.ColliderTriggerEnterWall(isRight);

        model.ColliderTriggerEnter2D(col);
    }

    public void ColliderTriggerExit2D(ICollider2DAdapter col)
    {
        if (col.Layer == (int)GameConst.GameObjectLayerType.Platform)
            model.ColliderTriggerExitWall(isRight);

        model.ColliderTriggerExit2D(col);
    }

    public void ColliderTriggerStay2D(ICollider2DAdapter col)
    {
        model.ColliderTriggerStay2D(col);
    }

    public void CollisionEnter2D(ICollision2DAdapter col)
    {
    }

    public void CollisionExit2D(ICollision2DAdapter col)
    {
    }
}