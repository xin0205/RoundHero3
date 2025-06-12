using System.Collections.Generic;
using System.Linq;

namespace RoundHero
{
    public partial class BattleBuffManager : Singleton<BattleBuffManager>
    {
    public List<TriggerData> BuffTrigger(EBuffTriggerType buffTriggerType, BuffData buffData, List<float> buffValues, int ownUnitIdx,
            int actionUnitIdx, int effectUnitIdx, List<TriggerData> triggerDatas, int actionUnitGridPosIdx = -1,
            int actionUnitPreGridPosIdx = -1)
        {
            //var drBuff = BattleBuffManager.Instance.GetBuffData(buffID);
            if (buffTriggerType != buffData.BuffTriggerType)
                return null;


            
            var isSubCurHP = false;
 
            
            var _triggerDatas = BattleBuffManager.Instance.InternalBuffTrigger(buffTriggerType, buffData, buffValues, ownUnitIdx, actionUnitIdx,
                effectUnitIdx, triggerDatas, actionUnitGridPosIdx, actionUnitPreGridPosIdx);

            foreach (var triggerData in _triggerDatas)
            {
                triggerData.BuffValue = new BuffValue()
                {
                    BuffData = buffData,
                    ValueList = new List<float>(buffValues),
                    UnitIdx = triggerData.EffectUnitIdx,
                    TargetGridPosIdx = triggerData.EffectUnitGridPosIdx,
                };
                if (GameUtility.IsSubCurHPTrigger(triggerData))
                {
                    isSubCurHP = true;
                }


                if (isSubCurHP)
                {
                    BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                    BattleUnitStateManager.Instance.CheckUnitState(actionUnitIdx, triggerDatas);
                }
            }

            return _triggerDatas;
        }
    
