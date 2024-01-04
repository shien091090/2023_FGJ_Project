using UnityEngine;

public interface ITileRemoverModel
{
    void UpdateRemoveTile(Vector3 centerPos);
    void BindView(ITileMap view);
}