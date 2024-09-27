namespace RoundHero
{
    public enum EHeroID
    {
        Normal,
        MoreEnergyBuff,
        AddUnitMaxHP,
        SubUnitCardEnergy,
        UsSubHurtEnemySubDmg,
        UsCounterAtk,
        ReceiveLink,
        AroundUnitDeBuffUnEffect,
        AroundUnitBuffAddMore,
        PassBePassSubHP,
        UsAddHPEnemySubHP,
        UnitSubHP,
        HurtAcquireCard,
        HurtComsumeCard,
        Empty,
    }

    public enum EEnergyBuffType
    {
        NormalEnergyBuff,
        UnitState,
        GridProp,
    }

    public enum ENormalEnergyBuff
    {
        UnitAddCurHP,
        AcquireCard,
        UnitMove,
        UnitAddMaxHP,
        SubCurHP,
        SubCardEnergy,
        AcquireAllStandByCard,
        HandCardEnergyHalf,
        ConsumeCard,
        
    }
    public enum EEnergyBuffValue
    {
        HandCardCount,
        UnitCount,
        UsBuffCount,
        EnemyDeBuffCount,

    }
}