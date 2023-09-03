using NUnit.Framework;

public class MonsterModelTest
{
    [Test]
    //被攻擊時, 陷入暈眩狀態, 一段時間後解除狀態
    public void be_attack_and_stun()
    {
        MonsterModel monsterModel = new MonsterModel(1);

        CurrentMonsterStateShouldBe(monsterModel, MonsterState.Normal);

        monsterModel.BeAttack();
        monsterModel.UpdateStunTimer(0.5f);

        CurrentMonsterStateShouldBe(monsterModel, MonsterState.Stun);

        monsterModel.UpdateStunTimer(0.5f);

        CurrentMonsterStateShouldBe(monsterModel, MonsterState.Normal);
    }

    private void CurrentMonsterStateShouldBe(MonsterModel monsterModel, MonsterState expectedState)
    {
        Assert.AreEqual(expectedState, monsterModel.CurrentState);
    }
    //陷入暈眩時不移動
}