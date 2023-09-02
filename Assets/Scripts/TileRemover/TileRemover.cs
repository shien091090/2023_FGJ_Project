using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRemover : MonoBehaviour
{
    public Tilemap tilemap;

    public void UpdateClearTile(Vector3 centerPos)
    {
        Vector3Int centerTileMapPos = tilemap.WorldToCell(centerPos);

        tilemap.SetTile(centerTileMapPos, null);
        tilemap.SetTile(new Vector3Int(centerTileMapPos.x + 1, centerTileMapPos.y + 1, centerTileMapPos.z), null);
        tilemap.SetTile(new Vector3Int(centerTileMapPos.x + 1, centerTileMapPos.y, centerTileMapPos.z), null);
        tilemap.SetTile(new Vector3Int(centerTileMapPos.x + 1, centerTileMapPos.y - 1, centerTileMapPos.z), null);
        tilemap.SetTile(new Vector3Int(centerTileMapPos.x, centerTileMapPos.y - 1, centerTileMapPos.z), null);
        tilemap.SetTile(new Vector3Int(centerTileMapPos.x - 1, centerTileMapPos.y - 1, centerTileMapPos.z), null);
        tilemap.SetTile(new Vector3Int(centerTileMapPos.x - 1, centerTileMapPos.y, centerTileMapPos.z), null);
        tilemap.SetTile(new Vector3Int(centerTileMapPos.x - 1, centerTileMapPos.y + 1, centerTileMapPos.z), null);
        tilemap.SetTile(new Vector3Int(centerTileMapPos.x, centerTileMapPos.y + 1, centerTileMapPos.z), null);
    }
}