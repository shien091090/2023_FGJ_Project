using System;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

public class TileRemoverTest
{
    private Action<Vector3[]> removeTileEvent;
    private TileRemoverModel tileRemoverModel;

    [SetUp]
    public void Setup()
    {
        removeTileEvent = Substitute.For<Action<Vector3[]>>();
    }

    [Test]
    //清除範圍為0時, 不會有清除事件
    public void clear_range_is_0()
    {
        tileRemoverModel = CreateModel(0, 0);

        tileRemoverModel.UpdateRemoveTile(Vector3.zero);

        removeTileEvent.DidNotReceive().Invoke(Arg.Any<Vector3[]>());
    }

    [Test]
    //有設定上下清除範圍
    public void have_clear_range_setting()
    {
        tileRemoverModel = CreateModel(2, 4);

        tileRemoverModel.UpdateRemoveTile(new Vector3(2, 1, 0));

        removeTileEvent.Received(1).Invoke(Arg.Is<Vector3[]>(x =>
            x.Length == 7 &&
            x[0] == new Vector3(2, 1, 0) &&
            x[1] == new Vector3(2, 3, 0) &&
            x[2] == new Vector3(2, 2, 0) &&
            x[3] == new Vector3(2, 0, 0) &&
            x[4] == new Vector3(2, -1, 0) &&
            x[5] == new Vector3(2, -2, 0) &&
            x[6] == new Vector3(2, -3, 0)));
    }

    [Test]
    //只有設定上清除範圍, 下清除範圍為0
    public void only_have_up_clear_range()
    {
        tileRemoverModel = CreateModel(3, 0);

        tileRemoverModel.UpdateRemoveTile(new Vector3(4, 0, 0));

        removeTileEvent.Received(1).Invoke(Arg.Is<Vector3[]>(x =>
            x.Length == 4 &&
            x[0] == new Vector3(4, 0, 0) &&
            x[1] == new Vector3(4, 3, 0) &&
            x[2] == new Vector3(4, 2, 0) &&
            x[3] == new Vector3(4, 1, 0)));
    }

    [Test]
    //只有設定下清除範圍, 上清除範圍為0
    public void only_have_down_clear_range()
    {
        tileRemoverModel = CreateModel(0, 3);

        tileRemoverModel.UpdateRemoveTile(new Vector3(4, 0, 0));

        removeTileEvent.Received(1).Invoke(Arg.Is<Vector3[]>(x =>
            x.Length == 4 &&
            x[0] == new Vector3(4, 0, 0) &&
            x[1] == new Vector3(4, -1, 0) &&
            x[2] == new Vector3(4, -2, 0) &&
            x[3] == new Vector3(4, -3, 0)));
    }

    private TileRemoverModel CreateModel(int upRange, int downRange)
    {
        tileRemoverModel = new TileRemoverModel(upRange, downRange);
        tileRemoverModel.OnRemoveTiles += removeTileEvent;

        return tileRemoverModel;
    }
}