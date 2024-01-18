using UnityEngine;

public interface IGameObjectPool
{
    void SpawnGameObject(string prefabName, Vector3 position, FaceDirection faceDirection = FaceDirection.None);
}