﻿namespace RoundHero
{
    // public enum EMonsterID
    // {
    //     AutoAttack_Around_Enemy_BeTrigger_CurHP,
    //     Round_Attack_Around,
    //     Round_Attack_Direct8_Extend,
    //     Move_Attack_BePass,
    //     Move_Attack_Pass,
    //     Round_Attack_CrossExtend,
    //     Round_Attack_XExtend,
    //     Round_Attack_UnitMaxDirect,
    //     Round_Recover_CrossExtend,
    //     Round_Recover_XExtend,
    //
    // }
    
    // public enum EFuneID
    // {
    //     SubEnergy,
    //     AvoidDead,
    //     AddMaxHP,
    //     AddDamage,
    //     FirstRound,
    //     MoveInRound,
    //     Fly,
    //     Obstacle,
    //     EachRound_AddCurHP,
    //     MultiplyMaxHP,
    //     Link_X_Extend_Send,
    //     Link_Cross_Extend_Send,
    //     Link_X_Extend_Receive,
    //     Link_Cross_Extend_Receive,
    //     InHand_AddHeroCurHP,
    //     InHand_AcquireCard,
    //     InHand_UnPass,
    //     Use_AddCurHP_SubCurHP,
    //     Use_ToStandBy,
    //     Use_CopyToPass,
    //     Kill_AddCurHP,
    //     Kill_SubEnergy,
    //     Kill_AddHeroCurHP,
    //     Kill_AddDamage,
    //     Kill_ToHand,
    //     Kill_AcquireNewCard,
    //     Kill_RemoveCard,
    //     Kill_ConsumeToStandBy,
    //     Kill_AddCoin,
    //     Kill_Dodge,
    //     Kill_Shield,
    //     Kill_CounterAttack,
    //     Dead_BuffDeBuff,
    //     Dead_UnAttack,
    //     Dead_UnMove,
    //     Dead_RemoveCard,
    //     Dead_ToHand,
    //     Dead_AcquirePassCards,
    //     Dead_Coin,
    //     Attack_HurtRoundStart,
    //     Attack_EachRound,
    //     Attack_AttackPassEnemy_AttackPassUs,
    //     Hurt_AddDamage,
    //     Hurt_HurtSubDamage,
    //     Hurt_SubDamage,
    //     Hurt_HurtAddDamage,
    //     Empty,
    //     
    //     
    //     // AddCurHP,
    //     // AddMaxHP,
    //     // AddHurt,
    //     // FirstRound,
    //     // MoveInRound,
    //     // Fly,
    //     // Obstacle,
    //     // AvoidDead,
    //     // EachRound_AddCurHP,
    //     // Attack_UnMove,
    //     // Attack_UnAttack,
    //     // Attack_AttackPassUs,
    //     // Hurt_AttackPassEnemy,
    //     // Link_Receive_Around_Us,
    //     // Link_Receive_XShort_Us,
    //     // InHand_NoPass,
    //     // InHand_AcquireCard,
    //     // InHand_AddHeroCurHP,
    //     // Use_ToStandbyCards,
    //     // Use_UnitCurHP,
    //     // Use_CopyToPassCards,
    //     // Kill_FullUnitCurHP,
    //     // Kill_AddHeroCurHP,
    //     // Kill_ToHandCards,
    //     // Kill_AddCoin,
    //     // Kill_AddAttack,
    //     // Kill_RemoveCard,
    //     // Kill_SelectNewCard,
    //     //
    //     // Dead_AcquireEnemyCoin,
    //     // Dead_AttackEnemy,
    //     // Dead_UnAttackenemy,
    //     // Dead_RemoveCard,
    //     // Dead_ToHandCards,
    //     // Dead_AcquirePassCards,
    //     // Dead_FireAndPoison,
    //     // Null,
    //     // AttackHorizontalMove,
    //     // AttackBackMove,
    //     // UseCardCrossMove,
    //     // DeadXMove,
    // }

    // public enum ECardID
    // {
    //     BePass_Pass_Enemy_BeTrigger_CurHP,
    //     BePass_Enemy_BeTrigger_CurHP_BeTriggerAttack,
    //     Pass_Enemy_All_CurHP,
    //     BePass_Us_BeTrigger_CurHP,
    //     Pass_Enemy_Trigger_Vertical_CurHP,
    //     Pass_Enemy_BeTrigger_HurtRoundStart,
    //     BePass_Enemy_BeTrigger_HurtEachMove,
    //     Pass_BePass_Us_Trigger_AttackPassEnemy,
    //     Pass_BePass_Enemy_Trigger_CounterAttack,
    //     Pass_Enemy_Us_BeTrigger_AttackPassUs,
    //     Pass_Us_BeTrigger_Hero_HurtSubDamage,
    //     Pass_Enemy_Hero_AcquireCard,
    //     BePass_Enemy_BeTrigger_RemoveCard,
    //     AutoAttack_UnitMaxDirect_EnemyUs_BeTrigger_CurHP,
    //     AutoAttack_Cross_Extend_UsEnemy_BeTrigger_CurHP,
    //     AutoAttack_Direct8_Enemy_DeBuffMax_CurHP,
    //     AutoAttack_Direct8_Enemy_MoveMax_CurHP,
    //     AutoAttack_Around_Enemy_BeTrigger_HurtEachMove,
    //     AutoAttack_Cross_Extend_Us_BeTrigger_AttackPassEnemy_Enemy_AttackPassUs,
    //     AutoAttack_Around_Enemy_BeTrigger_AttackPassEnemy,
    //     AutoAttack_Around_Enemy_BeTrigger_HurtAddDamage,
    //     AutoAttack_UsMaxDirect_Us_BeTrigger_AddDamage_Enemy_SubDamage,
    //     AutoAttack_Around_Us_BeTrigger_HurtSubDamage,
    //     AutoAttack_Around_Enemy_Hero_Coin,
    //     AutoAttack_Cross_Short_Enemy_Hero_NewCard,
    //     Move_Around_EnemyBeTrigger_CurHP,
    //     Move_Around_Us_BeTrigger_CurHP,
    //     Hurt_Around_Enemy_BeTrigger_CounterAttack,
    //     Hurt_Around_UsEnemy_InRange_CurHP,
    //     Attack_Cross_Short_Enemy_BeTrigger_UnAttack,
    //     Hurt_Cross_Short_Enemy_BeTrigger_HurtAddDamage,
    //     Hurt_Around_Enemy_BeTrigger_SubDamage,
    //     Hurt_Around_Enemy_BeTrigger_AddDamage,
    //     Hurt_Self_Us_Hero_AcquireCard,
    //     OtherEnterRange_Around_UsEnemy_BeTrigger_CurHP,
    //     SelfLeaveRange_Empty_Enemy_Us_BeTrigger_CurHP,
    //     SelfEnterRange_Empty_UsEnemy_BeTrigger_CurHP,
    //     OtherEnterRange_Empty_Enemy_BeTrigger_HurtRoundStart_OtherLeaveRange_Us_HurtRoundStart,
    //     OtherEnterRange_SelfLeaveRange_Empty_Enemy_BeTrigger_HurtEachMove,
    //     SelfEnterRange_Empty_Enemy_Trigger_AttackPassEnemy,
    //     OtherEnterRange_Cross_Short_Enemy_BeTrigger_UnMove,
    //     SelfEnterRange_Empty_Enemy_BeTrigger_UnAttack,
    //     OtherLeaveRange_Cross_Short_Enemy_BeTrigger_SubDamage,
    //     OtherEnterRange_Cross_Short_Us_BeTrigger_AddDamage,
    //     OtherEnterRange_Cross_Short_Enemy_Hero_NewCard,
    //     OtherEnterRange_Cross_Short_Us_Hero_RemoveCard,
    //     ActiveAttack_OtherBack_Around_Enemy_Us_BeTrigger_CurHP,
    //     ActiveAttack_OtherBack_Around_Us_BeTrigger_CurHP,
    //     ActiveAttack_SelfBack_Around_Enemy_BeTrigger_CurHP,
    //     ActiveAttack_OtherClose_Cross_Long_Enemy_Us_BeTrigger_CurHP,
    //     ActiveAttack_Exchange_Around_Enemy_BeTrigger_CurHP,
    //     ActiveAttack_BackToSelf_Around_Us_BeTrigger_CurHP_AttackUs,
    //     ActiveAttack_SelfCrossCross_Long_Empty_UsEnemy_BeTrigger_CurHP,
    //     ActiveAttack_OtherBack_Around_Enemy_BeTrigger_HurtRoundStart,
    //     ActiveAttack_SelfClose_Cross_Extend_Enemy_BeTrigger_CurHP,
    //     ActiveAttack_BackToSelf_Around_Us_BeTrigger_CurHP_AttackEnemy,
    //     ActiveAttack_SelfBack_Around_Enemy_BeTrigger_CounterAttack,
    //     ActiveAttack_OtherClose_Cross_Long_Enemy_BeTrigger_UnAttack,
    //     ActiveAttack_SelfCross_Cross_Long_Empty_UsEnemy_BeTrigger_CurHP,
    //     ActiveAttack_BackToSelf_Around_Enemy_BeTrigger_CurHP,
    //     ActiveAttack_SelfClose_Cross_Extend_Us_Hero_Coin,
    //     ActiveAttack_BackToSelf_Cross_Long_Empty_UsEnemy_BeTrigger_CurHP,
    //     MoveGrid,
    //     MoveAllGrid,
    //     MoveUnEnemy,
    //     MoveEnemy,
    //     ExchangeGrid,
    //     Link_Receive_XLong_Us,
    //     Link_Send_CrossLong_Us,
    //     CardEnergyMax,
    //     HurtUsDamage,
    //     MoveCountDamage,
    //     UnitCountDamage,
    //     DeBuffCountDamage,
    //     BuffUsAddCurHP,
    //     UnitAddCurHP,
    //     RemoveCardAddCurHP,
    //     HurtEachMove_HurtRoundStart,
    //     HurtRoundStart_HurtEachMove,
    //     UnMoveAroundHeroUnit,
    //     UnAttackAroundHeroUnit,
    //     AttackPassEnemyAddDamage_AttackPassUsAddDamage,
    //     HurtSubDamageAddHeroCurHP,
    //     LessHalfHPEnemyHurtAddDamge,
    //     FullHPUsAddDamage,
    //     MoreHalfHPEnemySubDamge,
    //     RoundCounterAttackAddDamage,
    //     //CurHP1UsDodgeShield,
    //     RemoveDebuff,
    //     RoundDeBuffUnEffect,
    //     RoundCurseUnEffect,
    //     AutoRemove,
    //     AcquireCardSubCurHP,
    //     UnRemove,
    //     UnUseHurtAddDamge,
    //     UseMax2Card,
    //     UseCardCopy,
    //     EnergyBuff_UnitAddCurHP,
    //     EnergyBuff_AcquireCard,
    //     EnergyBuff_UnitAttack,
    //     EnergyBuff_UnitAction,
    //     EnergyBuff_UnitAddMaxHP,
    //     EnergyBuff_SubCurHP,
    //     EnergyBuff_SubCardEnergy,
    //     EnergyBuff_AcquireAllStandByCard,
    //     EnergyBuff_HandCardEnergyHalf,
    //     EnergyBuff_HurtRoundStart,
    //     EnergyBuff_HurtEachMove,
    //     EnergyBuff_UnMove,
    //     EnergyBuff_UnAttack,
    //     EnergyBuff_AttackPassUs,
    //     EnergyBuff_HurtAddDamage,
    //     EnergyBuff_SubDamage,
    //     EnergyBuff_BuffUnEffect,
    //     EnergyBuff_AttackPassEnemy,
    //     EnergyBuff_HurtSubDamage,
    //     EnergyBuff_AddDamage,
    //     EnergyBuff_CounterAttack,
    //     EnergyBuff_DeBuffUnEffect,
    //     EnergyBuff_SubCurHPAddSelfCurHP,
    //     EnergyBuff_UnEffectLink,
    //     EnergyBuff_UnBePass,
    //     EnergyBuff_CollideUnHurt,
    //     EnergyBuff_UnHurt,
    //     EnergyBuff_DoubleDamage,
    //     
    //
    //     
    //     Empty,
    // }

    public enum EBuffID
    {
        Spec_MoveGrid,
        Spec_ExchangeGrid,
        Spec_SubEnergy,
        Spec_AddMaxHP,
        Spec_AddBaseDmg,
        Spec_FirstRound,
        Spec_UnPass,
        Spec_SameUnitSubEnergy,
        Spec_UnDead,
        Spec_NextCardSubEnergy,
        Spec_DeadClearBuff,
        Spec_MoveDirect,
        Spec_Obstacle,
        Spec_DoubleDmg,
        Spec_DoubleHP,
        Spec_UseSubEnergy,
        Use_Empty_Around_Range2Us_Atrb_HP,
        Use_Empty_Around_Effect2Enemy_Atrb_HP,
        Use_Empty_Empty_Hero_Card_ToStandBy,
        Use_Empty_Empty_Hero_Card_CopyToPass,
        Kill_Empty_Empty_Hero_Card_NewCard,
        Kill_Empty_Empty_Hero_Card_ToHand,
        Kill_Empty_Empty_Hero_Card_Consume,
        Kill_Empty_Empty_Hero_Card_ConsumeToStandBy,
        Kill_Enemy_Empty_Action_State_UnHurt,
        Kill_Enemy_Empty_Action_State_DoubleDmg,
        Dead_Empty_Empty_Hero_Card_NewCard,
        Dead_Empty_Empty_Hero_Card_ToHand,
        Dead_Empty_Empty_Hero_Card_Consume,
        Dead_Empty_Empty_Hero_Card_ConsumeToStandBy,
        Dead_UsEnemy_Around_Range2Enemy_State_BuffUnEffect,
        Dead_UsEnemy_Around_Range2Us_State_DeBuffUnEffect,
        Spec_UnHurtSubHPDmg,
        Spec_SubHPAddBaseDmg,
        Spec_AtkClearBuff,
        Spec_HurtClearDeBuff,
        Round_Empty_Self_Action_Atrb_HP,
        Round_Empty_Self_Action_Atrb_MaxHP,
        Battle_Empty_Self_Action_State_Fly,
        Battle_Empty_Self_Action_State_UnBePass,
        Battle_Empty_Self_Action_State_CollideUnHurt,
        Battle_Empty_Self_Action_State_UnEffectLink,
        Kill_Enemy_Self_Action_Atrb_HP,
        Kill_Enemy_Self_Action_Atrb_BaseDmg,
        Kill_Enemy_Self_Action_Atrb_MaxHP,
        Kill_Enemy_Self_Action_State_UnHurt,
        Kill_Us_Self_Action_State_DoubleDmg,
        Dead_UsEnemy_Cross2Short_Range2Us_State_UnHurt,
        Dead_UsEnemy_Cross2Short_Range2Enemy_State_UnAtk,
        Hurt_UsEmpty_Self_Action_State_UnHurt,
        Dead_UsEmpty_All_LessHP2Us_Atrb_HP,
        AddDeBuff_UsEnemy_Self_Action_Atrb_HP,
        Dead_Us_Self_All_Atrb_HP,
        Hurt_UsEnemy_Self_Effect_Atrb_BaseDmg,
        Atk_UsEnemy_Self_Action_Atrb_BaseDmg,
        Round_Link_Send_Cross2Extend_Us,
        Round_Link_Receive__Cross2Extend_Us,
        Move_Enemy_Around_Effect_State_HurtRoundStart,
        Move_Us_Around_Effect_State_HurtEachMove,
        Move_Enemy_Around_Effect_Atrb_HP,Move_Us_Around_Effect_Atrb_HP,
        Hurt_UsEnemy_Self_Action_State_BuffUnEffect,
        Hurt_Enemy_Self_Effect_State_HurtEachMove,
        Atk_UsEnemy_Self_Action_State_AddDamage,
        Atk_Enemy_Self_Effect_State_SubDamage,
        Atk_UsEnemy_Self_Action_State_AtkAddSelfHP,
        SelfEnter_Enemy_Empty_Effect_State_HurtAddDmg,
        EnterSelf_UsEnemy_Empty_Action_State_HurtSubDmg,
        OtherExit_Enemy_Empty_Action_State_BuffUnEffect,
        SelfExit_Enemy_Empty_Action_State_AtkPassUs,
        BePass_Us_Self_Effect_State_CounterAtk,
        BePass_Us_Self_Action_Atrb_HP,
        BePass_Enemy_Self_Effect_Atrb_HP,
        UnAtk_Us_Around_Range_State_DeBuffUnEffect,
        UnAtk_Us_Around_Range_Atrb_HP,
        Spec_AddHPUnEffect,
        Spec_RoundRandomUsClearDeBuff,
        Spec_HurtUnUseCard,
        Spec_SubAcquireCardCount,
        Spec_UseMaxCount,
        Spec_TacticCardUnSelect,
        Spec_LinkUnEffect,
        Spec_UnPlaceEnemyUnit,
        Spec_SingleRoundLinkUnEffect,
        Dead_Us_Empty_All2Us_Artb_HP,
        Akt_Enemy_Empty_Effect_State_UnRecover,
        RoundStart_Us_Empty_All2Us_Atrb_HP,
        Hurt_UsEnemy_Self_Effect_Atrb_HP,
        Hurt_Enemy_Empty_Hero_Card_NewCard,
        Kill_Enemy_Empty_Hero_Card_ToComsume,
        Hurt_Enemy_Empty_Hero_Card_StandByToPass,
        BattleStart_Self_Us_Action_Round_UnBeMove,
        BattleStart_Empty_Empty_HeroToEnemy_State_UnMove,
        
        Spec_MoveUs,
        Spec_AttackUs,
        Spec_MoveAllGrid,
        Spec_ChangeDirect,
        Spec_ActionEnemy,
        
        Spec_SubCardEnergy,
        Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg,
        Spec_HurtSubDmgAddHP,
        Spec_CounterAtkAddDmg,
        Spec_RemoveDeBuff,
        Spec_CurseUnEffect,
        Spec_ConsumeToStandBy,
        
        SelectUnit_Us_Atrb_HP,
        Use_Us_Hero_Card_AcquireCard,
        SelectUnit_Us_State_UnitMove,
        SelectUnit_Us_State_UnitAttack,
        SelectUnit_Us_State_UnitAction,
        SelectUnit_Us_State_UnitAddMaxHP,
        SelectUnit_Enemy_Atrb_HP,
        SelectCard_Us_State_SubCardEnergy,
        SelectUnit_Us_State_HurtRoundStart,
        SelectUnit_Us_State_HurtEachMove,
        SelectUnit_Us_State_UnMove,
        SelectUnit_Us_State_AtkPassUs,
        SelectUnit_Us_State_HurtAddDmg,
        SelectUnit_Us_State_SubDmg,
        SelectUnit_Us_State_BuffUnEffect,
        SelectUnit_Us_State_AtkPassEnemy,
        SelectUnit_Us_State_HurtSubDmg,
        SelectUnit_Us_State_AddDmg,
        SelectUnit_Us_State_CounterAtk,
        SelectUnit_Us_State_DeBuffUnEffect,
        SelectUnit_Us_State_SubHPAddSelfHP,
        SelectUnit_Us_State_UnHurt,
        SelectUnit_Us_State_DoubleDmg,
        SelectUnit_Us_State_UnAtk,
        SelectUnit_Us_State_Fly,
        SelectUnit_Us_State_UnEffectLink,
        SelectUnit_Us_State_UnBePass,
        SelectUnit_Us_State_CollideUnHurt,
        Use_Us_Hero_Card_HandCardEnergyHalf,
        Use_Us_Hero_Card_ConsumeCard,


        
        Empty,
        None,

    }
    
    // public enum EBuffID
    // {
    //     BePass_Enemy_EffectUnit_CurHP,
    //     Pass_Enemy_EffectUnit_CurHP,
    //     Pass_Enemy_All_CurHP,
    //     BePass_Us_EffectUnit_CurHP,
    //     Pass_Us_EffectUnit_CurHP,
    //     Pass_Enemy_ActionUnit_CurHP,
    //     Pass_Enemy_Vertical_CurHP,
    //     Pass_Enemy_EffectUnit_HurtRoundStart,
    //     BePass_Enemy_EffectUnit_HurtEachMove,
    //     Pass_Us_ActionUnit_AttackPassEnemy,
    //     BePass_Us_ActionUnit_AttackPassEnemy,
    //     Pass_Enemy_ActionUnit_CounterAttack,
    //     BePass_Enemy_ActionUnit_CounterAttack,
    //     Pass_Enemy_EffectUnit_AttackPassUs,
    //     Pass_Us_EffectUnit_AttackPassEnemy,
    //     Pass_Us_EffectUnit_HurtSubDamage,
    //     Pass_Us_ActionUnit_HurtSubDamage,
    //     Pass_Enemy_Hero_AcquireCard,
    //     BePass_UsEnemy_Hero_ConsumeCard,
    //     BePass_Enemy_Hero_AddCard,
    //     AutoAttack_UnitMaxDirect_Enemy_EffectUnit_CurHP,
    //     AutoAttack_UnitMaxDirect_Us_EffectUnit_CurHP,
    //     AutoAttack_Cross_Extend_UsEnemy_EffectUnit_CurHP,
    //     AutoAttack_Direct8_Enemy_DeBuffMax_CurHP,
    //     AutoAttack_Direct8_Enemy_MoveMax_CurHP,
    //     AutoAttack_Around_Enemy_EffectUnit_HurtEachMove,
    //     AutoAttack_Cross_Extend_Us_EffectUnit_AttackPassEnemy,
    //     AutoAttack_Cross_Extend_Enemy_EffectUnit_AttackPassUs,
    //     AutoAttack_Around_Enemy_EffectUnit_AttackPassEnemy,
    //     AutoAttack_Around_Enemy_EffectUnit_HurtAddDamage,
    //     AutoAttack_UnitMaxDirect_Us_EffectUnit_AddDamage,
    //     AutoAttack_UnitMaxDirect_Enemy_EffectUnit_SubDamage,
    //     AutoAttack_Around_Us_EffectUnit_HurtSubDamage,
    //     AutoAttack_Cross_Short_Us_Hero_AcquireCard,
    //     AutoAttack_Cross_Short_EnemyUs_Hero_ConsumeCard,
    //     AutoAttack_Cross_Short_Enemy_Hero_AddCard,
    //     Move_Around_Enemy_EffectUnit_CurHP,
    //     Move_Around_Us_EffectUnit_CurHP,
    //     Move_Cross_Short_Enemy_EffectUnit_UnMove,
    //     Move_Cross_Short_Us_Hero_AcquireCard,
    //     Attack_Around_Us_EffectUnit_AttackAddSelfCurHP,
    //     Attack_Cross_Short_Us_EffectUnit_AddDamage,
    //     Attack_Cross_Short_Enemy_EffectUnit_SubDamage,
    //     Hurt_Around_Us_EffectUnit_CounterAttack,
    //     Attack_Cross_Short_Enemy_EffectUnit_HurtAddDamage,
    //     Attack_Cross_Short_Us_EffectUnit_HurtSubDamage,
    //     BuffEffect_Cross_Short_Enemy_EffectUnit_BuffUnEffect,
    //     DeBuffEffect_Cross_Short_Us_EffectUnit_DeBuffUnEffect,
    //     OtherEnterRange_Around_UsEnemy_EffectUnit_CurHP,
    //     SelfLeaveRange_Around_Enemy_EffectUnit_CurHP,
    //     SelfLeaveRange_Around_Us_EffectUnit_CurHP,
    //     SelfEnterRange_Around_UsEnemy_EffectUnit_CurHP,
    //     OtherEnterRange_Around_Enemy_EffectUnit_HurtRoundStart,
    //     OtherLeaveRange_Around_Enemy_EffectUnit_HurtRoundStart,
    //     OtherEnterRange_Around_Enemy_EffectUnit_HurtEachMove,
    //     SelfLeaveRange_Around_Enemy_EffectUnit_HurtEachMove,
    //     SelfEnterRange_Around_Enemy_ActionUnit_AttackPassEnemy,
    //     OtherEnterRange_Around_Enemy_EffectUnit_UnMove,
    //     SelfEnterRange_Cross_Short_Enemy_EffectUnit_BuffUnEffect,
    //     OtherLeaveRange_Around_Enemy_EffectUnit_SubDamage,
    //     OtherEnterRange_Around_Us_EffectUnit_AddDamage,
    //     OtherEnterRange_Around_UsEnemy_EffectUnit_AttackAddSelfCurHP,
    //     OtherEnterRange_Around_Enemy_Hero_AddCard,
    //     OtherEnterRange_Around_Us_Hero_ConsumeCard,
    //     SelectUnit_OtherBack_Around_Enemy_EffectUnit_CurHP,
    //     SelectUnit_OtherBack_Around_Us_EffectUnit_CurHP,
    //     SelectUnit_SelfBack_Around_Enemy_EffectUnit_CurHP,
    //     SelectUnit_OtherClose_Cross_Extend_Enemy_EffectUnit_CurHP,
    //     SelectUnit_Self_Us_ActionUnit_CurHP,
    //     SelectUnit_SelfClose_Cross_Extend_Us_EffectUnit_CurHP,
    //     SelectUnit_Exchange_Around_UsEnemy_EffectUnit_CurHP,
    //     SelectUnit_BackToSelf_Around_Us_EffectUnit_CurHP,
    //     SelectGrid_SelfCross_Cross_Long_Empty_UsEnemy_EffectUnit_CurHP,
    //     SelectUnit_OtherBack_Around_Enemy_EffectUnit_HurtRoundStart,
    //     SelectUnit_SelfBack_Around_Enemy_EffectUnit_UnMove,
    //     SelectUnit_OtherClose_Cross_Extend_Enemy_EffectUnit_HurtSubDamage,
    //     SelectUnit_Us_ActionUnit_DeBuffUnEffect,
    //     SelectUnit_SelfClose_Cross_Extend_Us_EffectUnit_DeBuffUnEffect,
    //     SelectUnit_Exchange_Around_Us_ActionUnit_GetBuff,
    //     SelectUnit_Exchange_Around_Enemy_EffectUnit_SetDeBuff,
    //     SelectUnit_BackToSelf_Around_Enemy_EffectUnit_CurHP,
    //     SelectUnit_SelfCross_Cross_Long_Empty_UsEnemy_EffectUnit_UnMove,
    //     MoveGrid,
    //     MoveAllGrid,
    //     UnEnemyAction,
    //     MoveEnemy,
    //     AddUnitCurHP,
    //     ExchangeGrid,
    //     SelectUnit_Link_Receive_CrossExtend_Us,
    //     SelectUnit_Link_Send_CrossExtend_Us,
    //     CardEnergyMax,
    //     HurtUsDamage,
    //     MoveCountDamage,
    //     UnitCountDamage,
    //     DeBuffCountDamage,
    //     BuffUsAddCurHP,
    //     RemoveCardAddCurHP,
    //     HurtEachMove_HurtRoundStart,
    //     HurtRoundStart_HurtEachMove,
    //     UnMoveAroundHeroUnit,
    //     UnAttackAroundHeroUnit,
    //     AttackPassEnemyAddDamage_AttackPassUsAddDamage,
    //     HurtSubDamageDoubleEffect,
    //     LessHalfHPEnemyHurtAddDamge,
    //     FullHPUsAddDamage,
    //     MoreHalfHPEnemySubDamge,
    //     RoundCounterAttackAddDamage,
    //     RemoveDebuff,
    //     RoundDeBuffUnEffect,
    //     RoundCurseUnEffect,
    //     SelectCard_ConsumeToStandBy,
    //     SelectUnit_All_Us_Round_Fly,
    //     SelectUnit_All_Us_Round_UnBePass,
    //     SelectUnit_All_Us_Round_CollideUnHurt,
    //     SelectUnit_All_Us_Round_UnEffectLink,
    //     Use_All_Enemy_UnMove_CurHP,
    //     Use_All_Enemy_Move_CurHP,
    //     ActionEnd_Around_UsEnemy_EffectUnit_CurHP,
    //     ActionEnd_Cross_Extend_UsEnemy_EffectUnit_CurHP,
    //     ActionEnd_Direct_UsEnemy_EffectUnit_CurHP,
    //     ActionEnd_Around_Enemy_EffectUnit_CurHP_Move,
    //     Pass_UsEnemy_EffectUnit_CurHP,
    //     ActionEnd_OtherBack_Around_Enemy_EffectUnit_CurHP,
    //     ActionEnd_SelfBack_Around_Enemy_EffectUnit_CurHP,
    //     ActionEnd_OtherClose_Cross_Extend_Enemy_EffectUnit_CurHP,
    //     ActionEnd_SelfClose_Cross_Extend_Enemy_EffectUnit_CurHP,
    //     ActionEnd_Around_UsEnemy_EffectUnit_CurHP_Unit,
    //     ActionEnd_Around_Us_EffectUnit_CurHP,
    //     Hurt_Around_UsEnemy_EffectUnit_CurHP,
    //     Move_Around_UsEnemy_EffectUnit_CurHP,
    //     OtherEnterRange_Cross_Long_UsEnemy_EffectUnit_CurHP,
    //     OtherLeaveRange_Cross_Long_UsEnemy_EffectUnit_CurHP,
    //     BePass_UsEnemy_EffectUnit_CurHP,
    //     Hurt_UsEnemy_ActionUnit_RandomPositiveUnitState,
    //     Hurt_Enemy_EffectUnit_RandomNegativeUnitState,
    //     ActionEnd_UsEnemy_ActionUnit_RandomPositiveUnitState,
    //     ActionEnd_Around_Enemy_InRange_RandomNegativeUnitState,
    //     Action_Around_Us_InRange_RandomPositiveUnitState,
    //     Action_Around_Us_InRange_CurHP,
    //     SubEnergy,
    //     AddMaxHP,
    //     AddBaseDamage,
    //     FirstRound,
    //     RoundStart_CurHP,
    //     RoundStart_MaxHP,
    //     Link_Cross_Extend_Send,
    //     Link_Cross_Extend_Receive,
    //     InHand_UnPass,
    //     EachSubCurHPAddDamage,
    //     SameUnitSubEnergy,
    //     MultiplyMaxHP,
    //     UnDead,
    //     BattleStart_Fly,
    //     BattleStart_Obstacle,
    //     BattleStart_CollideUnHurt,
    //     BattleStart_UnEffectLink,
    //     Use_AddCurHP,
    //     Use_SubCurHP,
    //     Use_ToStandBy,
    //     Use_CopyToPass,
    //     Use_SubEnergy,
    //     Use_AddDamage,
    //     Use_HurtSubDamage,
    //     Pass_Enemy_EffectUnit_UnMove,
    //     BePass_Enemy_EffectUnit_HurtAddDamage,
    //     BePass_Enemy_EffectUnit_SubDamage,
    //     BePass_UsEnemy_ActionUnit_AddDamage,
    //     Pass_Us_ActionUnit_AttackAddSelfCurHP,
    //     Kill_CurHP,
    //     Kill_BaseDamage,
    //     Kill_MaxHP,
    //     Kill_UnHurt,
    //     Kill_DoubleDamage,
    //     Kill_SelectCard,
    //     Kill_ToHand,
    //     Kill_ConsumeCard,
    //     Kill_ConsumeToStandBy,
    //     Dead_Direct8_Extend_Us_EffectUnit_UnHurt,
    //     Dead_Direct8_Extend_Enemy_EffectUnit_UnAttack,
    //     Dead_SelectCard,
    //     Dead_ToHand,
    //     Dead_ConsumeCard,
    //     Dead_ConsumeToStandBy,
    //     Attack_ClearEffectUnitBuff,
    //     Attack_Enemy_EffectUnit_HurtRoundStart,
    //     Attack_Enemy_EffectUnit_HurtEachMovert,
    //     Attack_Enemy_EffectUnit_AttackPassUsrt,
    //     Attack_Enemy_ActionUnit_AttackPassEnemy,
    //     Hurt_ClearActionUnitDeBuff,
    //     Hurt_UsEnemy_ActionUnit_AddDamage,
    //     Hurt_UsEnemy_ActionUnit_HurtSubDamage,
    //     Hurt_Enemy_EffectUnit_SubDamage,
    //     Hurt_Enemy_EffectUnit_HurtAddDamage,
    //     UseMoreActionTimes,
    //     RoundStart_Self_Us_ActionUnit_Round_UnHurt,
    //     RoundStart_Self_Us_ActionUnit_Round_HurtSubDamage,
    //     RoundStart_Self_Us_EffectUnit_CurHP,
    //     Dead_LessCurHP_Us_EffectUnit_CurHP_Full,
    //     Dead_Self_Us_All_CurHP_Damage,
    //     Dead_Self_Us_ActionUnit_Avoid,
    //     ChangeHP_All_Enemy_EffectUnit_UnRecover,
    //     AddDebuff_Self_Us_ActionUnit_CurHP,
    //     Dead_All_Enemy_All_CurHP,
    //     Hurt_Self_Enemy_EffectUnit_UnAttack,
    //     SingleRound_Self_Us_ActionUnit_DoubleDamage,
    //     RoundStart_Self_Us_ActionUnit_CollideUnHurt,
    //     Dead_All_Us_ActionUnit_BaseDamage,
    //     Hurt_Self_Us_ActionUnit_AddDamage,
    //     SingleRound_Self_Us_ActionUnit_DeBuffUnEffect,
    //     Kill_Around_Us_InRange_BaseDamage,
    //     Kill_Around_Us_InRange_CurHP,
    //     RoundStart_Self_Us_ActionUnit_CounterAttack,
    //     RoundStart_Self_Us_ActionUnit_AttackPassEnemy,
    //     RoundStart_All_Us_RandomUnit_ClearDeBuff,
    //     Attack_Self_Us_ActionUnit_HurtSubDamage,
    //     Kill_Around_Us_InRange_AddDamage,
    //     Kill_Around_Us_InRange_HurtSubDamage,
    //     Hurt_Self_Enemy_Hero_AddDebuffCard,
    //     Kill_Self_Us_Hero_EffectToComsume,
    //     Kill_Self_Us_Hero_StandByToComsume,
    //     Hurt_Self_Us_Hero_BanHandCard,
    //     AcquireCard_SubAcquireCardCount,
    //     Use_MaxCount,
    //     Use_UnitAttack,
    //     TacticCardUnSelect,
    //     LinkUnEffect,
    //     UnPlaceEnemyUnit,
    //     ReceiveEnemyLink,
    //     BattleStart_Self_Us_ActionUnit_Round_UnBeMove,
    //     SingleRoundLinkUnEffect,
    //     SelectUnit_Us_CurHP,
    //     Use_AcquireCard,
    //     SelectUnit_Us_UnitMove,
    //     SelectUnit_Us_UnitAttack,
    //     SelectUnit_Us_UnitAction,
    //     SelectUnit_Us_UnitAddMaxHP,
    //     SelectUnit_Enemy_CurHP,
    //     SelectCard_Us_SubCardEnergy,
    //     SelectUnit_Enemy_HurtRoundStart,
    //     SelectUnit_Enemy_HurtEachMove,
    //     SelectUnit_Enemy_UnMove,
    //     SelectUnit_Enemy_AttackPassUs,
    //     SelectUnit_Enemy_HurtAddDamage,
    //     SelectUnit_Enemy_SubDamage,
    //     SelectUnit_Enemy_BuffUnEffect,
    //     SelectUnit_Us_AttackPassEnemy,
    //     SelectUnit_Us_HurtSubDamage,
    //     SelectUnit_Us_AddDamage,
    //     SelectUnit_Us_CounterAttack,
    //     SelectUnit_Us_DeBuffUnEffect,
    //     SelectUnit_Us_SubCurHPAddSelfCurHP,
    //     SelectUnit_Us_UnHurt,
    //     SelectUnit_Us_DoubleDamage,
    //     SelectUnit_Us_UnAttack,
    //     SelectUnit_Us_Fly,
    //     SelectUnit_Us_UnEffectLink,
    //     SelectUnit_Us_UnBePass,
    //     SelectUnit_Us_CollideUnHurt,
    //     Use_CardSubEnergy,
    //     Use_ConsumeCard,
    //     RoundStart_Cross_Extend_Us_EffectUnit_Pos_HurtSubDamage,
    //     RoundStart_Cross_Extend_enemy_EffectUnit_Pos_SubDamage,
    //     Hurt_Around_Us_TriggerUnit_CurHP,
    //     Link_Receive_Cross_Extend_Us,
    //     RoundStart_Cross_Short_Us_InRange_DeBuffUnEffect,
    //     RoundStart_Cross_Short_Enemy_InRange_BuffUnEffect,
    //     RoundStart_Around_Us_InRange_BuffAddMore,
    //     RoundStart_Around_Enemy_InRange_DeBuffAddMore,
    //     AutoAttack_Cross_Extend_Us_InRange_CurHP,
    //     AutoAttack_Cross_Extend_Enemy_InRange_CurHP,
    //     AutoAttack_Around_UsEnemy_InRange_CurHP,
    //     Hurt_Self_Us_Hero_AcquireCard,
    //     Hurt_Self_Us_Hero_ConsumeCard,
    //     Attack_Around_UsEnemy_ActionUnit_Pos_AttackPassEnemy,
    //     Attack_Around_UsEnemy_ActionUnit_Pos_HurtSubDamage,
    //     Attack_Around_UsEnemy_ActionUnit_Pos_AddDamage,
    //     Attack_Around_UsEnemy_ActionUnit_Pos_CounterAttack,
    //     Attack_Around_UsEnemy_ActionUnit_Pos_AttackAddSelfCurHP,
    //     Attack_Cross_Short_UsEnemy_ActionUnit_Pos_DeBuffUnEffect,
    //     Attack_Around_UsEnemy_ActionUnit_Pos_UnHurt,
    //     Attack_Around_UsEnemy_ActionUnit_DoubleDamage,
    //     Attack_Around_UsEnemy_EffectUnit_Pos_HurtRoundStart,
    //     Attack_Around_UsEnemy_EffectUnit_Pos_HurtEachMove,
    //     Attack_Around_UsEnemy_EffectUnit_Pos_UnMove,
    //     Attack_Around_UsEnemy_EffectUnit_Pos_AttackPassUs,
    //     Attack_Around_UsEnemy_EffectUnit_Pos_HurtAddDamage,
    //     Attack_Around_UsEnemy_EffectUnit_Pos_SubDamage,
    //     Attack_Cross_Short_UsEnemy_EffectUnit_Pos_BuffUnEffect,
    //     Attack_Around_UsEnemy_EffectUnit_Pos_UnAttack,
    //     Attack_Self_UsEnemy_ActionUnit_Pos_AttackPassEnemy,
    //     Attack_Self_UsEnemy_ActionUnit_Pos_HurtSubDamage,
    //     Attack_Self_UsEnemy_ActionUnit_Pos_AddDamage,
    //     Attack_Self_UsEnemy_ActionUnit_Pos_CounterAttack,
    //     Attack_Self_UsEnemy_ActionUnit_Pos_AttackAddSelfCurHP,
    //     Attack_Self_UsEnemy_ActionUnit_Pos_DeBuffUnEffect,
    //     Attack_Self_UsEnemy_ActionUnit_Pos_UnHurt,
    //     Attack_Self_UsEnemy_ActionUnit_Pos_DoubleDamage,
    //     Attack_Self_UsEnemy_EffectUnit_Pos_HurtRoundStart,
    //     Attack_Self_UsEnemy_EffectUnit_Pos_HurtEachMove,
    //     Attack_Self_UsEnemy_EffectUnit_Pos_UnMove,
    //     Attack_Self_UsEnemy_EffectUnit_Pos_AttackPassUs,
    //     Attack_Self_UsEnemy_EffectUnit_Pos_HurtAddDamage,
    //     Attack_Self_UsEnemy_EffectUnit_Pos_SubDamage,
    //     Attack_Self_UsEnemy_EffectUnit_Pos_BuffUnEffect,
    //     Attack_Self_UsEnemy_EffectUnit_Pos_UnAttack,
    //     Hurt_Around_UsEnemy_ActionUnit_Pos_HurtPassEnemy,
    //     Hurt_Around_UsEnemy_ActionUnit_Pos_HurtSubDamage,
    //     Hurt_Around_UsEnemy_ActionUnit_Pos_AddDamage,
    //     Hurt_Around_UsEnemy_ActionUnit_Pos_CounterAttack,
    //     Hurt_Around_UsEnemy_ActionUnit_Pos_AttackAddSelfCurHP,
    //     Hurt_Cross_Short_UsEnemy_ActionUnit_Pos_DeBuffUnEffect,
    //     Hurt_Around_UsEnemy_ActionUnit_Pos_UnHurt,
    //     Hurt_Around_UsEnemy_ActionUnit_Pos_DoubleDamage,
    //     Hurt_Around_UsEnemy_EffectUnit_Pos_HurtRoundStart,
    //     Hurt_Around_UsEnemy_EffectUnit_Pos_HurtEachMove,
    //     Hurt_Around_UsEnemy_EffectUnit_Pos_UnMove,
    //     Hurt_Around_UsEnemy_EffectUnit_Pos_AttackPassUs,
    //     Hurt_Around_UsEnemy_EffectUnit_Pos_HurtAddDamage,
    //     Hurt_Around_UsEnemy_EffectUnit_Pos_SubDamage,
    //     Hurt_Cross_Short_UsEnemy_EffectUnit_Pos_BuffUnEffect,
    //     Hurt_Around_UsEnemy_EffectUnit_Pos_UnAttack,
    //     Hurt_Self_UsEnemy_ActionUnit_Pos_AttackPassEnemy,
    //     Hurt_Self_UsEnemy_ActionUnit_Pos_HurtSubDamage,
    //     Hurt_Self_UsEnemy_ActionUnit_Pos_AddDamage,
    //     Hurt_Self_UsEnemy_ActionUnit_Pos_CounterAttack,
    //     Hurt_Self_UsEnemy_ActionUnit_Pos_AttackAddSelfCurHP,
    //     Hurt_Self_UsEnemy_ActionUnit_Pos_DeBuffUnEffect,
    //     Hurt_Self_UsEnemy_ActionUnit_Pos_UnHurt,
    //     Hurt_Self_UsEnemy_ActionUnit_Pos_DoubleDamage,
    //     Hurt_Self_UsEnemy_EffectUnit_Pos_HurtRoundStart,
    //     Hurt_Self_UsEnemy_EffectUnit_Pos_HurtEachMove,
    //     Hurt_Self_UsEnemy_EffectUnit_Pos_UnMove,
    //     Hurt_Self_UsEnemy_EffectUnit_Pos_AttackPassUs,
    //     Hurt_Self_UsEnemy_EffectUnit_Pos_HurtAddDamage,
    //     Hurt_Self_UsEnemy_EffectUnit_Pos_SubDamage,
    //     Hurt_Self_UsEnemy_EffectUnit_Pos_BuffUnEffect,
    //     Hurt_Self_UsEnemy_EffectUnit_Pos_UnAttack,
    //     BePass_Self_UsEnemy_ActionUnit_Pos_AttackPassEnemy,
    //     BePass_Self_UsEnemy_ActionUnit_Pos_HurtSubDamage,
    //     BePass_Self_UsEnemy_ActionUnit_Pos_AddDamage,
    //     BePass_Self_UsEnemy_ActionUnit_Pos_CounterAttack,
    //     BePass_Self_UsEnemy_ActionUnit_Pos_AttackAddSelfCurHP,
    //     BePass_Self_UsEnemy_ActionUnit_Pos_DeBuffUnEffect,
    //     BePass_Self_UsEnemy_ActionUnit_Pos_UnHurt,
    //     BePass_Self_UsEnemy_ActionUnit_Pos_DoubleDamage,
    //     BePass_Self_UsEnemy_EffectUnit_Pos_HurtRoundStart,
    //     BePass_Self_UsEnemy_EffectUnit_Pos_HurtEachMove,
    //     BePass_Self_UsEnemy_EffectUnit_Pos_UnMove,
    //     BePass_Self_UsEnemy_EffectUnit_Pos_AttackPassUs,
    //     BePass_Self_UsEnemy_EffectUnit_Pos_HurtAddDamage,
    //     BePass_Self_UsEnemy_EffectUnit_Pos_SubDamage,
    //     BePass_Self_UsEnemy_EffectUnit_Pos_BuffUnEffect,
    //     BePass_Self_UsEnemy_EffectUnit_Pos_UnAttack,
    //     BePass_Self_Enemy_EffectUnit_CurHP,
    //     ActionEnd_Cross_Extend_Enemy_EffectUnit_CurHP,
    //     ActionEnd_UnitMaxDirect_Enemy_EffectUnit_CurHP,
    //     ActionEnd_Around_Enemy_EffectUnit_CurHP,
    //     ActionEnd_Cross_Short_Enemy_EffectUnit_CurHP,
    //     BePass_Self_UsEnemy_EffectUnit_AddCard,
    //     BePass_Self_UsEnemy_EffectUnit_AddBless,
    //     BePass_Self_UsEnemy_EffectUnit_ChangeDirect,
    //     Empty,
    //
    //
    //
    // }

    public enum ELinkID
    {
        Link_Receive_Around_Us,
        Link_Send_Around_Us,
        Link_Receive_XLong_Us,
        Link_Send_CrossLong_Us,
        Link_Send_Direct8Extend_UsEnemy,
        Link_Receive_Direct8Extend_UsEnemy,
        Link_Receive_Cross_Extend_Us,

        Empty,
    }

    public enum ECurseID
    {
        
        UnitDeadUnRecoverHeroHP,
        RandomUnitUnRecover,
        
        RandomUnitAttackRecoverHP,
        UnitDeadRecoverLessHPUnit,
        AddDebuffRecoverHP,
        AllUnitDodgeSubHeartDamage,
        RandomUnitUnHurt,
        SameUnitSameCurHP,
        HP1UnRecover,
        RandomUnitUnAttack,
        TacticCardUnDamage_OddRound,
        TacticCardUnDamage_EvenRound,
        AttackMostUnit,
        RandomUnitDoubleDamage,
        RandomUnitClearDebuff,
        PlayerBuffUnEffect_OddRound,
        PlayerBuffUnEffect_EvenRound,
        PlayerDeBuffAddValue_OddRound,
        PlayerDeBuffAddValue_EvenRound,
        MaxHandCardCount,
        RandomCardUnUse,
        OnGirdUnitAddEnergy,
        UnitCardAddEnengy_OddRound,
        UnitCardAddEnengy_EvenRound,
        TacticCardAddEnengy_OddRound,
        TacticCardAddEnengy_EvenRound,
        ConsumeCardUnAcquire,
        EachRoundBreakEmptyGrid,
        RandomUnitUnMove,
        LinkUnEffect_OddRound,
        LinkUnEffect_EvenRound,


        
        Round_DamageAddCurHP,
    }
    
    public enum EBuffType
    {
        Fune,
        EnemyFune,
        EnergyBuff,
        EnemyGlobal
    }


    public enum EBlessID
    {
        HeroRebirth,
        HeroDodgeSubHeartDamage,
        ConsumeCardAddCurHP,
        UnUseCardAddCurHP,
        EachRoundAddCurHPInBigBattle,
        AddCurHPByAttackDamage,
        HeroKillEnemyAddMaxHP,
        RoundEndAddCurHPByUnitCountInBattle,
        HeroKillEnemyAddCurHP,
        AddMaxHPReplaceAddCard,
        StoreFullCurHP,
        ShuffleCardAddCurHP,
        AddStateCardAddMaxHP,
        DodgeHeroFirstSubCurHP,
        RoundEndAddCurHPByCardCount,
        DefenseToHP,
        HPTo0FullAllUnitHP,
        RadomUnitAttackAddSelfCurHP,
        UnitDeadAddCurHPUnit,
        AddDebuffAddCurHP,
        AddDamageToAddCurHP,
        UnEffectFirstCollideDamage,
        UnCollideDead,
        FirstMoveAddCurHP,
        HeroHurtToLestCurHPUnit,
        UseSameUnitAddMaxHP,
        EachUseCardUnUseEnergy,
        UseCardSubOtherCardEnergy,
        EachRoundUseFightCardAttackAllEnemy,
        EachRoundUseTacticCardAttackAllEnemy,
        EachUseCardDoubleHPDelta,
        EachRoundDoubleDamage,
        ConsumeCardAttackEnemy,
        ConsumeCardAttackAllEnemy,
        AddDebuffAttackAroundUnit,
        FirstAttackDoubleDamageInBattle,
        FirstAttackMoreDamageInRound,
        UseSameUnitAddBaseDamage,
        EachDamgeDoubleDamage,
        UnHurtSubDamageAddDamage,
        HurtSubDamageSubDamage,
        EachRoundUseUnitCardAddDefense,
        HeroEachHurtUnDamage,
        UnHurtSubDamageSubDamage,
        EnemyDeadDebuffToOtherEnemy,
        AddEnemyMoreDebuff,
        EnemyGenerateAddDebuff,
        EachRoundAddAllEnemyDebuff,
        CollideAcquireDebuff,
        AddBuffToNoBuffUs,
        ShuffleCardAddBuff,
        AppearAddBuffEnemyUnitCount,
        AppearAddBuffUsUnitCount,
        UseCardNextRoundAcquireCard,
        PassCardAcquireCard,
        BattleStartAcquireCard,
        EachUseCardAcquireCard,
        NoHandCardAcquireCard,
        BattleStartPassCardAcquireCard,
        HeroHurtAcquireCard,
        RoundEndSelectCard,
        ConsumeCardAddRandomCard,
        EachRoundAcquireNewCard,
        BattleaStartAddRandomCard0Energy,
        UseCardRandomCard0Energy,
        StandByCardsOrderByAcquire,
        UnPassCards,
        CardAddFunePos,
        UnConsumeCard,
        UseUnUseCard,
        UseUnUseCardConsumCard,
        AddCardCountMore,
        ShuffleCardPassCard,
        AcquireCardCountMore,
        RestRmoveCard,
        StoreLevelUpCard,
        StoreItemInfinite,
        EenergyBuffAccumulate,
        SplitFune,
        EachRoundFightCardAddLinkSend,
        EachRoundFightCardAddLinkReceive,
        StoreSale,
        EnemyCoinMore,
        UnknowRoomMoreCoin,
        EliteEnemyMoreReward,
        NormalEnemyMoreReward,
        RestMoreReward,
        AddCardAddCoin,
        UnknowRoomUnBattle,
        EachUnknowRoomRewardRoom,
        Empty,
        
        EachRoundAcquireCard,
        
    }

    public enum EHPChangeType
    {
        Unit,
        CardConsume,
        Bless,
        Action,
    }
    
}