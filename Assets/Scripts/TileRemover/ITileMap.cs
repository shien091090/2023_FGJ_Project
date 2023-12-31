using UnityEngine;
using UnityEngine.Tilemaps;

public interface ITileMap
{
    void SetTile(Vector3 pos, Tile tile);
    bool HaveTile(Vector3 pos);
}