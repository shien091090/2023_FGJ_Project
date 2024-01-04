using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class TileRemoverComponent : MonoBehaviour, ITileMap
{
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private Transform character;

    [Inject] private ITileRemoverModel tileRemoverModel;
    [Inject] private IMissingTextureManager missingTextureManager;

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
        tileRemoverModel.BindView(this);
    }

    private void Update()
    {
        tileRemoverModel.UpdateRemoveTile(character.position);
    }
}