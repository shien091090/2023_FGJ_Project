using System;
using UnityEngine;

public class TileRemoverModel
{
    private readonly int upRange;
    private readonly int downRange;
    private readonly ITileMap tileMap;
    private int tilesCount;

    public TileRemoverModel(int upRange, int downRange, ITileMap tileMap)
    {
        this.upRange = upRange;
        this.downRange = downRange;
        this.tileMap = tileMap;
        tilesCount = tileMap.GetTotalTilesCount();
    }

    public void UpdateRemoveTile(Vector3 centerPos)
    {
        if (upRange == 0 && downRange == 0)
            return;

        Vector3[] removePosArray = new Vector3 [upRange + downRange + 1];
        removePosArray[0] = centerPos;

        int currentIndex = 1;
        for (int i = upRange; i > 0; i--)
        {
            removePosArray[currentIndex] = new Vector3(centerPos.x, centerPos.y + i, centerPos.z);
            currentIndex += 1;
        }

        for (int i = 0; i < downRange; i++)
        {
            removePosArray[currentIndex] = new Vector3(centerPos.x, centerPos.y - 1 - i, centerPos.z);
            currentIndex += 1;
        }

        foreach (Vector3 pos in removePosArray)
        {
            if (tileMap.HaveTile(pos))
            {
                tileMap.SetTile(pos, null);
                tilesCount--;
            }
        }
    }
}