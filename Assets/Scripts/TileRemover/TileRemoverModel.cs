using UnityEngine;

public class TileRemoverModel : ITileRemoverModel
{
    private readonly IGameSetting gameSetting;
    private ITileMap tileMap;

    public TileRemoverModel(IGameSetting gameSetting)
    {
        this.gameSetting = gameSetting;
    }

    public void UpdateRemoveTile(Vector3 centerPos)
    {
        if (gameSetting.UpRange == 0 && gameSetting.DownRange == 0)
            return;

        Vector3[] removePosArray = new Vector3 [gameSetting.UpRange + gameSetting.DownRange + 1];
        removePosArray[0] = centerPos;

        int currentIndex = 1;
        for (int i = gameSetting.UpRange; i > 0; i--)
        {
            removePosArray[currentIndex] = new Vector3(centerPos.x, centerPos.y + i, centerPos.z);
            currentIndex += 1;
        }

        for (int i = 0; i < gameSetting.DownRange; i++)
        {
            removePosArray[currentIndex] = new Vector3(centerPos.x, centerPos.y - 1 - i, centerPos.z);
            currentIndex += 1;
        }

        foreach (Vector3 pos in removePosArray)
        {
            if (tileMap.HaveTile(pos))
                tileMap.SetTile(pos, null);
        }
    }

    public void BindView(ITileMap view)
    {
        tileMap = view;
    }
}