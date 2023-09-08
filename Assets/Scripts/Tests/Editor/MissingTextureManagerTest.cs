using NSubstitute;
using NUnit.Framework;

public class MissingTextureManagerTest
{
    private IMissingTextureManagerView view;

    [SetUp]
    public void Setup()
    {
        view = Substitute.For<IMissingTextureManagerView>();
    }

    [Test]
    //設定總數量為0時, 初始進度為0%
    public void set_total_count_to_0()
    {
        MissingTextureManager missingTextureManager = new MissingTextureManager(0, view);

        ShouldCallRefreshRemainCount(1, "0");
    }

    private void ShouldCallRefreshRemainCount(int callTimes, string expectedRemainCountText)
    {
        view.Received(callTimes).RefreshRemainCount(expectedRemainCountText);
    }

    //初始化時設置目前進度為100%
    //破圖數量減少時, 即使四捨五入後為100%, 仍顯示99%
    //破圖數量減少時, 即使四捨五入後為0%, 仍顯示1%
    //破圖數量減少時, 非0%或100%的狀況以四捨五入顯示
    //破圖數量減少到0時, 發出通知事件
}