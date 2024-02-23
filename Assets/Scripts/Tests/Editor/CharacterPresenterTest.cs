using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.AdapterTools;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;

public class CharacterPresenterTest
{
    private CharacterPresenter presenter;
    private ICharacterModel characterModel;
    private IGameObjectPool gameObjectPool;
    private IAudioManager audioManager;
    private IGameSetting gameSetting;
    private IDeltaTimeGetter deltaTimeGetter;
    private ICharacterView characterView;

    [SetUp]
    public void Setup()
    {
        characterModel = Substitute.For<ICharacterModel>();
        gameObjectPool = Substitute.For<IGameObjectPool>();
        audioManager = Substitute.For<IAudioManager>();
        gameSetting = Substitute.For<IGameSetting>();
        deltaTimeGetter = Substitute.For<IDeltaTimeGetter>();
        characterView = Substitute.For<ICharacterView>();

        presenter = new CharacterPresenter(characterModel, gameObjectPool, audioManager, gameSetting, deltaTimeGetter);

        presenter.BindView(characterView);
    }

    [Test]
    //角色初始狀態預設面向右
    public void default_face_right()
    {
        ShouldFaceRight(true);
        FaceDirectionScaleShouldBe(1);
    }

    [Test]
    [TestCase(0.5f, 1, -0.5f, -1)]
    [TestCase(-0.5f, -1, 0.5f, 1)]
    [TestCase(-0.5f, -1, -0.3f, -1)]
    //角色變換面向
    public void change_face(float firstMoveValue, int fistExpectedScale, float secondMoveValue, int secondExpectedScale)
    {
        FaceDirectionScaleShouldBe(1);

        presenter.PlayerMoveEffect(firstMoveValue);

        FaceDirectionScaleShouldBe(fistExpectedScale);

        presenter.PlayerMoveEffect(secondMoveValue);

        FaceDirectionScaleShouldBe(secondExpectedScale);
    }

    [Test]
    [TestCase(0.5f, true, 1)]
    [TestCase(-1, false, -1)]
    //角色停止後, 維持原本面向
    public void keep_face_when_stop(float moveValue, bool expectedIsFaceRight, int expectedScale)
    {
        presenter.PlayerMoveEffect(moveValue);

        ShouldFaceRight(expectedIsFaceRight);
        FaceDirectionScaleShouldBe(expectedScale);

        presenter.PlayerMoveEffect(0);

        ShouldFaceRight(expectedIsFaceRight);
        FaceDirectionScaleShouldBe(expectedScale);
    }

    private void FaceDirectionScaleShouldBe(int expectedScale)
    {
        int argument = (int)characterView
            .ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == "SetFaceDirectionScale")
            .ToList()
            .Last()
            .GetArguments()[0];

        Assert.AreEqual(expectedScale, argument);
    }

    private void ShouldFaceRight(bool expectedIsFaceRight)
    {
        Assert.AreEqual(expectedIsFaceRight, presenter.IsFaceRight);
    }
}