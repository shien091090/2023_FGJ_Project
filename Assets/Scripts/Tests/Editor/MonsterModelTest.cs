using NUnit.Framework;

public class MonsterModelTest
{
    [Test]
    //沒有被攻擊時為正常狀態
    public void not_be_attack()
    {
        MonsterModel monsterModel = new MonsterModel(1);

        CurrentMonsterStateShouldBe(monsterModel, MonsterState.Normal);
    }
    
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
    
    [Test]
    //被重複攻擊時, 暈眩時間重置
    public void be_attack_and_stun_and_be_attack_again()
    {
        MonsterModel monsterModel = new MonsterModel(1);

        CurrentMonsterStateShouldBe(monsterModel, MonsterState.Normal);

        monsterModel.BeAttack();
        monsterModel.UpdateStunTimer(0.5f);

        CurrentMonsterStateShouldBe(monsterModel, MonsterState.Stun);

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