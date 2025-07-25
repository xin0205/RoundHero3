﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework;
using UnityGameFramework.Runtime;


namespace RoundHero
{
    public class BuffData
    {
        public EBuffTriggerType BuffTriggerType = EBuffTriggerType.Empty;
        public List<ERelativeCamp> TriggerUnitCamps = new List<ERelativeCamp>();
        public EActionType TriggerRange = EActionType.Empty;
        public EFlyType FlyType = EFlyType.Empty;
        public EActionType FlyRange = EActionType.Empty;
        public List<ETriggerTarget> TriggerTargets = new List<ETriggerTarget>();
        public EBuffValueType BuffValueType = EBuffValueType.Empty;
        public List<EUnitStateEffectType> UnitStateEffectTypes = new List<EUnitStateEffectType>();
        public bool RangeTrigger;
        public EHeroAttribute HeroAttribute = EHeroAttribute.Empty;
        public EUnitAttribute UnitAttribute = EUnitAttribute.Empty;
        public EUnitState UnitState = EUnitState.Empty;
        public ECardTriggerType CardTriggerType = ECardTriggerType.Empty;
        public string BuffStr;
        public EBuffEquipType BuffEquipType = EBuffEquipType.Normal;
        public ETriggerTarget TriggerInitiator = ETriggerTarget.Empty;

        public BuffData Copy()
        {
            var buffData = new BuffData();
            buffData.BuffTriggerType = BuffTriggerType;
            buffData.TriggerUnitCamps = new List<ERelativeCamp>(TriggerUnitCamps);
            buffData.TriggerRange = TriggerRange;
            buffData.FlyType = FlyType;
            buffData.FlyRange = FlyRange;
            buffData.TriggerTargets = new List<ETriggerTarget>(TriggerTargets);
            buffData.BuffValueType = BuffValueType;
            buffData.HeroAttribute = HeroAttribute;
            buffData.UnitAttribute = UnitAttribute;
            buffData.UnitState = UnitState;
            buffData.CardTriggerType = CardTriggerType;
            buffData.BuffStr = BuffStr;

            return buffData;
        }
        //public List<string> Values;
    }
    // public class BuffTriggerDataContext
    // {
    //     public List<BuffTriggerData> BuffTriggerDatas { get; set; }
    // }
    //
    // public class BuffTriggerData
    // {
    //     public EUnitRole EffectEunitRole { get; set; }
    //
    //     public ERelativeCamp EffectRelativeCamp { get; set; }
    //     public ERelativeCamp TriggerRelativeCamp { get; set; }
    //     public EBuffValueType BuffValueType { get; set; }
    //     public EUnitAttribute UnitAttribute { get; set; }
    //     public EUnitState UnitState { get; set; }
    //     public float Value { get; set; }
    //
    // }

    public enum TriggerBuffType
    {
        Card,
        EnergyBuff,
        Empty,
    }

    public class TriggerBuffData
    {
        public TriggerBuffType TriggerBuffType;
        // public EBuffID BuffID;
        public int CardIdx;
        public Data_EnergyBuff EnergyBuffData = new Data_EnergyBuff();

        public TriggerBuffData Copy()
        {
            var triggerBuffData = new TriggerBuffData();
            triggerBuffData.TriggerBuffType = TriggerBuffType;
            // triggerBuffData.BuffID = BuffID;
            // triggerBuffData.CardID = CardID;
            triggerBuffData.EnergyBuffData = EnergyBuffData.Copy();

            return triggerBuffData;
        }

        public void Clear()
        {
            TriggerBuffType = TriggerBuffType.Empty;
            CardIdx = -1;
            // BuffID = EBuffID.Empty;
            // CardID = -1;
            EnergyBuffData.Clear();
        }
    }

    public partial class BattleBuffManager : Singleton<BattleBuffManager>
    {
        public System.Random Random;
        private int randomSeed;

