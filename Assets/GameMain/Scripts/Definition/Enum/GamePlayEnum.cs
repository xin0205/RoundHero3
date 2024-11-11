
namespace RoundHero
{
    public enum EHeroAttribute
    {
        CurHeart,
        MaxHeart,
        CurHP,
        MaxHP,
        Damage,
        // CurEnergy,
        // MaxEnergy,
        // RecoverEnergy,
        MaxCardCountEachRound,
        Coin,
        //Teleport,
        Refresh,
        Gem,
        Empty,
    }
    
    public enum EUnitAttribute
    {
        HP,
        MaxHP,
        BaseDamage,
        CardEnergy,
        Coin,
        Empty,

    }

    public enum ECardTriggerType
    {
        AcquireCard,
        ConsumeCard,
        AddCard,
        ToHand,
        ToStandBy,
        ToPass,
        EffectToComsume,
        StandByToComsume,
        ComsumeToStandBy,
        StandByToPass,
        AddSpecificCard,
        ConsumeSpecificCard,
        SelectCard,
        CardSubEnergy,
        Empty,                                                  
    }


    public enum EBlockType
    {
        Empty,
        Obstacle,
        Store,
        Enemy,

    }

    public enum ERewardType
    {
        GameBuff,
        Card,
        Prop,
    }

    public enum EGridType
    {
        Empty = 0,
        Obstacle = 1,
        Unit = 2,
        TemporaryUnit = 3,
        //Build,
        //Card = 4,
        //UnitState = 5,
        //Prop = 6,
        //Item,
    }


    public enum EShowState
    {
        Show,
        Unshow,
        Keep
    }

    public enum EBattleState
    {
        //Battle,
        End,
        UseCard,
        UnitSelectGrid,
        TacticSelectUnit,
        //ActionSelectUnit,
        //AttackSelectUnit,
        //MoveSelectUnit,
        MoveGrid,
        MoveUnit,
        FuneMoveUnit,
        ExchangeSelectGrid,
        EnemyAction,
        PlayerAction,
        EndTurn,
        EndRound,
        ActionExcuting,
        SelectHurtUnit,
    }
    
    public enum EActionProgress
    {
        UseCardTrigger,
        RoundStart,
        RoundStartUnit,
        RoundStartBuff,


        ActionStart,
        SoliderAttack,
        EnemyMove,
        ThirdUnitMove,
        EnemyAttack,
        ThirdUnitAttack,
        ActionEnd,
        RoundEnd,
        NotifyRoundEnd,
        Fly,
        PlayerAction,
        SoliderActiveAttack,
        SoliderAutoAttack,
    }

    public enum EDirection
    {
        Horizonal,
        Vertial,
        XLeft,
        XRight,
    }

    public enum EBattleUnitPos
    {
        Left,
        Right,
        Center,
    }

    public enum EEnemyType
    {
        Normal,
        Elite,
        Boss,
    }

    public enum EActionType
    {
        Self,
        Around,//周围
       
        Cross_Short,//短十字
        Cross2Long,//长十字
        Cross2Extend,//十字_发射
        UnitMaxDirect,

        HeroDirect,
        Direct8,//8方向
        Direct82Extend,
        Row,
        Column,
        All,
        UnFullCurHPUnit,
        Cross_Long_Empty,
        Direct8_Long_Empty,

        Empty,
        
        Null,
        //HeroDirect_Extend,
        //HeroDirect,
        //X_Extend,//X_发射
        //X_Short,//短X
        //X_Long,//长X
        //Direct8_Extend,//8方向发射
        Direct_Extend,
        //Parabola,//抛物线
        UnitMaxCrossExtend,
        UnitMaxAround,
        Select,
        //UnitMaxXExtend,
        LessCurHPUnit,
    }

    public enum EActionTarget
    {
        Fast,
        Near,
        
    }

    // public enum EItemType
    // {
    //     Card,
    //     Bless,
    //     Fune,
    //     HP,
    //     Heart,
    //     Coin
    // }

    public enum ECardType
    {
        Unit,
        Tactic,
        State,
    }

    public enum ETriggerType
    {
        Buff,
        Link,
        Empty,
    }

    public enum EBuffTriggerType
    {
        BePass,
        Pass,
        Move,
        Attack,
        Hurt,
        Dead,
        Kill,
        UseCard,
        Link,
        Trigger,
        RoundStart,
        RoundEnd,
        Spec,
        //Start,
        //End,
        //Action,
        // FirstUnit,
        // AttackAuxiliary,
        // PassAuxiliary,
        Empty,
        ActionEnd,
        SelfEnterRange,
        SelfLeaveRange,
        OtherEnterRange,
        OtherLeaveRange,
        ActiveAttack,
        Collision,
        SelectUnit,
        SelectGrid,
        Use,
        SelectCard,
        AutoAttack,
        AddDeBuff,
        SingleRound,
        ChangeHP,
        TacticSelectUnit,
        RangeTrigger,
    }

    

    public enum EGridPropID
    {
        MoveDirect,
        Empty,

    }

    

    public enum ERelativePos
    {
        Left,
        LeftUp,
        Up,
        RightUp,
        Right,
        RightDown,
        Down,
        LeftDown,
    }

    public enum ERelativeCamp
    {
        Us,
        Enemy,
        Third,
        Empty,
    }
    
    public enum EUnitCamp
    {
        Player1,
        Player2,
        Enemy,
        Third,
        Empty,
    }
    
    public enum EUnitRole
    {
        Hero,
        Staff,
    }
    
    // public enum ERelativeUnitCamp
    // {
    //     Self,
    //     Enemy,
    // }

    public enum ETriggerTarget
    {
        Effect,
        Action,
        Trigger,
        Hero,
        All,
        InRange,
        Vertical,
        EffectUnitDirect,
        DeBuffMax,
        MoveMax,
        Direct,
        LessCurHPUnit,
        Empty,
    }

