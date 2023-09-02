using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRemoverComponent : MonoBehaviour
{
    [SerializeField] private int upRange;
    [SerializeField] private int downRange;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform character;

    private TileRemoverModel tileRemoverModel;

    private void Start()
    {
        tileRemoverModel = new TileRemoverModel(upRange, downRange);

        tileRemoverModel.OnRemoveTiles -= OnClearTile;
        tileRemoverModel.OnRemoveTiles += OnClearTile;
    }

    public void Update()
    {
        tileRemoverModel.UpdateRemoveTile(character.position);
    }

    private void OnClearTile(Vector3[] posArray)
    {
        foreach (Vector3 pos in posArray)
        {
            tilemap.SetTile(tilemap.WorldToCell(pos), null);
        }
    }
}