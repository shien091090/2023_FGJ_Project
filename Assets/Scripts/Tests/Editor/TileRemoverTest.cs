using System;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRemoverTest
{
    private TileRemoverModel tileRemoverModel;
    private ITileMap tileMap;

    [SetUp]
    public void Setup()
    {
        tileMap = Substitute.For<ITileMap>();
    }


    [Test]
    //清除範圍為0時, 不會有清除事件
    public void clear_range_is_0()
    {
        tileRemoverModel = CreateModel(0, 0);

        tileRemoverModel.UpdateRemoveTile(Vector3.zero);

        ShouldNotCheckAndSetAnyTile();
    }

    [Test]
    //有設定上下清除範圍
    public void have_clear_range_setting()
    {
        tileRemoverModel = CreateModel(2, 4);

        tileRemoverModel.UpdateRemoveTile(new Vector3(2, 1, 0));

        ShouldCheckHaveTiles(
            new Vector3(2, 1, 0),
            new Vector3(2, 3, 0),
            new Vector3(2, 2, 0),
            new Vector3(2, 0, 0),
            new Vector3(2, -1, 0),
            new Vector3(2, -2, 0),
            new Vector3(2, -3, 0));
    }

    [Test]
    //只有設定上清除範圍, 下清除範圍為0
    public void only_have_up_clear_range()
    {
        tileRemoverModel = CreateModel(3, 0);

        tileRemoverModel.UpdateRemoveTile(new Vector3(4, 0, 0));

        ShouldCheckHaveTiles(
            new Vector3(4, 0, 0),
            new Vector3(4, 3, 0),
            new Vector3(4, 2, 0),
            new Vector3(4, 1, 0));
    }

    [Test]
    //只有設定下清除範圍, 上清除範圍為0
    public void only_have_down_clear_range()
    {
        tileRemoverModel = CreateModel(0, 3);

        tileRemoverModel.UpdateRemoveTile(new Vector3(4, 0, 0));

        ShouldCheckHaveTiles(
            new Vector3(4, 0, 0),
            new Vector3(4, -1, 0),
            new Vector3(4, -2, 0),
            new Vector3(4, -3, 0));
    }

    private void ShouldCheckHaveTiles(params Vector3[] posArray)
    {
        foreach (Vector3 pos in posArray)
        {
            tileMap.Received(1).HaveTile(pos);
        }
    }

    private void ShouldNotCheckAndSetAnyTile()
    {
        tileMap.DidNotReceive().HaveTile(Arg.Any<Vector3>());
        tileMap.DidNotReceive().SetTile(Arg.Any<Vector3>(), Arg.Any<Tile>());
    }

    private TileRemoverModel CreateModel(int upRange, int downRange)
    {
        tileRemoverModel = new TileRemoverModel(upRange, downRange, tileMap);

        return tileRemoverModel;
    }
}