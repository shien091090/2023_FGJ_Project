using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRemoverComponent : MonoBehaviour, ITileMap
{
    [SerializeField] private int upRange;
    [SerializeField] private int downRange;
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private Transform character;

    private TileRemoverModel tileRemoverModel;

    public int GetTotalTilesCount()
    {
        return MissingTextureManager.Instance.GetTotalMissingTextureCount;
    }

    public void SetTile(Vector3 pos, Tile tile)
    {
        targetTilemap.SetTile(targetTilemap.WorldToCell(pos), null);
        MissingTextureManager.Instance.SubtractMissingTextureCount();
    }

    public bool HaveTile(Vector3 pos)
    {
        return targetTilemap.GetTile(targetTilemap.WorldToCell(pos)) != null;
    }

    private void Start()
    {
        tileRemoverModel = new TileRemoverModel(upRange, downRange, this);
    }

    private void Update()
    {
        tileRemoverModel.UpdateRemoveTile(character.position);
    }
}