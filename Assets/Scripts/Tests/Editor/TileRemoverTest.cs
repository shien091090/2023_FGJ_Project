using System;
using System.Numerics;
using NSubstitute;
using NUnit.Framework;

public class TileRemoverTest
{
    private Action<Vector3[]> removeTileEvent;

    [SetUp]
    public void Setup()
    {
        removeTileEvent = Substitute.For<Action<Vector3[]>>();
    }

    [Test]
    //清除範圍為0時, 不會有清除事件
    public void clear_range_is_0()
    {
        TileRemoverModel tileRemoverModel = new TileRemoverModel(0, 0);
        tileRemoverModel.OnRemoveTiles += removeTileEvent;

        tileRemoverModel.UpdateRemoveTile();

        removeTileEvent.DidNotReceive().Invoke(Arg.Any<Vector3[]>());
    }

    //有設定上下清除範圍
}