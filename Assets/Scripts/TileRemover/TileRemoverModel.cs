using System;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class TileRemoverModel
{
    private readonly int upRange;
    private readonly int downRange;

    public event Action<Vector3[]> OnRemoveTiles;

    public TileRemoverModel(int upRange, int downRange)
    {
        this.upRange = upRange;
        this.downRange = downRange;
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
            removePosArray[currentIndex] = new Vector3(centerPos.X, centerPos.Y + i, centerPos.Z);
            currentIndex += 1;
        }

        for (int i = 0; i < downRange; i++)
        {
            removePosArray[currentIndex] = new Vector3(centerPos.X, centerPos.Y - 1 - i, centerPos.Z);
            currentIndex += 1;
        }

        OnRemoveTiles?.Invoke(removePosArray);
    }
}