        public void CacheBuffData(BuffData buffData, EUnitCamp unitCamp, Data_BattleUnit effectUnit, List<float> value1s, float ratio)
        {

            
            
            
            //var buffData =  BattleBuffManager.Instance.GetBuffData(buffID);
            // switch (buffID)
            // {
            //     // case EBuffID.AcquireCard:
            //     //     BattleBuffManager.Instance.BuffTrigger(drBuff.BuffTriggerType, buffID, value1s, -1, -1,
            //     //         effectUnit.ID, FightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas[effectUnit.ID]);
            //     //     break;
            //     // case EBuffID.UnitAttack:
            //     //     break;
            //     // case EBuffID.UnitAction:
            //     //     break;
            //     // case EBuffID.UnitAddMaxHP:
            //     //     break;
            //     // case EBuffID.SubCurHP:
            //     //     break;
            //     // case EBuffID.SubCardEnergy:
            //     //     break;
            //     // case EBuffID.AcquireAllStandByCard:
            //     //     break;
            //     // case EBuffID.HandCardEnergyHalf:
            //     //     break;
            //     // case EBuffID.HurtRoundStart:
            //     // case EBuffID.HurtEachMove:
            //     // case EBuffID.UnMove:
            //     // case EBuffID.UnAttack:
            //     // case EBuffID.AttackPassUs:
            //     // case EBuffID.HurtAddDamage:
            //     // case EBuffID.SubDamage:
            //     // case EBuffID.BuffUnEffect:
            //     // case EBuffID.AttackPassEnemy:
            //     // case EBuffID.HurtSubDamage:
            //     // case EBuffID.AddDamage:
            //     // case EBuffID.CounterAttack:
            //     // case EBuffID.SubCurHPAddSelfCurHP:
            //     // case EBuffID.UnEffectLink:
            //     // case EBuffID.UnBePass:
            //     // case EBuffID.CollideUnHurt:
            //     // case EBuffID.UnHurt:
            //     // case EBuffID.DoubleDamage:
            //     //     AddStates(buffID, effectUnit, value1s);
            //     //     break;
            //     //
            //     // case EBuffID.MoveUnEnemy:
            //     //     break;
            //     //
            //     // case EBuffID.HurtUsDamage:
            //     //     HurtUsDamage(unitCamp, effectUnit,value1s, ratio);
            //     //     break;
            //     // case EBuffID.MoveCountDamage:
            //     //     MoveCountDamage(effectUnit, value1s, ratio);
            //     //     break;
            //     // case EBuffID.UnitCountDamage:
            //     //     UnitCountDamage(unitCamp, effectUnit, value1s, ratio);
            //     //     break;
            //     // case EBuffID.DeBuffCountDamage:
            //     //     DeBuffCountDamage(unitCamp, value1s, ratio);
            //     //     break;
            //     // case EBuffID.BuffUsAddCurHP:
            //     //     BuffUsAddCurHP(unitCamp, value1s, ratio);
            //     //     break;
            //     // case EBuffID.CardEnergyMax:
            //     //     AddRoundStates(buffID, unitCamp);
            //     //     break;
            //     // case EBuffID.FullHPUsAddDamage:
            //     //     FullHPUsAddDamage(unitCamp, value1s);
            //     //     break;
            //     // // case ECardID.CurHP1UsDodgeShield:
            //     // //     //TacticCard_CurHP1UsDodge(cardID, unitCamp, effectUnit);
            //     // //     break;
            //     // case EBuffID.LessHalfHPEnemyHurtAddDamge:
            //     //     LessHalfHPEnemyHurtAddDamge(unitCamp, value1s);
            //     //     break;
            //     // case EBuffID.MoreHalfHPEnemySubDamge:
            //     //     MoreHalfHPEnemySubDamge(unitCamp, value1s);
            //     //     break;
            //     // case EBuffID.Link_Receive_XLong_Us:
            //     //     Link(buffID, effectUnit);
            //     //     break;
            //     // case EBuffID.Link_Send_CrossLong_Us:
            //     //     Link(buffID, effectUnit);
            //     //     break;
            //     // case EBuffID.UnitAddCurHP:
            //     //     UnitAddCurHP(effectUnit, value1s);
            //     //     break;
            //     // case EBuffID.RemoveDebuff:
            //     //     RemoveDebuff(unitCamp, effectUnit);
            //     //     break;
            //     // case EBuffID.RemoveCardAddCurHP:
            //     //     RemoveCardAddCurHP(effectUnit);
            //     //     break;
            //     // case EBuffID.HurtEachMove_HurtRoundStart:
            //     //     State1AddState2(unitCamp, EUnitState.HurtEachMove,
            //     //         EUnitState.HurtRoundStart, value1s);
            //     //     break;
            //     // case EBuffID.HurtRoundStart_HurtEachMove:
            //     //     State1AddState2(unitCamp, EUnitState.HurtRoundStart,
            //     //         EUnitState.HurtEachMove, value1s);
            //     //     break;
            //     // case EBuffID.UnMoveAroundHeroUnit:
            //     //     UnMoveHeroUnit(unitCamp);
            //     //     break;
            //     // case EBuffID.UnAttackAroundHeroUnit:
            //     //     UnMoveHeroUnit(unitCamp);
            //     //     break;
            //     // case EBuffID.AttackPassEnemyAddDamage_AttackPassUsAddDamage:
            //     //     AddRoundStates(buffID, unitCamp);
            //     //     break;
            //     // case EBuffID.HurtSubDamageAddHeroCurHP:
            //     //     AddRoundStates(buffID, unitCamp);
            //     //     break;
            //     // case EBuffID.RoundCounterAttackAddDamage:
            //     //     AddRoundStates(buffID, unitCamp);
            //     //     break;
            //     // case EBuffID.DeBuffUnEffect:
            //     //     AddRoundStates(buffID, unitCamp);
            //     //     break;
            //     // case EBuffID.RoundCurseUnEffect:
            //     //     AddRoundStates(buffID, unitCamp);
            //     //     break;
            //
            //     default:
            //         break;
            // }


            BattleFightManager.Instance.CalculateHeroHPDelta(BattleFightManager.Instance.RoundFightData.BuffData_Use);

        }