    public enum EUnitState
    { 
        /*
         *点燃 回合伤害
        */
        HurtRoundStart,
        HurtEachMove,
        UnMove,
        UnAtk,
        AtkPassUs,
        AtkPassEnemy,
        HurtSubDmg,
        HurtAddDmg,
        AddDmg,
        SubDmg,
        CounterAtk,
                
        DeBuffUnEffect,
        BuffUnEffect,
        SubHPAddSelfHP,
                
        Fly,
        UnEffectLink,
        UnBePass,
        CollideUnHurt,
        UnBeMove,
        
        UnRecover,
        RecoverHP,
        UnHurt,
        DoubleDmg,
        ActiveAtk,
        AutoAtk,
        
        BuffAddMore,
        
        ActiveAction,
        Empty,
    }

    public enum ERoleAttribute
    {
        HP,
        HPMax,
        Empty,

    }

    public enum ETriggerDataType
    {
        Hero,
        RoleAttribute,
        RoleState,
        RoundRoleState,
        Card,
        Link,
        RemoveUnit,
        RoundBuff,
        Curse,
        Empty,
    }
    
    public enum ETriggerDataSubType
    {
        Bless,
        Unit,
        Curse,
        Empty,
    }

    public enum EUnitType
    {
        Role,
        Device,
    }

    public enum EHPRefreshType
    {
       Round,
       Trigger,
    }

    public enum EBuffValueType
    {
        Atrb,
        Hero,
        State,
        Card,
        RoundState,
        // UnitAttribute,
        // HeroAttribute,
        // UnitState,
        // RoundUnitState,
        // PosUnitState,
        // BattleUnitState,
        // RandomPositiveUnitState,
        // RandomNegativeUnitState,
        // GridProp,
        // Card,
        Empty,
    }

    public enum ELinkType
    {
        Receive,
        Send,
    }

    public enum EUnitStateEffectType
    {
        Positive,
        Negative,
    }

    public enum EGamMode
    {
        PVE,
        PVP,
    }

    public enum EUnitActionState
    {
        Idle,
        Run,
        Attack,
        Hurt,
        Dead,
        RunAttack,
        RunHurt,
        Dodge,
        Recover,
        Quit,
        Fly,
        Empty,
    }

    public enum EActionDataType
    {
        Unit,
        UnitState,
        Curse,
        Bless,
    }

    public enum EFlyType
    {
        OtherBack,
        SelfBack,
        OtherClose,
        SelfClose,
        //AllBack,
        //AllClose,
        //CrossOtherBack1,
        Exchange,
        SelfCross,
        BackToSelf,
        OtherBack1,
        SelfBack1,
        OtherClose1,
        SelfClose1,
        Empty,
    }

    public enum EMoveType
    {
        Run,
        Fly,
    }

    public enum EValueType
    {
        EffectUnitAttack,
        Empty,
        HandCardCount,
        UnitCount,
        UsBuffCount,
        EnemyDeBuffCount,
        EffectUnitHurt,
    }

    public enum EMapSite
    {
        NormalBattle,
        EliteBattle,
        BossBattle,
        Store,
        Rest,
        Treasure,
        Event,
        Empty,
    }

    public enum EEventSubType
    {
        Random,
        Appoint,
        Value,
    }

    public enum EEventType
    {
        Card_Remove,
        Card_Change,
        Card_Copy,
        
        Random_UnitCard,
        Random_TacticCard,
        Random_Fune,
        Random_Bless,
        
        Appoint_UnitCard,
        Appoint_TacticCard,
        Appoint_Fune,
        Appoint_Bless,
        
        AddCoin,
        AddHeroMaxHP,
        AddHeroCurHP,
        
        NegativeCard,
        SubCoin,
        SubHeroMaxHP,
        SubHeroCurHP,
    }

    public enum ERandomType
    {
        Event,
        Store,
        Rest,
        NormalBattle,
        
    }
    
    public enum EBattleEventExpressionType
    {
        Selection,
        Game,
    }
    
    public enum EBattleEventGameType
    {
        Line,
        Circle,
        Time,
        Quick,
        Select,
    }

    public enum EBattleEvent
    {
        Line_Y_O_Y,
        Line_YN_O_YN,
        Line_N_O_N,
        Line_Y_N_Y,
        Line_N_Y_N,
        
        Time_O_N_Y_O,
        
        QuickClick_N_Y_N,
        QuickClick_O_Y_O,
        QuickClick_N_O_N,
        QuickClick_Y_Y_Y,
        QuickClick_YN_YN_YN,
        
        QuickSelect_Y_O,
        QuickSelect_Y_N,
        QuickSelect_O_N,
        
        Select_Y_Y_O,
        Select_YN_YN_O,
        Select_N_N,
    }

    // public enum EBattleEvent_Select
    // {
    //
    //     Select_Y_Y,
    //     Select_YN_YN_O,
    //     Select_N_N,
    // }

    
    public enum EBattleEventYNType
    {
        Y,
        N,
        O,
        YN,
    }

    public enum EGridPropEffectType
    {
        Once,
        Round,
        Forever,
    }

    public enum EAttackTarget
    {
        Hero,
        MoreEnemy,
        MoreUs,
        MoreUnit,
        LessEnemy,
        LessUs,
        LessUnit,
        PassMoreEnemy,
        PassMoreUs,
        PassMoreUnit,
        FastEnemy,
        FastUs,
        FastUnit,
        CloseEnemy,
        CloseUs,
        CloseUnit,
        LessHPEnemy,
        LessHPUs,
        LessHPUnit,

    }

    public enum EAttackType
    {
        Dynamic,
        Lock,
    }



}