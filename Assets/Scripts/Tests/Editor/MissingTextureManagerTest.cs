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

        ShouldCallRefreshRemainPercentText("0");
        ShouldCallRefreshProgress(0);
    }

    [Test]
    //初始化時設置目前進度為100%
    public void init_current_count_to_100_percent()
    {
        MissingTextureManager missingTextureManager = new MissingTextureManager(10, view);

        ShouldCallRefreshRemainPercentText("100");
        ShouldCallRefreshProgress(1);
    }

    [Test]
    //破圖數量減少時, 非0%或100%的狀況以四捨五入顯示
    public void subtract_missing_texture_count()
    {
        MissingTextureManager missingTextureManager = new MissingTextureManager(1000, view);

        missingTextureManager.SubtractMissingTextureCount(254);
        ShouldCallRefreshRemainPercentText("75");
        ShouldCallRefreshProgress(0.746f);
    }
    
    [Test]
    //破圖數量減少時, 即使四捨五入後為100%, 仍顯示99%
    public void subtract_missing_texture_count_to_100_percent()
    {
        MissingTextureManager missingTextureManager = new MissingTextureManager(1000, view);

        missingTextureManager.SubtractMissingTextureCount(1);
        ShouldCallRefreshRemainPercentText("99");
        ShouldCallRefreshProgress(0.999f);
    }

    private void ShouldCallRefreshProgress(float expectedProgress, int callTimes = 1)
    {
        view.Received(callTimes).RefreshProgress(expectedProgress);
    }

    private void ShouldCallRefreshRemainPercentText(string expectedRemainCountText, int callTimes = 1)
    {
        view.Received(callTimes).RefreshRemainPercentText(expectedRemainCountText);
    }
    //破圖數量減少時, 即使四捨五入後為0%, 仍顯示1%
    //破圖數量減少到0時, 發出通知事件
}