        public void HurtUsDamage(EUnitCamp camp, Data_BattleUnit effectUnit, List<float> value1s, float ratio)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;
          
            var unitCount =
                BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetUnitCount(BattleManager.Instance.CurUnitCamp,
                    new List<ERelativeCamp>() {ERelativeCamp.Us}, new List<EUnitRole>() {EUnitRole.Staff, EUnitRole.Hero});
            var value = unitCount * value1s[1] * ratio;

            var triggerData = new TriggerData()
            {
                TriggerDataType = ETriggerDataType.RoleAttribute,
                EffectUnitIdx = effectUnit.Idx,
                BattleUnitAttribute = EUnitAttribute.HP,
                Value = value,
            };
            useCardData.AddEmptyTriggerDataList(effectUnit.Idx);
            
            BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[effectUnit.Idx]);
            
            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if (kv.Value.UnitCamp == camp && kv.Value.CurHP > 0)
                {
                    var value2 =  value1s[0] * ratio;
                    triggerData = new TriggerData()
                    {
                        TriggerDataType = ETriggerDataType.RoleAttribute,
                        EffectUnitIdx = kv.Value.Idx,
                        BattleUnitAttribute = EUnitAttribute.HP,
                        Value = value2,

                    };

                    useCardData.AddEmptyTriggerDataList(kv.Value.Idx);
                    BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.Idx]);
                }
                
            }
        }
        
        private void UnitCountDamage(EUnitCamp unitCamp, Data_BattleUnit effectUnit, List<float> value1s, float ratio)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            var unitCount =
                BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetUnitCount(BattleManager.Instance.CurUnitCamp,
                    new List<ERelativeCamp>() {ERelativeCamp.Us, ERelativeCamp.Enemy}, new List<EUnitRole>() {EUnitRole.Staff, EUnitRole.Hero});

            var value = unitCount * value1s[0] * ratio;

            var triggerData = new TriggerData()
            {
                TriggerDataType = ETriggerDataType.RoleAttribute,
                EffectUnitIdx = effectUnit.Idx,
                BattleUnitAttribute = EUnitAttribute.HP,
                Value = value,
            };
            
            useCardData.AddEmptyTriggerDataList(effectUnit.Idx);
            BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[effectUnit.Idx]);

        }
        
        private void MoveCountDamage(Data_BattleUnit effectUnit, List<float> value1s, float ratio)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            var value = effectUnit.RoundGridMoveCount * value1s[0] * ratio;

            var triggerData = new TriggerData()
            {
                TriggerDataType = ETriggerDataType.RoleAttribute,
                EffectUnitIdx = effectUnit.Idx,
                BattleUnitAttribute = EUnitAttribute.HP,
                Value = value,
            };
            
            useCardData.AddEmptyTriggerDataList(effectUnit.Idx);
            BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[effectUnit.Idx]);

        }
        
        private void DeBuffCountDamage(EUnitCamp unitCamp, List<float> value1s, float ratio)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if(unitCamp == kv.Value.UnitCamp)
                    continue;
                
                var debuffCount = kv.Value.TargetBuffCount(EUnitStateEffectType.Negative);

                var value = debuffCount * value1s[0] * ratio;

                var triggerData = new TriggerData()
                {
                    TriggerDataType = ETriggerDataType.RoleAttribute,
                    EffectUnitIdx = kv.Value.Idx,
                    BattleUnitAttribute = EUnitAttribute.HP,
                    Value = value,
                };
                
                useCardData.AddEmptyTriggerDataList(kv.Value.Idx);
                BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.Idx]);
            }

        }
        
        private void BuffUsAddCurHP(EUnitCamp unitCamp, List<float> value1s, float ratio)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                
                if(kv.Value.UnitCamp != unitCamp)
                    continue;
                
                var buffCount = kv.Value.TargetBuffCount(EUnitStateEffectType.Positive);

                var value = buffCount * value1s[0] * ratio;
     
                var triggerData = new TriggerData()
                {
                    TriggerDataType = ETriggerDataType.RoleAttribute,
                    EffectUnitIdx = kv.Value.Idx,
                    BattleUnitAttribute = EUnitAttribute.HP,
                    Value = value,
                };
                
                useCardData.AddEmptyTriggerDataList(kv.Value.Idx);
                BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.Idx]);
            }

        }
        
        private void FullHPUsAddDamage(EUnitCamp unitCamp, List<float> value1s)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if(kv.Value.UnitCamp != unitCamp)
                    continue;
                
                if(kv.Value.CurHP < kv.Value.MaxHP)
                    continue;

                var triggerData = new TriggerData()
                {
                    TriggerDataType = ETriggerDataType.RoleState,
                    EffectUnitIdx = kv.Value.Idx,
                    UnitStateDetail = new UnitStateDetail()
                    {
                        UnitState = EUnitState.AddDmg,
                        Value = (int)value1s[0],
                    },
                    
                   
                };
                
                useCardData.AddEmptyTriggerDataList(kv.Value.Idx);
                BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.Idx]);
            }

        }
        
        // private void TacticCard_CurHP1UsDodge(int cardID, EUnitCamp unitCamp, Data_BattleUnit effectUnit)
        // {
        //     var useCardData = FightManager.Instance.RoundFightData.UseCardData;
        //     var drCard = CardManager.Instance.GetCardTable(cardID);
        //     
        //     foreach (var kv in FightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
        //     {
        //         if(kv.Value.UnitCamp != unitCamp)
        //             continue;
        //         
        //         if(kv.Value.CurHP != drCard.Values1[0])
        //             continue;
        //         
        //         var dodgeTriggerData = new TriggerData()
        //         {
        //             TriggerDataType = ETriggerDataType.RoleState,
        //             EffectUnitID = kv.Value.ID,
        //             UnitState = EUnitState.Dodge,
        //             Value = drCard.Values1[1],
        //         };
        //         
        //         var shieldTriggerData = new TriggerData()
        //         {
        //             TriggerDataType = ETriggerDataType.RoleState,
        //             EffectUnitID = kv.Value.ID,
        //             UnitState = EUnitState.Shield,
        //             Value = drCard.Values1[1],
        //         };
        //         
        //         useCardData.AddEmptyTriggerDataList(kv.Value.ID);
        //         BattleBuffManager.Instance.CacheTriggerData(dodgeTriggerData, useCardData.TriggerDatas[kv.Value.ID]);
        //         BattleBuffManager.Instance.CacheTriggerData(shieldTriggerData, useCardData.TriggerDatas[kv.Value.ID]);
        //     }
        //
        // }
        
        private void LessHalfHPEnemyHurtAddDamge(EUnitCamp unitCamp, List<float> value1s)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if(kv.Value.UnitCamp == unitCamp)
                    continue;
                
                if(kv.Value.CurHP >= kv.Value.MaxHP * value1s[0])
                    continue;
                
                var triggerData = new TriggerData()
                {
                    TriggerDataType = ETriggerDataType.RoleState,
                    EffectUnitIdx = kv.Value.Idx,
                    UnitStateDetail = new UnitStateDetail()
                    {
                        UnitState = EUnitState.HurtAddDmg,
                        Value = (int)value1s[1],
                    },
                };
                
                useCardData.AddEmptyTriggerDataList(kv.Value.Idx);
                BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.Idx]);
            }

        }
        
        private void MoreHalfHPEnemySubDamge(EUnitCamp unitCamp, List<float> value1s)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {

                if(kv.Value.UnitCamp == unitCamp)
                    continue;
                
                if(kv.Value.CurHP <= kv.Value.MaxHP * value1s[0])
                    continue;

                var triggerData = new TriggerData()
                {
                    TriggerDataType = ETriggerDataType.RoleState,
                    EffectUnitIdx = kv.Value.Idx,
                    UnitStateDetail = new UnitStateDetail()
                    {
                        UnitState = EUnitState.AddDmg,
                        Value = (int)value1s[1],
                    },
                };
                
                useCardData.AddEmptyTriggerDataList(kv.Value.Idx);
                BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.Idx]);
            }

        }
        
        private void UnitAddCurHP(Data_BattleUnit effectUnit, List<float> value1s)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            var value = effectUnit.UnitRole == EUnitRole.Staff ? value1s[0] : value1s[1];

            var triggerData = new TriggerData()
            {
                TriggerDataType = ETriggerDataType.RoleAttribute,
                EffectUnitIdx = effectUnit.Idx,
                BattleUnitAttribute = EUnitAttribute.HP,
                Value = value,
                ChangeHPInstantly = true,
            };
            
            useCardData.AddEmptyTriggerDataList(effectUnit.Idx);
            BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[effectUnit.Idx]);

        }
        
        private void RemoveDebuff(EUnitCamp unitCamp, Data_BattleUnit effectUnit)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            var keyList = effectUnit.UnitStateData.UnitStates.Keys.ToList();
            for (int i = effectUnit.UnitStateData.UnitStates.Count - 1; i >= 0; i--)
            {
                var state = keyList[i];
                if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Contains(state) && effectUnit.UnitCamp == unitCamp)
                {
                    var triggerData = new TriggerData()
                    {
                        TriggerDataType = ETriggerDataType.RoleState,
                        EffectUnitIdx = effectUnit.Idx,
                        UnitStateDetail = new UnitStateDetail(effectUnit.UnitStateData.UnitStates[state]),
 
                    };
            
                    useCardData.AddEmptyTriggerDataList(effectUnit.Idx);
                    BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[effectUnit.Idx]);
                }
            }
            
            // foreach (var kv in effectUnit.UnitState.UnitStates)
            // {
            //     if (Constant.Battle.EffectBuffs[EBuffEffectType.Debuff].Contains(kv.Key) && effectUnit.UnitCamp == unitCamp)
            //     {
            //         effectUnit.UnitState.UnitStates[kv.Key] = 0;
            //     }
            //     if (Constant.Battle.EffectBuffs[EBuffEffectType.Buff].Contains(kv.Key) && effectUnit.UnitCamp != unitCamp)
            //     {
            //         effectUnit.UnitState.UnitStates[kv.Key] = 0;
            //     }
            // }

        }

        private void RemoveCardAddCurHP(Data_BattleUnit effectUnit)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;
            var triggerData = new TriggerData()
            {
                TriggerDataType = ETriggerDataType.RemoveUnit,
                BattleUnitAttribute = EUnitAttribute.HP,
                EffectUnitIdx = effectUnit.Idx,
                Value = -effectUnit.CurHP,
            };
            useCardData.AddEmptyTriggerDataList(effectUnit.Idx);
            BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[effectUnit.Idx]);
        }

        private void State1AddState2(EUnitCamp unitCamp, EUnitState state1, EUnitState state2, List<float> value1s)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if(kv.Value.UnitCamp == unitCamp)
                    continue;
                
                if(kv.Value.GetStateCount(state1) <= 0)
                    continue;

                var triggerData = new TriggerData()
                {
                    TriggerDataType = ETriggerDataType.RoleState,
                    EffectUnitIdx = kv.Value.Idx,
                    UnitStateDetail = new UnitStateDetail()
                    {
                        UnitState = state2,
                        Value = (int)value1s[0],
                    },
                };
                
                useCardData.AddEmptyTriggerDataList(kv.Value.Idx);
                BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.Idx]);
            }
        }
        
        private void UnMoveHeroUnit(EUnitCamp unitCamp)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            var battleHero = BattleFightManager.Instance.RoundFightData.GamePlayData.GetPlayerData(unitCamp).BattleHero;
            var aroundHeroRange = GameUtility.GetRange(battleHero.GridPosIdx, EActionType.Direct82Short, unitCamp,
                new List<ERelativeCamp>()
                {
                    ERelativeCamp.Enemy
                });


            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
  
                if(kv.Value.UnitCamp == unitCamp)
                    continue;
                
                if(aroundHeroRange.Contains(kv.Value.GridPosIdx))
                    continue;

                var triggerData = new TriggerData()
                {
                    TriggerDataType = ETriggerDataType.RoleState,
                    EffectUnitIdx = kv.Value.Idx,
                    UnitStateDetail = new UnitStateDetail()
                    {
                        UnitState = EUnitState.UnMove,
                        Value = 1,
                    },
                };
                
                useCardData.AddEmptyTriggerDataList(kv.Value.Idx);
                BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.Idx]);
            }

        }

        private void AddRoundStates(EBuffID buffID, EUnitCamp unitCamp)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            var battleHero = BattleFightManager.Instance.RoundFightData.GamePlayData.GetPlayerData(unitCamp).BattleHero;
            var triggerData = new TriggerData()
            {
                TriggerDataType = ETriggerDataType.RoundBuff,
                EffectUnitIdx = battleHero.Idx,
                //BuffID = buffID,
                Value = 1,
            };
                
            useCardData.AddEmptyTriggerDataList(triggerData.EffectUnitIdx);
            BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[triggerData.EffectUnitIdx]);
        }
        
        // private void AddStates(int buffID, Data_BattleUnit effectUnit, List<float> value1s)
        // {
        //     var drBuff = BattleBuffManager.Instance.GetBuffData(buffID);
        //
        //     var useCardData = FightManager.Instance.RoundFightData.BuffData_Use;
        //
        //     var triggerData = new TriggerData()
        //     {
        //         TriggerDataType = ETriggerDataType.RoleState,
        //         EffectUnitID = effectUnit.ID,
        //         UnitState = drBuff.UnitState,
        //         Value = value1s[0],
        //     };
        //         
        //     useCardData.AddEmptyTriggerDataList(triggerData.EffectUnitID);
        //     BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[triggerData.EffectUnitID]);
        // }

        // private void TacticCard_UnAttackHeroUnit(int cardID, EUnitCamp unitCamp, Data_BattleUnit effectUnit)
        // {
        //     var useCardData = FightManager.Instance.RoundFightData.UseCardData;
        //     var drCard = CardManager.Instance.GetCardTable(cardID);
        //     var battleHero = FightManager.Instance.RoundFightData.GamePlayData.GetPlayerData(unitCamp).BattleHero;
        //     var aroundHeroRange = GameUtility.GetRange(battleHero.GridPosIdx, EActionType.Around, unitCamp,
        //         new List<ERelativeCamp>()
        //         {
        //             ERelativeCamp.Enemy
        //         });
        //
        //
        //     foreach (var kv in FightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
        //     {
        //
        //         if(kv.Value.UnitCamp == unitCamp)
        //             continue;
        //         
        //         if(!aroundHeroRange.Contains(kv.Value.GridPosIdx))
        //             continue;
        //         
        //         var triggerData = new TriggerData()
        //         {
        //             TriggerDataType = ETriggerDataType.RoleState,
        //             EffectUnitID = kv.Value.ID,
        //             UnitState = EUnitState.UnAttack,
        //             Value = 1,
        //         };
        //         
        //         useCardData.AddEmptyTriggerDataList(kv.Value.ID);
        //         BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[kv.Value.ID]);
        //     }
        //
        // }
        //
        private void Link(EBuffID buffID, Data_BattleUnit effectUnit)
        {
            var useCardData = BattleFightManager.Instance.RoundFightData.BuffData_Use;

            var linkID = GameUtility.BuffIDToLinkID(buffID);
            effectUnit.BattleLinkIDs.Add(linkID);
            
            var triggerData = new TriggerData()
            {
                TriggerDataType = ETriggerDataType.Link,
                EffectUnitIdx = effectUnit.Idx,
                LinkID = linkID,
            };
            
            useCardData.AddEmptyTriggerDataList(effectUnit.Idx);
            BattleBuffManager.Instance.CacheTriggerData(triggerData, useCardData.TriggerDatas[effectUnit.Idx]);

        }    
    }
}