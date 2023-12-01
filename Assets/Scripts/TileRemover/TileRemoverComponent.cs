using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRemoverComponent : MonoBehaviour, ITileMap
{
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private Transform character;

    private MissingTextureManager missingTextureManager;
    private TileRemoverModel tileRemoverModel;

    public void SetTile(Vector3 pos, Tile tile)
    {
        targetTilemap.SetTile(targetTilemap.WorldToCell(pos), null);
        missingTextureManager.SubtractMissingTextureCount();
    }

    public bool HaveTile(Vector3 pos)
    {
        return targetTilemap.GetTile(targetTilemap.WorldToCell(pos)) != null;
    }

    private void Start()
    {
        missingTextureManager = MissingTextureManager.Instance;
        tileRemoverModel = TileRemoverModel.Instance;
        tileRemoverModel.BindView(this);
    }

    private void Update()
    {
        tileRemoverModel.UpdateRemoveTile(character.position);
    }
}