        //public TriggerBuffData TriggerBuffData = new ();

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);

        }
        
        public void Destory()
        {}
        

        // public TriggerData UsActionEndTrigger(EBuffID buffID, List<float> values, int ownUnitID, int actionUnitID,
        //     int effectUnitID)
        // {
        //     var triggerDatas = new List<TriggerData>();
        //     var retTriggerData = BuffTrigger(ETriggerType.UsActionEnd, buffID, values, ownUnitID, actionUnitID,
        //         effectUnitID, null, triggerDatas);
        //
        //     foreach (var triggerData in triggerDatas)
        //     {
        //         FightManager.Instance.TriggerAction(triggerData);
        //     }
        //
        //     return retTriggerData;
        // }
        
        private List<TriggerData> InternalBuffTrigger(EBuffTriggerType buffTriggerType, BuffData buffData, List<string> values, int ownUnitIdx, int actionUnitIdx,
            int effectUnitIdx, List<TriggerData> triggerDatas, int actionUnitGridPosIdx = -1,
            int actionUnitPreGridPosIdx = -1, int cardIdx = -1, TriggerData preTriggerData = null)
        {
            var actionUnit = GameUtility.GetUnitDataByIdx(actionUnitIdx);
            // if (actionUnit != null && actionUnit.GetAllStateCount(EUnitState.UnAttack) > 0 &&
            //     !GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData, ECardID.RoundDeBuffUnEffect))
            // {
            //     return null;
            // }
        
            //var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
            if (buffTriggerType != buffData.BuffTriggerType)
                return null;
            
            if(buffTriggerType == EBuffTriggerType.UseCard && 
               (BattlePlayerManager.Instance.BattlePlayerData.RoundUseCardCount <= 0 || BattlePlayerManager.Instance.BattlePlayerData.RoundUseCardCount % int.Parse(values[1]) != 0))
                return null;
        
            var buffvalueType = buffData.BuffValueType;
            var _triggerDatas = new List<TriggerData>();
        
            var realEffectUnitIdxs = BattleFightManager.Instance.GetEffectUnitIdxs(buffData, ownUnitIdx, actionUnitIdx, effectUnitIdx ,actionUnitGridPosIdx, actionUnitPreGridPosIdx);

            if (buffvalueType == EBuffValueType.Card)
            {
                realEffectUnitIdxs.Add(effectUnitIdx);
            }
            
            if (realEffectUnitIdxs.Count > 0)
            {
                TriggerData triggerData = null;
                foreach (var realEffectUnitIdx in realEffectUnitIdxs)
                {
                    var realEffectUnit = GameUtility.GetUnitDataByIdx(realEffectUnitIdx);
                    
                    var buffValues = new List<float>();
                    foreach (var value in values)
                    {
                        
                        buffValues.Add(BattleBuffManager.Instance.GetBuffValue(value, realEffectUnitIdx, cardIdx, preTriggerData));
                    }
        
                    switch (buffvalueType)
                    {
                        case EBuffValueType.HeroAtrb:
                            triggerData = BattleFightManager.Instance.Unit_HeroAttribute(ownUnitIdx, actionUnitIdx,
                                realEffectUnitIdx, buffData.HeroAttribute, buffValues[0]);
                            
                            break;
                        case EBuffValueType.Atrb:
                            triggerData = BattleFightManager.Instance.BattleRoleAttribute(ownUnitIdx, actionUnitIdx,
                                realEffectUnitIdx, buffData.UnitAttribute, buffValues[0], ETriggerDataSubType.Unit);
                            break;
                        case EBuffValueType.State:
                            
                            // switch (buffData.BuffTriggerType)
                            // {
                            //     case EBuffTriggerType.TacticClearBuff:
                            //         
                            //     // case EBuffID.Hurt_HeroAddDebuff:
                            //     //     var randomDebuffIdx = Random.Next(0,
                            //     //         Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Count);
                            //     //     unitState = Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative][randomDebuffIdx];
                            //     //     break;
                            // }
                            var unitState = buffData.UnitState;
                            triggerData = BattleFightManager.Instance.Unit_State(triggerDatas, ownUnitIdx, actionUnitIdx, realEffectUnitIdx,
                                unitState, buffValues[0], ETriggerDataType.RoleState);
                            var addEnemyMoreDebuff =
                                BattleFightManager.Instance.RoundFightData.GamePlayData.GetUsefulBless(
                                    EBlessID.AddEnemyMoreDebuff, BattleManager.Instance.CurUnitCamp);
        
        
                            if (addEnemyMoreDebuff != null &&
                                Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(buffData.UnitState) &&
                                realEffectUnit != null &&
                                realEffectUnit.UnitCamp != actionUnit.UnitCamp)
                            {
                                triggerData.DeltaValue += 1;
                            }
        
                            break;
                        case EBuffValueType.RoundState:
                            // var value = values[0];
                            // switch (buffID)
                            // {
                            //     case  EBuffID.Round_AddDefenseSameAttack:
                            //         value = Math.Abs(BattleUnitManager.Instance.GetDamage(effectUnitID)) - 1;
                            //         break;
                            // }
                            triggerData = BattleFightManager.Instance.Unit_State(triggerDatas, ownUnitIdx, actionUnitIdx, realEffectUnitIdx,
                                buffData.UnitState, buffValues[0], ETriggerDataType.RoundRoleState);
                            break;
                        case EBuffValueType.Card:
                            triggerData = BattleFightManager.Instance.Hero_Card(ownUnitIdx, actionUnitIdx, realEffectUnitIdx,
                                buffValues[0], buffData.CardTriggerType);
                            triggerData.EffectUnitIdx = realEffectUnitIdx;
                            break;
                        case EBuffValueType.ClearBuff:
                            triggerData = new TriggerData();
                            triggerData.TriggerDataType = ETriggerDataType.ClearBuff;
                            triggerData.UnitStateEffectTypes = buffData.UnitStateEffectTypes;
                            triggerData.EffectUnitIdx = realEffectUnitIdx;
                            // triggerData.BuffValue = new BuffValue()
                            // {
                            //     BuffData = buffData,
                            //     
                            // }
                            
                            break;
                        case EBuffValueType.TransferBuff:
                            triggerData = new TriggerData();
                            triggerData.TriggerDataType = ETriggerDataType.TransferBuff;
                            triggerData.UnitStateEffectTypes = buffData.UnitStateEffectTypes;
                            if(buffData.TriggerInitiator == ETriggerTarget.Effect)
                                triggerData.ActionUnitIdx = effectUnitIdx;
                            else if(buffData.TriggerInitiator == ETriggerTarget.Action)
                                triggerData.ActionUnitIdx = actionUnitIdx;    
                            
                            triggerData.EffectUnitIdx = realEffectUnitIdx;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
        
        
                    if (triggerData != null)
                    {
                        _triggerDatas.Add(triggerData);
                        triggerData.BuffTriggerType = buffTriggerType;
                        //realEffectUnit.UnitRole == EUnitRole.Hero && 
                        
                        //buffTriggerType != EBuffTriggerType.Use && 
                        if (realEffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx ||
                            (realEffectUnit != null && realEffectUnit.UnitRole == EUnitRole.Hero) &&
                            buffvalueType == EBuffValueType.Atrb &&
                            buffData.UnitAttribute == EUnitAttribute.HP &&
                            (buffTriggerType != EBuffTriggerType.UseCard))

                        {
              
                            triggerData.ChangeHPInstantly = false;
                        }
        
                        if (actionUnit != null)
                        {
                            triggerData.ActionUnitGridPosIdx = actionUnit.GridPosIdx;
                        }
                        
                        
                        triggerData.EffectUnitGridPosIdx = realEffectUnit != null ? realEffectUnit.GridPosIdx : -1;
                        CacheTriggerData(triggerData, triggerDatas);
                        
                        var isSubCurHP = false;
                        
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
                }
            }
            else
            {
                // if (buffID == EBuffID.Hurt_AddCard)
                // {
                //     triggerData = FightManager.Instance.Hero_Card(ownUnitID, actionUnitID, actionUnitID, 
                //         (float)ECardID.UnRemove, ECardTriggerType.AddSpecificCard);
                //     
                // }
                // else if (buffID == EBuffID.Hurt_ConsumeCard)
                // {
                //     var unit = BattleUnitManager.Instance.GetUnitByID(preTriggerData.ActionUnitID) as BattleSoliderEntity;
                //     if (unit != null)
                //     {
                //         triggerData = FightManager.Instance.Hero_Card(ownUnitID, actionUnitID, actionUnitID, 
                //             unit.BattleSoliderEntityData.BattleSoliderData.CardID, ECardTriggerType.ConsumeSpecificCard);
                //     }
                //     
                // }
                // else if (buffID == EBuffID.Hurt_CardStandbyToPass)
                // {
                //     triggerData = FightManager.Instance.Hero_Card(ownUnitID, actionUnitID, actionUnitID, 
                //         1, ECardTriggerType.StandByToPass);
                //     
                // }
                //PostTrigger(triggerData, triggerDatas);
            }
        
            
        
            return _triggerDatas;
        }

        public void CacheTriggerData(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var actionUnit = GameUtility.GetUnitDataByIdx(triggerData.ActionUnitIdx);
            
            if (triggerData.BattleUnitAttribute == EUnitAttribute.HP &&
                triggerData.Value + triggerData.DeltaValue < 0)
            {
                triggerData.Value += actionUnit == null ? 0 : actionUnit.BaseDamage;

                var effectUnit = GameUtility.GetUnitDataByIdx(triggerData.EffectUnitIdx);
                //effectUnit.GetAllStateCount(EUnitState.Dodge) <= 0 || 
                if (effectUnit != null && effectUnit.GetAllStateCount(EUnitState.UnHurt) <= 0)
                {
                    //var ownUnit = GameUtility.GetUnitByID(triggerData.OwnUnitID);
                    if (Math.Abs(triggerData.Value + triggerData.DeltaValue) >= effectUnit.CurHP)
                    {
                        FuneManager.Instance.CacheUnitKillData(triggerData.OwnUnitIdx,
                            triggerData.ActionUnitIdx, triggerData.EffectUnitIdx, triggerDatas);
                    }
                    //CacheHurtTriggerDatas(triggerData, triggerDatas);
                }

            }

                        
            PostTrigger(triggerData, triggerDatas);
        }

        public void PostTrigger(TriggerData triggerData, List<TriggerData> triggerDatas, bool isAddTriggerData = true)
        {
            if(triggerData == null)
                return;
   
            BattleFightManager.Instance.SimulateTriggerData(triggerData, triggerDatas);
            if (isAddTriggerData)
            {
                triggerDatas.Add(triggerData);
            }
            
            
            HurtTrigger(triggerData, triggerDatas);
        }
        
        public void AttackTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            if (!(triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                triggerData.BattleUnitAttribute == EUnitAttribute.HP &&
                triggerData.Value + triggerData.DeltaValue < 0))
                return;
            
            var actionUnit = BattleFightManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
            if(actionUnit == null || !actionUnit.Exist())
                return;
            
            BuffsTrigger(BattleFightManager.Instance.RoundFightData.GamePlayData, actionUnit, triggerData, triggerDatas, EBuffTriggerType.Attack);
            BattleGridPropManager.Instance.AttackTrigger(actionUnit.GridPosIdx, triggerData, triggerDatas);
            BattleFightManager.Instance.TriggerUnitData(actionUnit.Idx, triggerData.EffectUnitIdx, actionUnit.GridPosIdx,
                EBuffTriggerType.Attack, triggerDatas);
        }
        
        private void HurtTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            if (!(triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                  triggerData.BattleUnitAttribute == EUnitAttribute.HP &&
                  triggerData.Value + triggerData.DeltaValue < 0))
                return;
            
            var effectUnit = BattleFightManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            // || effectUnit.CurHP <= 0
            if(effectUnit == null)
                return;
            
            if(!triggerData.ChangeHPInstantly)
                return;

            // if (effectUnit.BuffCount(EBuffID.Round_AttackUnitUnAttack) > 0)
            // {
            //     var actionUnit = FightManager.Instance.GetUnitByID(triggerData.ActionUnitID);
            //     if (actionUnit != null)
            //     {
            //         triggerData = FightManager.Instance.Unit_State(triggerDatas, effectUnit.ID, effectUnit.ID, actionUnit.ID,
            //             EUnitState.UnAttack, 1, ETriggerDataType.RoleState);
            //         FightManager.Instance.SimulateTriggerData(triggerData, triggerDatas);
            //         triggerDatas.Add(triggerData);
            //     }
            // }
            
            

            BuffsTrigger(BattleFightManager.Instance.RoundFightData.GamePlayData, effectUnit, triggerData, triggerDatas, EBuffTriggerType.Hurt);
  
            BattleFightManager.Instance.TriggerUnitData(effectUnit.Idx, triggerData.ActionUnitIdx, effectUnit.GridPosIdx, EBuffTriggerType.Hurt, triggerDatas);
        }
        
        public void BuffsTrigger(Data_GamePlay gamePlayData, Data_BattleUnit unit, TriggerData triggerData, List<TriggerData> triggerDatas, EBuffTriggerType buffTriggerType)
        {
            BattleUnitManager.Instance.GetBuffValue(gamePlayData, unit, out List<BuffValue> triggerBuffDatas, -1, triggerData);
            
            if(triggerBuffDatas == null)
                return;
            
            var idx = -1;
            foreach (var triggerBuffData in triggerBuffDatas)
            {
                idx++;
                if(triggerBuffData.BuffData.BuffTriggerType != buffTriggerType)
                    continue;
                
                if(triggerBuffData.BuffData.RangeTrigger)
                    continue;
                
                BuffTrigger(buffTriggerType,
                    triggerBuffData.BuffData, triggerBuffData.ValueList, triggerData.EffectUnitIdx, triggerData.ActionUnitIdx, triggerData.EffectUnitIdx,
                    triggerDatas, -1, -1, triggerData);
    
            }
           
        }

        // public TriggerData UsActionEndTrigger(EBuffID buffID, List<float> values, int ownUnitID, int actionUnitID,
        //     int effectUnitID, List<TriggerData> triggerDatas)
        // {
        //     return BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.ActionEnd, buffID, values, ownUnitID, actionUnitID,
        //         effectUnitID, null, triggerDatas);
        // }
        
        public void RoundStartTrigger(BuffData buffData, List<float> values, int ownUnitID, int actionUnitID,
            int effectUnitID, List<TriggerData> triggerDatas)
        {
            BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.RoundStart, buffData, values, ownUnitID, actionUnitID,
                effectUnitID, triggerDatas);
        }
        
        public void AutoAttackTrigger(BuffData buffData, List<float> values, int ownUnitID, int actionUnitID,
            int effectUnitID, List<TriggerData> triggerDatas)
        {

            BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.AutoAttack, buffData, values, ownUnitID, actionUnitID,
                effectUnitID, triggerDatas);
        }

        // public int GetHurtTimes(Data_GamePlay gamePlayData, int buffID)
        // {
        //     var hurtTimes = 0;
        //     foreach (var kv in gamePlayData.LastBattleData.BattleUnitDatas)
        //     {
        //         if (kv.Value.BuffCount(buffID) > 0 && kv.Value.HurtTimes > 0)
        //         {
        //             hurtTimes += kv.Value.HurtTimes;
        //         }
        //     }
        //
        //     return hurtTimes;
        // }
        
        public int GetHurtTimes(Data_GamePlay gamePlayData, EBuffID buffID)
        {
            var hurtTimes = 0;
            foreach (var kv in gamePlayData.LastRoundBattleData.BattleUnitDatas)
            {
                if (kv.Value.BuffCount(buffID.ToString()) > 0 && kv.Value.HurtTimes > 0)
                {
                    hurtTimes += kv.Value.HurtTimes;
                }
            }

            return hurtTimes;
        }

        public List<int> GetUnLinkPosIdxs(Data_GamePlay gamePlayData, EUnitCamp unitCamp)
        {
            var posIdxs = new List<int>();
            foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
            {
                if(kv.Value.UnitCamp == unitCamp)
                    continue;
                
                // if (kv.Value.BuffCount(EBuffID.LinkUnEffect) > 0)
                // {
                //     var range = GameUtility.GetRange(kv.Value.GridPosIdx, EActionType.Around);
                //     posIdxs.AddRange(range);
                // }
            }

            return posIdxs;

        }

        public List<int> GetUnPlacePosIdxs(Data_GamePlay gamePlayData, List<EUnitCamp> exceptUnitCamps = null)
        {
            var posIdxs = new List<int>();
            foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
            {
                if(exceptUnitCamps != null && exceptUnitCamps.Contains(kv.Value.UnitCamp))
                    continue;
                
                if (kv.Value.BuffCount(EBuffID.Spec_UnPlaceEnemyUnit.ToString()) > 0)
                {
                    var range = GameUtility.GetRange(kv.Value.GridPosIdx, EActionType.Direct82Short, EUnitCamp.Empty, null, false, true);
                    posIdxs.AddRange(range);
                }
                else
                {
                    posIdxs.Add(kv.Value.GridPosIdx);
                }
            }
            
            foreach (var kv in gamePlayData.BattleData.GridPropDatas)
            {
                if(exceptUnitCamps != null && exceptUnitCamps.Contains(kv.Value.UnitCamp))
                    continue;
                
                var isStayProp = BattleGridPropManager.Instance.IsStayProp(kv.Value.GridPropID);
                if (isStayProp)
                {
                    continue;
                }

                
                posIdxs.Add(kv.Value.GridPosIdx);
            }

            return posIdxs;
        }


        // public void UseCardTrigger()
        // {
        //     var triggerDatas = new List<TriggerData>();
        //     var emptyTriggerData = new TriggerData();
        //     foreach (var kv in BattleUnitManager.Instance.BattleUnitDatas)
        //     {
        //         BuffsTrigger(FightManager.Instance.RoundFightData.GamePlayData, kv.Value, emptyTriggerData, triggerDatas, EBuffTriggerType.UseCard);
        //
        //     }
        //
        //     foreach (var triggerData in triggerDatas)
        //     {
        //         FightManager.Instance.TriggerAction(triggerData);
        //     }
        //     
        // }

        public class UnitDisplacementData
        {
            public int ActionUnitID;
            public int PreGridPosIdx;
            public int CurGridPosIdx;

            public UnitDisplacementData(int actionUnitID, int preGridPosIdx, int curGridPosIdx)
            {
                ActionUnitID = actionUnitID;
                PreGridPosIdx = preGridPosIdx;
                CurGridPosIdx = curGridPosIdx;
            }

        }

        public class UnitDisplacementResult
        {
            public int BeTriggerUnitID;
            public int TriggerUnitID;
            public EBuffTriggerType BuffTriggerType;
            
            public UnitDisplacementResult(int beTriggerUnitID, int triggerUnitID, EBuffTriggerType buffTriggerType)
            {
                BeTriggerUnitID = beTriggerUnitID;
                TriggerUnitID = triggerUnitID;
                BuffTriggerType = buffTriggerType;
            }
        }
        
        public List<UnitDisplacementResult> CacheUnitRangeTrigger(Data_GamePlay gamePlayData, List<UnitDisplacementData> unitDisplacementDatas)
        {
            var unitDisplacementResults = new List<UnitDisplacementResult>();
            
            foreach (var unitDisplacementData in unitDisplacementDatas)
            {
                var actionUnit = GameUtility.GetUnitDataByIdx(unitDisplacementData.ActionUnitID, true);
                actionUnit.GridPosIdx = unitDisplacementData.PreGridPosIdx;
            }
            var preUnitRanges = RecordUnitRange(gamePlayData);
            
            foreach (var unitDisplacementData in unitDisplacementDatas)
            {
                var actionUnit = GameUtility.GetUnitDataByIdx(unitDisplacementData.ActionUnitID, true);
                actionUnit.GridPosIdx = unitDisplacementData.CurGridPosIdx;
            }
            var curUnitRanges = RecordUnitRange(gamePlayData);
            
            foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
            {
                var preUnitRange = new List<int>();
                if (preUnitRanges.Contains(kv.Key))
                {
                    preUnitRange = preUnitRanges[kv.Key].ToList();
                }
                
                var curUnitRange = new List<int>();
                if (curUnitRanges.Contains(kv.Key))
                {
                    curUnitRange = curUnitRanges[kv.Key].ToList();
                }
                
                BattleUnitManager.Instance.GetBuffValue(gamePlayData, kv.Value, out List<BuffValue> unitTriggerBuffDatas);

                //离开触发
                var preUnitRangeOwn = preUnitRange.Except(curUnitRange).ToList();
                foreach (var leaveUnitID in preUnitRangeOwn)
                {
                    var leaveUnit = GameUtility.GetUnitDataByIdx(leaveUnitID, true);
                    
                    foreach (var triggerBuffData in unitTriggerBuffDatas)
                    {
                        var buffData = triggerBuffData.BuffData;
                        if (buffData.BuffTriggerType == EBuffTriggerType.OtherLeaveRange &&
                            GameUtility.CheckUnitCamp(buffData.TriggerUnitCamps, kv.Value.UnitCamp, leaveUnit.UnitCamp))
                        {
                            var leaveResult = new UnitDisplacementResult(leaveUnitID, kv.Key, EBuffTriggerType.OtherLeaveRange);
                            unitDisplacementResults.Add(leaveResult);
                        }

                    }
                    
                    BattleUnitManager.Instance.GetBuffValue(gamePlayData, leaveUnit, out List<BuffValue> leaveUnitTriggerBuffDatas);
                    
                    foreach (var triggerBuffData in leaveUnitTriggerBuffDatas)
                    {
                        var buffData = triggerBuffData.BuffData;
                        if (buffData.BuffTriggerType == EBuffTriggerType.SelfLeaveRange &&
                            GameUtility.CheckUnitCamp(buffData.TriggerUnitCamps, kv.Value.UnitCamp, leaveUnit.UnitCamp))
                        {
                            var leaveResult = new UnitDisplacementResult(kv.Key, leaveUnitID, EBuffTriggerType.SelfLeaveRange);
                            unitDisplacementResults.Add(leaveResult);
                        }
                    }

                }
                
                var curUnitRangeOwn = curUnitRange.Except(preUnitRange).ToList();
                foreach (var enterUnitID in curUnitRangeOwn)
                {
                    var enterUnit = GameUtility.GetUnitDataByIdx(enterUnitID, true);
                    foreach (var triggerBuffData in unitTriggerBuffDatas)
                    {
                        var buffData = triggerBuffData.BuffData;
                        if (buffData.BuffTriggerType == EBuffTriggerType.OtherEnterRange &&
                            GameUtility.CheckUnitCamp(buffData.TriggerUnitCamps, kv.Value.UnitCamp, enterUnit.UnitCamp))
                        {
                            var enterResult = new UnitDisplacementResult( enterUnitID, kv.Key, EBuffTriggerType.OtherEnterRange);
                            unitDisplacementResults.Add(enterResult);
                        }

                    }
                    
                    
                    BattleUnitManager.Instance.GetBuffValue(gamePlayData, enterUnit, out List<BuffValue> enterUnitTriggerBuffDatas);
                    
                    foreach (var triggerBuffData in enterUnitTriggerBuffDatas)
                    {
                        var buffData = triggerBuffData.BuffData;
                        if (buffData.BuffTriggerType == EBuffTriggerType.SelfEnterRange &&
                            GameUtility.CheckUnitCamp(buffData.TriggerUnitCamps, kv.Value.UnitCamp, enterUnit.UnitCamp))
                        {
                            var enterResult = new UnitDisplacementResult(  kv.Key, enterUnitID, EBuffTriggerType.SelfEnterRange);
                            unitDisplacementResults.Add(enterResult);
                        }
                    }
                    
                    
                    
                }
            }

            return unitDisplacementResults;
        }

        private GameFrameworkMultiDictionary<int, int> RecordUnitRange(Data_GamePlay gamePlayData)
        {
            var unitRanges = new GameFrameworkMultiDictionary<int, int>();
            foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
            {
                BattleUnitManager.Instance.GetBuffValue(gamePlayData, kv.Value, out List<BuffValue> triggerBuffDatas);
                
                foreach (var triggerBuffData in triggerBuffDatas)
                {
                    var buffData = triggerBuffData.BuffData;
                    var range = GameUtility.GetRange(kv.Value.GridPosIdx, buffData.TriggerRange, kv.Value.UnitCamp,
                        buffData.TriggerUnitCamps, true);

                    foreach (var gridPosIdx in range)
                    {
                        var unit = GameUtility.GetUnitByGridPosIdx(gridPosIdx, true);
                        if (unit != null)
                        {
                            unitRanges.Add(kv.Key, unit.Idx);
                        }
                    }
                }
            }

            return unitRanges;
        }
        
        public void RecoverUseBuffState()
        {
            //GameEntry.Event.FireNow(null, RefreshCardUseTipsEventArgs.Create(false));
            BattleManager.Instance.SetBattleState(EBattleState.UseCard);
            BattleAreaManager.Instance.IsMoveGrid = false;
            BattleCardManager.Instance.UnSelectCard();
            //CardUseLine.gameObject.SetActive(false);

            
            BattleManager.Instance.TempTriggerData.TriggerBuffData.Clear();
            //SelectEnemyEntityID = -1;
            BattleCardManager.Instance.ResetCardUseType();
            BattleCardManager.Instance.ResetCardsPos(true);
        }
        
        public void TriggerBuff()
        {
            foreach (var kv in BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas)
            {
                foreach (var triggerData in kv.Value)
                {
                    BattleFightManager.Instance.TriggerAction(triggerData);
                }
            }


        }
        
        public void UseBuff(int gridPosIdx, int unitID = -1)
        {
            if (BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType == TriggerBuffType.Card)
            {
                var cardIdx = BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx;
                BattleCardManager.Instance.CardEntities[cardIdx].UseCardAnimation(gridPosIdx);
                BattleCardManager.Instance.UseCard(cardIdx, unitID);
                
            }
            else if (BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType == TriggerBuffType.EnergyBuff)
            {
                BattleEnergyBuffManager.Instance.UseEnergyBuff(
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.Heart,
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.HP);
            }
        }

        

        public Dictionary<string, BuffData> CacheBuffDatas = new Dictionary<string, BuffData>();


        public List<BuffData> GetBuffData(int buffID)
        {
            var drBuff = GameEntry.DataTable.GetBuff(buffID);
            var buffDatas = new List<BuffData>();
            foreach (var buffStr in drBuff.BuffIDs)
            {
                buffDatas.Add(GetBuffData(buffStr));
            }

            return buffDatas;
        }
        
        public BuffData GetBuffData(string buffKey)
        {
            //Log.Debug("GetBuffData:" + buffKey);
            if (CacheBuffDatas.ContainsKey(buffKey))
                return CacheBuffDatas[buffKey];

            var strList = buffKey.Split("_");
            var buffData = new BuffData();

            buffData.BuffStr = buffKey;
            buffData.BuffTriggerType = Enum.Parse<EBuffTriggerType>(strList[0]);
            //buffData.DrBuff = GameEntry.DataTable.GetBuff(buffKey);
            
            switch (buffData.BuffTriggerType)
            {
                case EBuffTriggerType.ActionEnd:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.BePass:
                case EBuffTriggerType.Pass:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.Move:
                    BuffParse_Normal(strList, buffData);
                    buffData.RangeTrigger = true;
                    break;
                case EBuffTriggerType.Attack:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.Stay:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.Hurt:
                    break;
                case EBuffTriggerType.Dead:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.Kill:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.UseCard:
                    
                    break;
                case EBuffTriggerType.Use:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.Link:
                    break;
                case EBuffTriggerType.Trigger:
                    break;
                case EBuffTriggerType.RoundStart:
                    break;
                case EBuffTriggerType.RoundEnd:
                    break;
                case EBuffTriggerType.Empty:
                    break;
                
                case EBuffTriggerType.SelfEnterRange:
                    break;
                case EBuffTriggerType.SelfLeaveRange:
                    break;
                case EBuffTriggerType.OtherEnterRange:
                    break;
                case EBuffTriggerType.OtherLeaveRange:
                    break;
                case EBuffTriggerType.ActiveAttack:
                    break;
                case EBuffTriggerType.Collide:
                    BuffParse_Collide(strList, buffData);
                    break;
                case EBuffTriggerType.SelectUnit:
                    BuffParse_SelectUnit(strList, buffData);
                    break;
                case EBuffTriggerType.TacticSelectUnit:
                    BuffParse_TacticSelectUnit(strList, buffData);
                    break;
                case EBuffTriggerType.TacticSelectGrid:
                    BuffParse_TacticSelectGrid(strList, buffData);
                    break;
                case EBuffTriggerType.SelectGrid:
                    BuffParse_SelectGrid(strList, buffData);
                    break;
                
                case EBuffTriggerType.SelectCard:
                    break;
                case EBuffTriggerType.AutoAttack:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.AddDeBuff:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.SingleRound:
                    break;
                case EBuffTriggerType.ChangeHP:
                    break;
                case EBuffTriggerType.Spec:
                    BuffParse_Spec(buffKey, strList, buffData);
                    break;
                case EBuffTriggerType.TacticAtrb:
                    BuffParse_Normal(strList, buffData);
                    break;
                case EBuffTriggerType.TacticClearBuff:
                    BuffParse_ClearBuff(strList, buffData);
                    break;
                case EBuffTriggerType.TacticProp:
                    BuffParse_TacticProp(strList, buffData);
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            CacheBuffDatas.Add(buffKey, buffData);
            return buffData;
        }

        public void BuffParse_Normal(string[] strList, BuffData buffData)
        {
            buffData.TriggerRange = Enum.Parse<EActionType>(strList[1]);
            
            var unitCamps = strList[2].Split("|");
            foreach (var unitCamp in unitCamps)
            {
                buffData.TriggerUnitCamps.Add(Enum.Parse<ERelativeCamp>(unitCamp));
            }
            
            var triggerTargets = strList[3].Split("|");
            foreach (var triggerTarget in triggerTargets)
            {
                buffData.TriggerTargets.Add(Enum.Parse<ETriggerTarget>(triggerTarget));
            }
            
            buffData.BuffValueType = Enum.Parse<EBuffValueType>(strList[4]);

            switch (buffData.BuffValueType)
            {
                case EBuffValueType.Atrb:
                    buffData.UnitAttribute = Enum.Parse<EUnitAttribute>(strList[5]);
                    break;
                case EBuffValueType.HeroAtrb:
                    buffData.HeroAttribute = Enum.Parse<EHeroAttribute>(strList[5]);
                    break;
                case EBuffValueType.State:
                    buffData.UnitState = Enum.Parse<EUnitState>(strList[5]);
                    break;
                case EBuffValueType.Card:
                    buffData.CardTriggerType = Enum.Parse<ECardTriggerType>(strList[5]);
                    break;
                case EBuffValueType.ClearBuff:
                    var unitStateEffectTypesStr = strList[5].Split("|");
                    
                    buffData.UnitStateEffectTypes = GameUtility.GetEnums<EUnitStateEffectType>(unitStateEffectTypesStr.ToList());
                    break;
                case EBuffValueType.Empty:
                    break;
                default:
                    break;
            }

            if (strList.Length >= 8)
            {
                buffData.FlyType = Enum.Parse<EFlyType>(strList[6]);
                buffData.FlyRange = Enum.Parse<EActionType>(strList[7]);
            }
            
        }
        
        public void BuffParse_Collide(string[] strList, BuffData buffData)
        {

            var unitCamps = strList[1].Split("|");
            foreach (var unitCamp in unitCamps)
            {
                buffData.TriggerUnitCamps.Add(Enum.Parse<ERelativeCamp>(unitCamp));
            }
            
            buffData.TriggerInitiator = Enum.Parse<ETriggerTarget>(strList[2]);

            
            var triggerTargets = strList[3].Split("|");
            foreach (var triggerTarget in triggerTargets)
            {
                buffData.TriggerTargets.Add(Enum.Parse<ETriggerTarget>(triggerTarget));
            }
            
            buffData.BuffValueType = Enum.Parse<EBuffValueType>(strList[4]);
            
            var unitStateEffectTypes = strList[5].Split("|");;
            foreach (var unitStateEffectType in unitStateEffectTypes)
            {
                buffData.UnitStateEffectTypes.Add(Enum.Parse<EUnitStateEffectType>(unitStateEffectType));
            }
            
        }
        
        
        public void BuffParse_ClearBuff(string[] strList, BuffData buffData)
        {
            buffData.BuffValueType = EBuffValueType.ClearBuff;
            buffData.TriggerRange = Enum.Parse<EActionType>(strList[1]);
            
            var unitCamps = strList[2].Split("|");
            foreach (var unitCamp in unitCamps)
            {
                buffData.TriggerUnitCamps.Add(Enum.Parse<ERelativeCamp>(unitCamp));
            }
            
            var triggerTargets = strList[3].Split("2");
            foreach (var triggerTarget in triggerTargets)
            {
                buffData.TriggerTargets.Add(Enum.Parse<ETriggerTarget>(triggerTarget));
            }
            
            var unitStateEffectTypes = strList[4].Split("|");;
            foreach (var unitStateEffectType in unitStateEffectTypes)
            {
                buffData.UnitStateEffectTypes.Add(Enum.Parse<EUnitStateEffectType>(unitStateEffectType));
            }
            
        }


        public void BuffParse_TacticProp(string[] strList, BuffData buffData)
        {
            var drGridProp = GameEntry.DataTable.GetGridProp(int.Parse(strList[1]));
            var gridPropStrList = drGridProp.GridPropIDs[0].Split("_");
            BuffParse_Normal(gridPropStrList, buffData);
        }

        // public void BuffParse_Pass(string[] strList, BuffData buffData)
        // {
        //     buffData.TriggerRange = Enum.Parse<EActionType>(strList[1]);
        //     
        //     var unitCamps = strList[2].Split("|");
        //     foreach (var unitCamp in unitCamps)
        //     {
        //         buffData.TriggerUnitCamps.Add(Enum.Parse<ERelativeCamp>(unitCamp));
        //     }
        //     
        //     var triggerTargets = strList[3].Split("2");
        //     foreach (var triggerTarget in triggerTargets)
        //     {
        //         buffData.TriggerTargets.Add(Enum.Parse<ETriggerTarget>(triggerTarget));
        //     }
        //     
        //     buffData.BuffValueType = Enum.Parse<EBuffValueType>(strList[4]);
        //
        //     switch (buffData.BuffValueType)
        //     {
        //         case EBuffValueType.Atrb:
        //             buffData.UnitAttribute = Enum.Parse<EUnitAttribute>(strList[5]);
        //             break;
        //         case EBuffValueType.Hero:
        //             buffData.HeroAttribute = Enum.Parse<EHeroAttribute>(strList[5]);
        //             break;
        //         case EBuffValueType.State:
        //             buffData.UnitState = Enum.Parse<EUnitState>(strList[5]);
        //             break;
        //         case EBuffValueType.Card:
        //             buffData.CardTriggerType = Enum.Parse<ECardTriggerType>(strList[5]);
        //             break;
        //         case EBuffValueType.Empty:
        //             break;
        //         default:
        //             break;
        //     }
        // }
        //
        public void BuffParse_SelectUnit(string[] strList, BuffData buffData)
        {
            
            buffData.TriggerRange = Enum.Parse<EActionType>(strList[1]);

            
            var unitCamps = strList[2].Split("|");
            foreach (var unitCamp in unitCamps)
            {
                buffData.TriggerUnitCamps.Add(Enum.Parse<ERelativeCamp>(unitCamp));
            }
            
            var triggerTargets = strList[3].Split("|");
            foreach (var triggerTarget in triggerTargets)
            {
                buffData.TriggerTargets.Add(Enum.Parse<ETriggerTarget>(triggerTarget));
            }

            buffData.BuffValueType = Enum.Parse<EBuffValueType>(strList[4]);
            switch (buffData.BuffValueType)
            {
                case EBuffValueType.Atrb:
                    buffData.UnitAttribute = Enum.Parse<EUnitAttribute>(strList[5]);
                    break;
                case EBuffValueType.HeroAtrb:
                    buffData.HeroAttribute = Enum.Parse<EHeroAttribute>(strList[5]);
                    break;
                case EBuffValueType.State:
                    buffData.UnitState = Enum.Parse<EUnitState>(strList[5]);
                    break;
                case EBuffValueType.Card:
                    buffData.CardTriggerType = Enum.Parse<ECardTriggerType>(strList[5]);
                    break;
                case EBuffValueType.Empty:
                    break;
                default:
                    break;
            }
            
            
            if (strList.Length >= 8)
            {
                buffData.FlyType = Enum.Parse<EFlyType>(strList[6]);
                buffData.FlyRange = Enum.Parse<EActionType>(strList[7]);
            }
        }
        
        public void BuffParse_SelectGrid(string[] strList, BuffData buffData)
        {
            buffData.FlyType = Enum.Parse<EFlyType>(strList[1]);
            buffData.FlyRange = Enum.Parse<EActionType>(strList[2]);
            buffData.TriggerRange = Enum.Parse<EActionType>(strList[3]);
            
            var unitCamps = strList[4].Split("|");
            foreach (var unitCamp in unitCamps)
            {
                buffData.TriggerUnitCamps.Add(Enum.Parse<ERelativeCamp>(unitCamp));
            }
            
            var triggerTargets = strList[5].Split("2");
            foreach (var triggerTarget in triggerTargets)
            {
                buffData.TriggerTargets.Add(Enum.Parse<ETriggerTarget>(triggerTarget));
            }

            buffData.BuffValueType = Enum.Parse<EBuffValueType>(strList[6]);
            switch (buffData.BuffValueType)
            {
                case EBuffValueType.Atrb:
                    buffData.UnitAttribute = Enum.Parse<EUnitAttribute>(strList[7]);
                    break;
                case EBuffValueType.HeroAtrb:
                    buffData.HeroAttribute = Enum.Parse<EHeroAttribute>(strList[7]);
                    break;
                case EBuffValueType.State:
                    buffData.UnitState = Enum.Parse<EUnitState>(strList[7]);
                    break;
                case EBuffValueType.Card:
                    buffData.CardTriggerType = Enum.Parse<ECardTriggerType>(strList[7]);
                    break;
                case EBuffValueType.Empty:
                    break;
                default:
                    break;
            }
        }
        
        public void BuffParse_TacticSelectUnit(string[] strList, BuffData buffData)
        {
            buffData.TriggerRange = Enum.Parse<EActionType>(strList[1]);
            
            var unitCamps = strList[2].Split("|");
            foreach (var unitCamp in unitCamps)
            {
                buffData.TriggerUnitCamps.Add(Enum.Parse<ERelativeCamp>(unitCamp));
            }
            
            var triggerTargets = strList[3].Split("2");
            foreach (var triggerTarget in triggerTargets)
            {
                buffData.TriggerTargets.Add(Enum.Parse<ETriggerTarget>(triggerTarget));
            }

            buffData.BuffValueType = Enum.Parse<EBuffValueType>(strList[4]);

            switch (buffData.BuffValueType)
            {
                case EBuffValueType.Atrb:
                    buffData.UnitAttribute = Enum.Parse<EUnitAttribute>(strList[5]);
                    break;
                case EBuffValueType.HeroAtrb:
                    buffData.HeroAttribute = Enum.Parse<EHeroAttribute>(strList[5]);
                    break;
                case EBuffValueType.State:
                    buffData.UnitState = Enum.Parse<EUnitState>(strList[5]);
                    break;
                case EBuffValueType.Card:
                    buffData.CardTriggerType = Enum.Parse<ECardTriggerType>(strList[5]);
                    break;
                case EBuffValueType.Empty:
                    break;
                default:
                    break;
            }
        }
        
        public void BuffParse_TacticSelectGrid(string[] strList, BuffData buffData)
        {
            
            buffData.TriggerRange = Enum.Parse<EActionType>(strList[1]);
            
            var unitCamps = strList[2].Split("|");
            foreach (var unitCamp in unitCamps)
            {
                buffData.TriggerUnitCamps.Add(Enum.Parse<ERelativeCamp>(unitCamp));
            }
            
            var triggerTargets = strList[3].Split("2");
            foreach (var triggerTarget in triggerTargets)
            {
                buffData.TriggerTargets.Add(Enum.Parse<ETriggerTarget>(triggerTarget));
            }
            
            buffData.BuffValueType = Enum.Parse<EBuffValueType>(strList[4]);

            switch (buffData.BuffValueType)
            {
                case EBuffValueType.Atrb:
                    buffData.UnitAttribute = Enum.Parse<EUnitAttribute>(strList[5]);
                    break;
                case EBuffValueType.HeroAtrb:
                    buffData.HeroAttribute = Enum.Parse<EHeroAttribute>(strList[5]);
                    break;
                case EBuffValueType.State:
                    buffData.UnitState = Enum.Parse<EUnitState>(strList[5]);
                    break;
                case EBuffValueType.Card:
                    buffData.CardTriggerType = Enum.Parse<ECardTriggerType>(strList[5]);
                    break;
                case EBuffValueType.Empty:
                    break;
                default:
                    break;
            }
        }
        
        public void BuffParse_Spec(string buffKey, string[] strList, BuffData buffData)
        {
            var buff = Enum.Parse<EBuffID>(buffKey);
            switch (buff)
            {
                case EBuffID.Spec_MoveUs:
                    buffData.TriggerUnitCamps.Add(ERelativeCamp.Us);
                    break;
                case EBuffID.Spec_AttackUs:
                    buffData.TriggerUnitCamps.Add(ERelativeCamp.Us);
                    break;
            }
            
            
        }
        
        public float GetBuffValue(string value, int effectUnitIdx = -1, int cardIdx = -1, TriggerData preTriggerData = null)
        {
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            var absValue = value;
            var valueTag = 1;
            if (value.Contains("-"))
            {
                absValue = value.Remove(0, 1);
                valueTag = -1;
            }

            
            if (float.TryParse(absValue, out float floatValue))
            {
                return floatValue * valueTag;
            }
            else if(Enum.TryParse(absValue, out EValueType valueType))
            {
                switch (valueType)
                {
                    case EValueType.UnitHP:
                        
                        if (effectUnit != null)
                            return effectUnit.CurHP * valueTag;
                        break;
                    case EValueType.EffectUnitAttack:
                        break;
                    case EValueType.Empty:
                        break;
                    case EValueType.HandCardCount:
                        break;
                    case EValueType.UnitCount:
                        break;
                    case EValueType.UsStaffCount:
                        var usStaffCount = 0;
                        foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
                        {
                            if (kv.Value.UnitCamp == PlayerManager.Instance.PlayerData.UnitCamp &&
                                kv.Value.BattleUnitData.UnitRole == EUnitRole.Staff)
                            {
                                usStaffCount++;
                            }
                        }

                        return usStaffCount;
                        break;
                    case EValueType.EnemyCount:
                        var enemyCount = 0;
                        foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
                        {
                            if (kv.Value.UnitCamp != EUnitCamp.Third &&
                                kv.Value.UnitCamp != PlayerManager.Instance.PlayerData.UnitCamp)

                            {
                                enemyCount++;
                            }
                        }

                        return enemyCount;
                        break;
                    case EValueType.UsBuffCount:
                        break;
                    case EValueType.EnemyDeBuffCount:
                        break;
                    case EValueType.DeBuff:
                        if (effectUnit != null)
                            return effectUnit.BattleUnitData.GetStateCountByEffectType(EUnitStateEffectType.DeBuff) * valueTag;
                        break;
                    case EValueType.Buff:
                        if (effectUnit != null)
                            return effectUnit.BattleUnitData.GetStateCountByEffectType(EUnitStateEffectType.Buff) * valueTag;
                        break;
                    case EValueType.CardEnergy:
                        if (cardIdx != -1)
                        {
                            return BattleCardManager.Instance.GetCardEnergy(cardIdx);
                        }
                        break;
                    case EValueType.SubEnergy:
                        if (cardIdx != -1)
                        {
                            var drCard = CardManager.Instance.GetCardTable(cardIdx);
                            
                            var cardEnergy = drCard.Energy;
                            var actualCardEnergy = BattleCardManager.Instance.GetCardEnergy(cardIdx);
                            var energyDelta = actualCardEnergy - cardEnergy;
                            if (energyDelta < 0)
                                return -energyDelta;

                            return 0;
                        }
                        break;
                    case EValueType.OverflowDmg:
                        if (preTriggerData != null)
                        {
                            var delta = Math.Abs(preTriggerData.Value + preTriggerData.DeltaValue) -
                                        Math.Abs(preTriggerData.ActualValue);
                            return delta > 0 ? delta : 0;
                        }
                        break;
                        
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return 0;
            }

            return 0;
        }

        // public int GetBuffValue(string value)
        // {
        //     int.TryParse(value, out int resValue);
        //     return resValue;
        // }


        public void TransferBuff(Data_BattleUnit actionUnit, Data_BattleUnit effectUnit, TriggerData triggerData)
        {
            foreach (var kv in actionUnit.UnitStateData.UnitStates)
            {
                foreach (var unitStateEffectType in triggerData.UnitStateEffectTypes)
                {
                    if (Constant.Battle.EffectUnitStates[unitStateEffectType].Contains(kv.Key))
                    {
                        effectUnit.ChangeState(kv.Key, kv.Value.Value);
                    }
                }
            }
            actionUnit.RemoveAllState(triggerData.UnitStateEffectTypes);
        }

    }
}