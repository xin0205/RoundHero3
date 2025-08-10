using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;
using Random = System.Random;


namespace RoundHero
{

    public partial class BattleFightManager : Singleton<BattleFightManager>
    {
        public RoundFightData RoundFightData = new();

        private Dictionary<int, List<UnitActionRange>> UnitActionRange = new();

        // private Dictionary<int, Data_BattleSolider> Soliders;
        // private Dictionary<int, Data_BattleMonster> Enemies;

        public Dictionary<int, Data_BattleUnit> BattleUnitDatas = new Dictionary<int, Data_BattleUnit>();
        public Dictionary<int, Data_BattleUnit> BattleUnitDatasByGridPosIdx = new Dictionary<int, Data_BattleUnit>();

        private Dictionary<int, Data_BattleUnit> Roles;
        public EActionProgress ActionProgress;
        public int AcitonUnitIdx;

        private Dictionary<int, Data_GridPropMoveDirect> MoveDirectPropUseDict =
            new Dictionary<int, Data_GridPropMoveDirect>();

        private Dictionary<EBuffTriggerType, List<int>> ActionUnitIDs = new();

        public Dictionary<EGridPropID, string> CacheGridPorpIDStr = new ();

        public BattleFightManager()
        {
            foreach (EBuffTriggerType triggerType in Enum.GetValues(typeof(EBuffTriggerType)))
            {
                ActionUnitIDs.Add(triggerType, new List<int>(10));
            }
        }

        public Random Random;
        private int randomSeed;

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);

        }

        public void Destory()
        {
            AcitonUnitIdx = 0;
            RoundFightData.Clear();
        }

        public Data_Player PlayerData;

        


        

        private static bool ContainsMoveUnitStateData(List<MoveUnitStateData> moveUnitStateDatas, EUnitState unitState,
            bool isBePass)
        {
            if (moveUnitStateDatas == null)
                return false;

            return moveUnitStateDatas.Any(data => data.UnitState == unitState && data.IsBePass == isBePass);
        }

        private List<MoveUnitStateData> MoveTrigger(int moveIdx, Data_BattleUnit passUnit, Data_BattleUnit bePassUnit,
            List<TriggerData> triggerDatas, List<MoveUnitStateData> preMoveUnitStateDatas = null)
        {
            var moveUnitStateDatas = new List<MoveUnitStateData>();

            if (passUnit == null || bePassUnit == null)
                return moveUnitStateDatas;

            var bePassUnitAttackPassUs = moveIdx != 0 && bePassUnit != null
                ? bePassUnit.GetAllStateCount(EUnitState.AtkPassUs)
                : 0;
            var passUnitAttackPassUs =
                moveIdx != 0 && passUnit != null ? passUnit.GetAllStateCount(EUnitState.AtkPassUs) : 0;
            var bePassUnitAttackPassEnemy = moveIdx != 0 && bePassUnit != null
                ? bePassUnit.GetAllStateCount(EUnitState.AtkPassEnemy)
                : 0;
            var passUnitAttackPassEnemy = moveIdx != 0 && passUnit != null
                ? passUnit.GetAllStateCount(EUnitState.AtkPassEnemy)
                : 0;

            if (bePassUnitAttackPassUs > 0 && passUnit.UnitCamp == bePassUnit.UnitCamp)
            {

                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassUs, true) &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                {

                    //passUnit.GetAllStateCount(EUnitState.UnAction) > 0 ||
                    if ((passUnit.GetAllStateCount(EUnitState.UnMove) > 0))
                        return null;

                    var value = -bePassUnitAttackPassUs;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(bePassUnit
                            .UnitCamp);
                    if (battlePlayerData.BattleBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? 1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(bePassUnit.Idx,
                        bePassUnit.Idx, passUnit.Idx,
                        EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = bePassUnit.GridPosIdx;
                    triggerData.UnitStateDetail.UnitState = EUnitState.AtkPassUs;
                    //bePassUnit.RemoveState(EUnitState.AtkPassUs);

                    BattleBuffManager.Instance.PostTrigger(triggerData, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(bePassUnit.Idx, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassUs, true));
                    
                    var subAtkPassUsData = BattleFightManager.Instance.Unit_State(triggerDatas, bePassUnit.Idx,
                        bePassUnit.Idx, bePassUnit.Idx, EUnitState.AtkPassUs, -1,
                        ETriggerDataType.RoleState);
                    BattleBuffManager.Instance.PostTrigger(subAtkPassUsData, triggerDatas);

                }

            }

            if (passUnitAttackPassUs > 0 && passUnit.UnitCamp == bePassUnit.UnitCamp)
            {
                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassUs, false) &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                {
                    var value = -passUnitAttackPassUs;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                            passUnit.UnitCamp);
                    if (battlePlayerData.BattleBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? 1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(passUnit.Idx,
                        passUnit.Idx, bePassUnit.Idx,
                        EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = bePassUnit.GridPosIdx;
                    triggerData.UnitStateDetail.UnitState = EUnitState.AtkPassUs;
                    //passUnit.RemoveState(EUnitState.AtkPassUs);

                    BattleBuffManager.Instance.PostTrigger(triggerData, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(passUnit.Idx, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassUs, false));
                    
                    var subAtkPassUsData = BattleFightManager.Instance.Unit_State(triggerDatas, bePassUnit.Idx,
                        bePassUnit.Idx, passUnit.Idx, EUnitState.AtkPassUs, -1,
                        ETriggerDataType.RoleState);
                    BattleBuffManager.Instance.PostTrigger(subAtkPassUsData, triggerDatas);

                }
            }

            if (bePassUnitAttackPassEnemy > 0 && passUnit.UnitCamp != bePassUnit.UnitCamp)
            {
                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassEnemy, true))
                {
                    var value = -bePassUnitAttackPassEnemy;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(bePassUnit
                            .UnitCamp);
                    if (battlePlayerData != null && battlePlayerData.BattleBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? -1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(bePassUnit.Idx, bePassUnit.Idx,
                        passUnit.Idx, EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = bePassUnit.GridPosIdx;
                    triggerData.UnitStateDetail.UnitState = EUnitState.AtkPassEnemy;
                    //bePassUnit.RemoveState(EUnitState.AtkPassEnemy);

                    BattleBuffManager.Instance.PostTrigger(triggerData, triggerDatas);
    
                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(bePassUnit.Idx, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassEnemy, true));
                    
                    var subAtkPassEnemyData = BattleFightManager.Instance.Unit_State(triggerDatas, bePassUnit.Idx,
                        bePassUnit.Idx, bePassUnit.Idx, EUnitState.AtkPassEnemy, -1,
                        ETriggerDataType.RoleState);
                    BattleBuffManager.Instance.PostTrigger(subAtkPassEnemyData, triggerDatas);

                }
            }

            if (passUnitAttackPassEnemy > 0 && passUnit.UnitCamp != bePassUnit.UnitCamp)
            {
                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassEnemy, false))
                {
                    var value = -passUnitAttackPassEnemy;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                            passUnit.UnitCamp);
                    if (battlePlayerData != null && battlePlayerData.BattleBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? -1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(passUnit.Idx, passUnit.Idx,
                        bePassUnit.Idx, EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = bePassUnit.GridPosIdx;
                    triggerData.UnitStateDetail.UnitState = EUnitState.AtkPassEnemy;
                    //passUnit.RemoveState(EUnitState.AtkPassEnemy);

                    BattleBuffManager.Instance.PostTrigger(triggerData, triggerDatas);
                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(passUnit.Idx, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassEnemy, false));

                    var subAtkPassEnemyData = BattleFightManager.Instance.Unit_State(triggerDatas, passUnit.Idx,
                        passUnit.Idx, passUnit.Idx, EUnitState.AtkPassEnemy, -1,
                        ETriggerDataType.RoleState);
                    BattleBuffManager.Instance.PostTrigger(subAtkPassEnemyData, triggerDatas);
                }
            }

            return moveUnitStateDatas;
        }


        
        public void TriggerUnitData(int triggerUnitID, int effectUnitID, int gridPosIdx, EBuffTriggerType buffTriggerType,
            List<TriggerData> triggerDatas)
        {
            var triggerUnits = GetTriggerUnits(gridPosIdx);

            if (triggerUnits != null)
            {
                foreach (var soliderActionRange in triggerUnits)
                {
                    if (soliderActionRange.ActionUnitID == triggerUnitID)
                        continue;

                    if (soliderActionRange.BuffTriggerType != buffTriggerType)
                        continue;

                    var ownUnit = BattleUnitDatas[soliderActionRange.OwnUnitID];

                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, ownUnit,
                        out List<BuffValue> triggerBuffDatas);

                    if (triggerBuffDatas != null)
                    {
                        foreach (var triggerBuffData in triggerBuffDatas)
                        {
                            BattleBuffManager.Instance.BuffTrigger(buffTriggerType,
                                triggerBuffData.BuffData, triggerBuffData.ValueList, soliderActionRange.OwnUnitID,
                                soliderActionRange.ActionUnitID, effectUnitID,
                                triggerDatas);
                        }

                    }


                }

            }
        }


        
        private void CurHPTriggerData(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            Data_BattleUnit effectUnitData;
            if (triggerData.EffectUnitIdx == BattlePlayerManager.Instance.PlayerData.BattleHero.Idx)
            {
                effectUnitData = BattlePlayerManager.Instance.PlayerData.BattleHero;
            }
            else
            {
                effectUnitData = GetUnitByIdx(triggerData.EffectUnitIdx);
            }
            
            if (effectUnitData == null)
                return;
            
            var actionUnitData = GetUnitByIdx(triggerData.ActionUnitIdx);

            var effectUnitOldHP = effectUnitData.CurHP;

            var triggerValue = triggerData.Value + triggerData.DeltaValue;
            if (triggerValue < 0)
            {
               
                if (effectUnitData.GetStateCount(EUnitState.UnHurt) > 0)
                {
                    triggerData.TriggerResult = ETriggerResult.UnHurt;

                }
                // else if (effectUnitData.GetStateCount(EUnitState.Dodge) > 0)
                // {
                //     effectUnitData.RemoveState(EUnitState.Dodge);
                //     triggerData.TriggerResult = ETriggerResult.Dodge;
                //
                // }
                else
                {
                    if (triggerData.OwnUnitIdx > 0 && triggerData.OwnUnitIdx == triggerData.ActionUnitIdx)
                    {
                        var ownUnit = GetUnitByIdx(triggerData.OwnUnitIdx);

                        if (ownUnit != null && triggerData.UnitStateDetail.UnitState == EUnitState.Empty)
                        {
                            BattleGridPropManager.Instance.TriggerStayPropState(triggerData.EffectUnitGridPosIdx,
                                effectUnitData, EUnitState.HurtAddDmg);
                            
                            BattleGridPropManager.Instance.TriggerStayPropState(triggerData.ActionUnitGridPosIdx,
                                actionUnitData, EUnitState.SubDmg);
                            
                            BattleGridPropManager.Instance.TriggerStayPropState(triggerData.ActionUnitGridPosIdx,
                                actionUnitData, EUnitState.AddDmg);
                            
                            var hurtAddDmgCount = effectUnitData.GetAllStateCount(EUnitState.HurtAddDmg);
                            var addDmgCount = actionUnitData.GetAllStateCount(EUnitState.AddDmg);
                            var subDmgCount = actionUnitData.GetAllStateCount(EUnitState.SubDmg);
                            var subHPAddSelfHPCount = actionUnitData.GetAllStateCount(EUnitState.SubHPAddSelfHP);
                            triggerData.DeltaValue += -addDmgCount;
                            
                            if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                            {
                                triggerData.DeltaValue += subDmgCount;
                            }

                            if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                            {
                                triggerData.DeltaValue += -hurtAddDmgCount;
                            }

                            triggerData.DeltaValue += -actionUnitData.FuneCount(EBuffID.Spec_AddBaseDmg);

                            if (hurtAddDmgCount > 0)
                            {
                                var subHurtAddDmgData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.Idx,
                                    effectUnitData.Idx, effectUnitData.Idx, EUnitState.HurtAddDmg, -1,
                                    ETriggerDataType.RoleState);
                                SimulateTriggerData(subHurtAddDmgData, triggerDatas);
                                triggerDatas.Add(subHurtAddDmgData);
                            }
                            
                            if (addDmgCount > 0)
                            {
                                var subAddDmgCountData = BattleFightManager.Instance.Unit_State(triggerDatas, actionUnitData.Idx,
                                    actionUnitData.Idx, actionUnitData.Idx, EUnitState.AddDmg, -1,
                                    ETriggerDataType.RoleState);
                                SimulateTriggerData(subAddDmgCountData, triggerDatas);
                                triggerDatas.Add(subAddDmgCountData);
                            }

                            if (subHPAddSelfHPCount > 0)
                            {
                                var subHPAddSelfHPData = BattleFightManager.Instance.Unit_State(triggerDatas, actionUnitData.Idx,
                                    actionUnitData.Idx, actionUnitData.Idx, EUnitState.SubHPAddSelfHP, -1,
                                    ETriggerDataType.RoleState);
                                
                                
                                SimulateTriggerData(subHPAddSelfHPData, triggerDatas);
                                triggerDatas.Add(subHPAddSelfHPData);
                                
                                var subHPAddSelfHPTriggerData = BattleFightManager.Instance.BattleRoleAttribute(actionUnitData.Idx,
                                    actionUnitData.Idx, actionUnitData.Idx, EUnitAttribute.HP, subHPAddSelfHPCount,
                                    ETriggerDataSubType.Unit);
                                subHPAddSelfHPTriggerData.ActionUnitGridPosIdx = actionUnitData.GridPosIdx;
                                subHPAddSelfHPTriggerData.EffectUnitGridPosIdx = actionUnitData.GridPosIdx;
                                
                                SimulateTriggerData(subHPAddSelfHPTriggerData, triggerDatas);
                                triggerDatas.Add(subHPAddSelfHPTriggerData);
                            }
                        }

                        if (actionUnitData is Data_BattleSolider solider &&
                            actionUnitData.UnitCamp == BattleManager.Instance.CurUnitCamp)
                        {

                            // var energy0CardAddDamageCount =
                            //     RoundFightData.GamePlayData.BlessCount(EBlessID.Energy0CardAddDamage,
                            //         BattleManager.Instance.CurUnitCamp);
                            // if (energy0CardAddDamageCount > 0)
                            // {
                            //     var energy = BattleCardManager.Instance.GetCardEnergy(solider.CardID);
                            //     if (energy == 0)
                            //     {
                            //         var drEnergy0CardAddDamage =
                            //             GameEntry.DataTable.GetBless(EBlessID.Energy0CardAddDamage);
                            //         triggerData.DeltaValue +=
                            //             energy0CardAddDamageCount * drEnergy0CardAddDamage.Values1[0];
                            //     }
                            // }

                            var eachRoundDoubleDamageCount =
                                RoundFightData.GamePlayData.BlessCount(EBlessID.EachRoundDoubleDamage,
                                    BattleManager.Instance.CurUnitCamp);
                            var drEachRoundDoubleDamage = GameEntry.DataTable.GetBless(EBlessID.EachRoundDoubleDamage);
                            if (eachRoundDoubleDamageCount > 0 &&
                                !RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(solider.UnitCamp)
                                    .RoundIsAttack &&
                                RoundFightData.GamePlayData.BattleData.Round > 0 &&
                                RoundFightData.GamePlayData.BattleData.Round % BattleBuffManager.Instance.GetBuffValue(drEachRoundDoubleDamage.Values0[0]) == 0)
                            {

                                triggerData.DeltaValue +=
                                    eachRoundDoubleDamageCount * BattleBuffManager.Instance.GetBuffValue(drEachRoundDoubleDamage.Values0[1]);
                            }

                            RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(solider.UnitCamp).RoundIsAttack =
                                true;


                        }


                        if (triggerData.Value + triggerData.DeltaValue > 0)
                        {
                            triggerData.DeltaValue = -triggerData.Value;
                        }
                        
                        triggerValue = triggerData.Value + triggerData.DeltaValue;
                    }


                    var hpDelta = (int) triggerValue;
                    
                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, effectUnitData,
                        out List<BuffValue> effectTriggerBuffDatas);
                    if (effectTriggerBuffDatas != null)
                    {
                        foreach (var triggerBuffData in effectTriggerBuffDatas)
                        {
                            // if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_UnHurt &&
                            //     (RoundFightData.GamePlayData.BattleData.Round + 1) % triggerBuffData.ValueList[0] ==
                            //     0 &&
                            //     RoundFightData.GamePlayData.BattleData.Round > 0)
                            // {
                            //     triggerData.DeltaValue = -triggerData.Value;
                            // }
                            // else if (triggerBuffData.DrBuff.BuffID == EBuffID.UnHurtDamage &&
                            //          hpDelta == triggerBuffData.ValueList[0])
                            // {
                            //     triggerData.DeltaValue = -triggerData.Value;
                            // }

                        }
                    }

                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, actionUnitData,
                        out List<BuffValue> actionTriggerBuffDatas);

                    if (actionTriggerBuffDatas != null)
                    {
                        foreach (var triggerBuffData in actionTriggerBuffDatas)
                        {
                            // if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_DoubleDamage &&
                            //     (RoundFightData.GamePlayData.BattleData.Round + 1) % triggerBuffData.ValueList[0] ==
                            //     triggerBuffData.ValueList[1] && hpDelta < 0)
                            // {
                            //     triggerData.DeltaValue = hpDelta + triggerData.DeltaValue;
                            // }
                        }
                    }
                    
                    hpDelta = (int) (triggerData.Value + triggerData.DeltaValue);
                    var counterAtkCount = effectUnitData.GetAllStateCount(EUnitState.CounterAtk);
                    if (counterAtkCount > 0)
                    {
                        var counterValue = -counterAtkCount;
                        var battlePlayerData =
                            BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                                effectUnitData.UnitCamp);
                        // if (battlePlayerData.RoundBuffs.Contains(EBuffID.RoundCounterAttackAddDamage))
                        // {
                        //     counterValue += -1;
                        // }
                        
                        var subCounterAttackTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.Idx,
                            effectUnitData.Idx, effectUnitData.Idx, EUnitState.CounterAtk, -counterAtkCount,
                            ETriggerDataType.RoleState);
                        //subCounterAttackTriggerData.ActionUnitGridPosIdx
                        SimulateTriggerData(subCounterAttackTriggerData, triggerDatas);
                        triggerDatas.Add(subCounterAttackTriggerData);

                        var counterAttackTriggerData = BattleFightManager.Instance.BattleRoleAttribute(effectUnitData.Idx,
                            effectUnitData.Idx, triggerData.ActionUnitIdx, EUnitAttribute.HP, counterValue,
                            ETriggerDataSubType.Unit);
                        counterAttackTriggerData.ActionUnitGridPosIdx = effectUnitData.GridPosIdx;
                        counterAttackTriggerData.EffectUnitGridPosIdx = actionUnitData.GridPosIdx;
                        SimulateTriggerData(counterAttackTriggerData, triggerDatas);
                        triggerDatas.Add(counterAttackTriggerData);

                        
                    }


                    if (actionUnitData != null && actionUnitData.GetStateCount(EUnitState.RecoverHP) > 0)
                    {
                        var recoverHPValue = -hpDelta;
                        var recoverHPTriggerData = BattleFightManager.Instance.BattleRoleAttribute(actionUnitData.Idx,
                            actionUnitData.Idx, actionUnitData.Idx, EUnitAttribute.HP, recoverHPValue,
                            ETriggerDataSubType.Unit);
                        SimulateTriggerData(recoverHPTriggerData, triggerDatas);
                        triggerDatas.Add(recoverHPTriggerData);
                    }

                    // if (effectUnitData is Data_BattleSolider)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
                    // {
                    //     var oldSoliderHP = effectUnitData.CurHP;
                    //     FightManager.Instance.ChangeHP(effectUnitData, hpDelta);
                    //     var newSoliderHP = effectUnitData.CurHP;
                    //     var deltaSoliderHP = newSoliderHP - oldSoliderHP;
                    //     if (deltaSoliderHP < 0)
                    //     {
                    //         FightManager.Instance.ChangeHP(RoundFightData.GamePlayData.BattleHero, -deltaSoliderHP);
                    //     }
                    // }
                    // else
                    // {
                    //     FightManager.Instance.ChangeHP(effectUnitData, hpDelta);
                    // }

                    if ( actionUnitData!= null  && actionUnitData.GetStateCount(EUnitState.DoubleDmg) > 0 && !GameUtility.ContainRoundState(
                        GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    {
                        triggerValue *= 2;
                    }

                    triggerValue = BattleFightManager.Instance.ChangeHP(effectUnitData, triggerValue, EHPChangeType.Unit, true,
                        triggerData.ChangeHPInstantly);
                    
                    
                    
                }

                // if (actionUnitData.GetStateCount(EUnitState.HurtSubDamage) > 0)
                // {
                //     var hurtSubDamageTriggerData = FightManager.Instance.Unit_State(actionUnitData.ID,
                //         actionUnitData.ID, actionUnitData.ID, EUnitState.HurtSubDamage, 1, ETriggerDataType.RoleState);
                //     SimulateTriggerData(hurtSubDamageTriggerData, triggerDatas);
                //     triggerDatas.Add(hurtSubDamageTriggerData);
                //
                // }

                //受击增加强化
                if (effectUnitData.GetStateCount(EUnitState.AddDmg) > 0)
                {
                    
                    // var addDamageTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.Idx,
                    //     effectUnitData.Idx, effectUnitData.Idx, EUnitState.AddDmg, 1, ETriggerDataType.RoleState);
                    // SimulateTriggerData(addDamageTriggerData, triggerDatas);
                    // triggerDatas.Add(addDamageTriggerData);
                }

                // if (actionUnitData.GetStateCount(EUnitState.HurtAddDamage) > 0)
                // {
                //     var hurtAddDamageTriggerData = FightManager.Instance.Unit_State(actionUnitData.ID,
                //         actionUnitData.ID, actionUnitData.ID, EUnitState.HurtAddDamage, -1, ETriggerDataType.RoleState);
                //     SimulateTriggerData(hurtAddDamageTriggerData, triggerDatas);
                //     triggerDatas.Add(hurtAddDamageTriggerData);
                // }



                if (actionUnitData != null && actionUnitData.GetStateCount(EUnitState.SubDmg) > 0)
                {
                    var subDamageTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, actionUnitData.Idx,
                        actionUnitData.Idx, actionUnitData.Idx, EUnitState.SubDmg, -1, ETriggerDataType.RoleState);
                    SimulateTriggerData(subDamageTriggerData, triggerDatas);
                    triggerDatas.Add(subDamageTriggerData);
                }

                // if (actionUnitData.GetStateCount(EUnitState.SubDamage) > 0)
                // {
                //     var subDamageTriggerData = FightManager.Instance.Unit_State(actionUnitData.ID,
                //         actionUnitData.ID, actionUnitData.ID, EUnitState.SubDamage, -1, ETriggerDataType.RoleState);
                //     SimulateTriggerData(subDamageTriggerData, triggerDatas);
                //     triggerDatas.Add(subDamageTriggerData);
                // }
            }
            else
            {
                if (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.HP1UnRecover) &&
                      effectUnitData.CurHP == 1 && effectUnitData.UnitCamp == EUnitCamp.Player1)
                {
                    triggerData.Value = 0;
                    triggerData.DeltaValue = 0;
                }
                
                if ( actionUnitData!= null  && actionUnitData.GetStateCount(EUnitState.DoubleDmg) > 0 && !GameUtility.ContainRoundState(
                    GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
                {
                    triggerValue *= 2;
                }
                
                BattleFightManager.Instance.ChangeHP(effectUnitData, triggerValue,
                    EHPChangeType.Unit, true, triggerData.ChangeHPInstantly);
                
            }


            if (triggerData.EffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx)
            {
                triggerData.ActualValue = triggerValue;     
            }
            else if (triggerValue < 0)
            {
                if (Mathf.Abs(triggerValue) > effectUnitOldHP)
                {
                    triggerData.ActualValue = -effectUnitOldHP;
                }
                else
                {
                    triggerData.ActualValue = triggerValue;
                }
            }
            else
            {
                
                triggerData.ActualValue = effectUnitData.CurHP - effectUnitOldHP;
            }
            
            if (!effectUnitData.Exist())
            {
                BattleCurseManager.Instance.CacheUnitDeadRecoverLessHPUnit(effectUnitOldHP, effectUnitData.CurHP,
                    triggerDatas);
                DeadTrigger(triggerData, triggerDatas);
                KillTrigger(triggerData, triggerDatas);
                CacheLinks();

            }
            
        }

        

        public List<UnitActionRange> GetTriggerUnits(int gridPosIdx)
        {
            if (!UnitActionRange.ContainsKey(gridPosIdx))
                return null;

            // if (!UnitActionRange[gridPosIdx].ContainsKey(triggerType))
            //     return null;

            return UnitActionRange[gridPosIdx];

        }


        public void RefreshUnitGridPosIdx()
        {
            BattleUnitDatasByGridPosIdx.Clear();
            
            foreach (var kv in BattleUnitDatas)
            {
                if (!BattleUnitDatasByGridPosIdx.ContainsKey(kv.Value.GridPosIdx))
                {
                    BattleUnitDatasByGridPosIdx.Add(kv.Value.GridPosIdx, kv.Value);
                }

            }
        }


        public TriggerData BattleRoleAttribute(int ownSoliderIdx, int actionSoliderIdx, int effectUnitIdx,
            EUnitAttribute attribute, float attributeValue, ETriggerDataSubType triggerDataSubType)
        {
            var triggerData = new TriggerData();
            triggerData.TriggerDataType = ETriggerDataType.RoleAttribute;
            triggerData.TriggerDataSubType = triggerDataSubType;
            triggerData.OwnUnitIdx = ownSoliderIdx;
            triggerData.ActionUnitIdx = actionSoliderIdx;
            triggerData.EffectUnitIdx = effectUnitIdx;
            triggerData.BattleUnitAttribute = attribute;
            triggerData.Value = attributeValue;

            return triggerData;
        }



        public TriggerData Unit_HeroAttribute(int triggerSoliderID, int actionSoliderID, int effectUnitID,
            EHeroAttribute attribute, float attributeValue)
        {
            var cardTriggerData = new TriggerData();
            cardTriggerData.TriggerDataType = ETriggerDataType.HeroAtrb;

            cardTriggerData.OwnUnitIdx = triggerSoliderID;
            cardTriggerData.ActionUnitIdx = actionSoliderID;
            cardTriggerData.EffectUnitIdx = effectUnitID;
            cardTriggerData.HeroAttribute = attribute;
            cardTriggerData.Value = attributeValue;

            return cardTriggerData;
        }

        public TriggerData Unit_State(List<TriggerData> triggerDatas, int triggerSoliderID, int actionSoliderID, int effectUnitID,
            EUnitState unitState, float attributeValue, ETriggerDataType triggerDataType)
        {
            var effectUnit = GameUtility.GetUnitDataByIdx(effectUnitID);
            if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(unitState))
            {
                if (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.AddDebuffRecoverHP))
                {
                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(effectUnit.Idx, effectUnit.Idx,
                        effectUnit.Idx, EUnitAttribute.HP, 1, ETriggerDataSubType.Curse);
                    triggerDatas.Add(triggerData);
                }
            }
            
            return Unit_State(triggerSoliderID, actionSoliderID, effectUnitID,
                unitState, attributeValue, triggerDataType);
        }

        private TriggerData Unit_State(int triggerSoliderID, int actionSoliderID, int effectUnitID,
            EUnitState unitState, float value, ETriggerDataType triggerDataType)
        {
            var cardTriggerData = new TriggerData();
            var unit = GetUnitByIdx(effectUnitID);
            if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(unitState))
            {
                if (unit.GetStateCount(EUnitState.DeBuffUnEffect) > 0)
                {
                    cardTriggerData.TriggerDataType = ETriggerDataType.RoleState;
                    cardTriggerData.OwnUnitIdx = effectUnitID;
                    cardTriggerData.ActionUnitIdx = effectUnitID;
                    cardTriggerData.EffectUnitIdx = effectUnitID;
                    cardTriggerData.UnitStateDetail.UnitState = EUnitState.DeBuffUnEffect;
                    cardTriggerData.Value = -1;
                    cardTriggerData.ActualValue = cardTriggerData.Value;
                    return cardTriggerData;
                }
                else if (unit.GetRoundStateCount(EUnitState.DeBuffUnEffect) > 0)
                {
                    cardTriggerData.TriggerDataType = ETriggerDataType.Empty;
                    return cardTriggerData;
                }
                
            }
            else if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Buff].Contains(unitState))
            {
                if (unit.GetAllStateCount(EUnitState.BuffAddMore) > 0)
                {
                    value += 1;
                }
                
            }
            
            cardTriggerData.TriggerDataType = triggerDataType;
            cardTriggerData.OwnUnitIdx = triggerSoliderID;
            cardTriggerData.ActionUnitIdx = actionSoliderID;
            cardTriggerData.EffectUnitIdx = effectUnitID;
            cardTriggerData.UnitStateDetail.UnitState = unitState;
            cardTriggerData.UnitStateDetail.Value = (int)value;
            cardTriggerData.Value = value;
            cardTriggerData.ActualValue = cardTriggerData.Value;
            return cardTriggerData;
        }
        
        public TriggerData  Hero_Card(int triggerSoliderID, int actionSoliderID, int effectUnitID, float value,
            ECardTriggerType cardTriggerType)
        {

            var cardTriggerData = new TriggerData();
            cardTriggerData.TriggerDataType = ETriggerDataType.Card;
            cardTriggerData.CardTriggerType = cardTriggerType;
            cardTriggerData.OwnUnitIdx = triggerSoliderID;
            cardTriggerData.ActionUnitIdx = actionSoliderID;
            cardTriggerData.EffectUnitIdx = effectUnitID;
        
            cardTriggerData.Value = value;
        
            return cardTriggerData;
        
        }
        
        public async Task TriggerAction(TriggerData triggerData)
        {
            // var ownUnitEntity = BattleUnitManager.Instance.GetUnitByID(triggerData.OwnUnitID);
            // if (ownUnitEntity == null)
            //     return;
            //
            
            var effectUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            // if (effectUnitEntity == null && triggerData.TriggerDataType != ETriggerDataType.Card)
            //     return;
            
            var actionUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
            
            
            if (triggerData.TriggerResult == ETriggerResult.UnHurt)
            {
                effectUnitEntity?.Dodge();
                return;
            }


            if (triggerData.BuffTriggerType == EBuffTriggerType.TacticClearBuff)
            {
                BattleBuffManager.Instance.Spec_ClearBuff(triggerData);
            }
            // if (triggerData.TriggerResult == ETriggerResult.Dodge)
            // {
            //     effectUnitEntity.BattleUnit.RemoveState(EUnitState.Dodge);
            //     effectUnitEntity.Dodge();
            //     return;
            // }
            
            var triggerValue = (int) (triggerData.Value + triggerData.DeltaValue);
            
            

            // if (effectUnitEntity.BattleUnit.AddHeroHP != 0)
            // {
            //     var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(effectUnitEntity.UnitCamp);
            //     playerData.BattleHero.CacheHPDelta += effectUnitEntity.BattleUnit.AddHeroHP;
            //     effectUnitEntity.BattleUnit.AddHeroHP = 0;
            //     return;
            // }
                
            
            switch (triggerData.TriggerDataType)
            {
                case ETriggerDataType.HeroAtrb:
                    var battleHeroEntity = effectUnitEntity as BattleHeroEntity;
                    switch (triggerData.HeroAttribute)
                    {
                        case EHeroAttribute.HP:
                            HeroManager.Instance.BattleHeroData.CacheHPDelta += triggerValue;

                            // var unitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);
                            // var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                            //     AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), unitPos);
                            //
                            //
                            // var uiCorePos = AreaController.Instance.UICore.transform.localPosition;
                            // uiCorePos.y -= 25f;
                            
                            var endValue = BlessManager.Instance.AddCurHPByAttackDamage()
                                ? (int)(triggerData.Value + triggerData.DeltaValue)
                                : (int)triggerData.ActualValue;
                            ////AQA
                            // await GameEntry.Entity.ShowBattleMoveValueEntityAsync(uiLocalPoint,
                            //     uiCorePos,
                            //     (int)triggerData.ActualValue, (int)endValue, -1, false, false);
                            
                            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                            var moveParams = new MoveParams()
                            {
                                FollowGO = actionUnit.gameObject,
                                DeltaPos = new Vector2(0, 25f),
                                IsUIGO = false,
                            };
            
                            var targetMoveParams = new MoveParams()
                            {
                                FollowGO = AreaController.Instance.UICore,
                                DeltaPos = new Vector2(0, -25f),
                                IsUIGO = true,
                            };

                            GameEntry.Entity.ShowBattleMoveValueEntityAsync((int)triggerData.ActualValue, (int)endValue, -1, false, false,
                                moveParams,
                                targetMoveParams);
                            
                            break;
                        // case EHeroAttribute.CurHP:
                        //     var hurt = (int) (triggerData.Value + triggerData.DeltaValue);
                        //     if (hurt < 0)
                        //     {
                        //         effectUnitEntity.LastCurHPDelta();
                        //     } 
                        //     else if (hurt > 0)
                        //     {
                        //         effectUnitEntity.Recover();
                        //     }
                        //     effectUnitEntity.ChangeCurHP(hurt);
                        //     break;

                        // case EHeroAttribute.CurEnergy:
                        //     battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.SetAttribute(
                        //         EHeroAttribute.CurEnergy,
                        //         +battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.GetAttribute(
                        //             EHeroAttribute.CurEnergy) + triggerData.Value);
                        //     break;
                        case EHeroAttribute.CurHeart:
                            break;
                        case EHeroAttribute.MaxHeart:
                            break;

                        case EHeroAttribute.MaxHP:
                            break;
                        
                        case EHeroAttribute.Coin:
                            battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.SetAttribute(
                                EHeroAttribute.Coin,
                                +battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.GetAttribute(
                                    EHeroAttribute.Coin) + triggerData.Value);
                            break;
                        case EHeroAttribute.Damage:
                            battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.SetAttribute(
                                EHeroAttribute.Damage,
                                +battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.GetAttribute(
                                    EHeroAttribute.Damage) + triggerData.Value);
                            break;
                        
                        
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.RoleAttribute:
                    switch (triggerData.BattleUnitAttribute)
                    {
                        case EUnitAttribute.HP:
                            
                            var hpDelta = effectUnitEntity.ChangeCurHP(triggerValue, true, true, triggerData.ChangeHPInstantly);
                            
                            if (triggerData.BattleUnitAttribute == EUnitAttribute.HP &&
                                !triggerData.ChangeHPInstantly && hpDelta != 0)
                            {
                                //effectUnitEntity.BattleUnit.CacheHPDelta += hpDelta;
                                //effectUnitEntity.BattleUnitData.AddHeroHP += hpDelta;
                                HeroManager.Instance.BattleHeroData.CacheHPDelta += hpDelta;
                                effectUnitEntity.Hurt();
                                //HeroManager.Instance.HeroEntity.AddHurts(hpDelta);
                                return;
                            }
                            
                            if (triggerData.HeroHPDelta && hpDelta != 0)
                            {
                                effectUnitEntity.BattleUnitData.AddHeroHP += hpDelta;
                                HeroManager.Instance.BattleHeroData.CacheHPDelta += -hpDelta;
                                //HeroManager.Instance.BattleHeroData.CacheHPDelta += -hpDelta;
                                triggerData.HeroHPDelta = false;
                                // var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnitEntity.UnitCamp);
                                //
                                // if (heroEntity != null)
                                // {
                                //     heroEntity.BattleUnit.CacheHPDelta += -hpDelta;
                                //     triggerData.HeroHPDelta = false;
                                //
                                // }

                            }

                            
                            
                            if (triggerValue < 0)
                            {
                                
                                effectUnitEntity.BattleUnitData.HurtTimes += 1;
                                
                                if (triggerData.UnitStateDetail.UnitState == EUnitState.HurtRoundStart)
                                {
                                    BattleEffectManager.Instance.ShowHurtRoundStartEffect(effectUnitEntity.Position);
                                }
                                
                                effectUnitEntity.Hurt();
                            } 
                            else if (triggerValue > 0)
                            {
                                effectUnitEntity.Recover();
                            }

                            // if (triggerData.UnitStateDetail.UnitState == EUnitState.HurtEachMove || triggerData.UnitStateDetail.UnitState == EUnitState.HurtRoundStart)
                            // {
                            //     effectUnitEntity.BattleUnitData.RemoveState(triggerData.UnitStateDetail.UnitState);
                            // }
                            
                            if (actionUnitEntity != null)
                            {
                                // if (triggerData.UnitStateDetail.UnitState == EUnitState.AtkPassUs ||
                                //     triggerData.UnitStateDetail.UnitState == EUnitState.AtkPassEnemy &&
                                //     actionUnitEntity.BattleUnitData.GetStateCount(triggerData.UnitStateDetail.UnitState) > 0)
                                // {
                                //     actionUnitEntity.BattleUnitData.RemoveState(triggerData.UnitStateDetail.UnitState);
                                // }
                            }
                            
                            break;
                        case EUnitAttribute.MaxHP:
                            var recover = (int) (triggerData.Value + triggerData.DeltaValue);
                            effectUnitEntity.BattleUnitData.BaseMaxHP += recover;
                            effectUnitEntity.ChangeCurHP(recover, true, true, true);
                            break;
                        case EUnitAttribute.Empty:
                            break;
                        case EUnitAttribute.BaseDamage:
                            var damage = (int) (triggerData.Value + triggerData.DeltaValue);
                            effectUnitEntity.BattleUnitData.BaseDamage += damage;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.RoleState:

                    if (triggerData.UnitStateDetail.Value > 0)
                    {
                        if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(triggerData.UnitStateDetail.UnitState))
                        {
                            effectUnitEntity.Hurt();    
                        }
                        if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Buff].Contains(triggerData.UnitStateDetail.UnitState))
                        {
                            effectUnitEntity.Recover();    
                        }
                    }
                    
                    if (triggerData.UnitStateDetail.UnitState == EUnitState.HurtRoundStart)
                    {
                        BattleEffectManager.Instance.ShowHurtRoundStartEffect(effectUnitEntity.Position, effectUnitEntity.transform);
                        effectUnitEntity.AnimtionChangeUnitState(triggerData.UnitStateDetail.UnitState, 1,
                            effectUnitEntity,  -1, false);
                    }

                    effectUnitEntity.BattleUnitData.ChangeState(triggerData.UnitStateDetail.UnitState, triggerValue);
                    break;
                case ETriggerDataType.RoundRoleState:
                    // if (actionUnitEntity != null && actionUnitEntity.ID != effectUnitEntity.ID)
                    // {
                    //     actionUnitEntity.Attack();
                    // }
                    effectUnitEntity.Hurt();
                    effectUnitEntity.BattleUnitData.ChangeRoundState(triggerData.UnitStateDetail.UnitState, (int)(triggerData.Value + triggerData.DeltaValue));
                    break;
                case ETriggerDataType.Card:
                    
                    switch (triggerData.CardTriggerType)
                    {
     
                        case ECardTriggerType.AcquireCard:
                            BattleCardManager.Instance.AcquireCards((int)(triggerData.Value + triggerData.DeltaValue));
                            break;
                        case ECardTriggerType.ToHand:
                            
                            if (actionUnitEntity is BattleSoliderEntity ToHand_solider)
                            {
                                BattleCardManager.Instance.AnimationToHandCards(ToHand_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                            }
                            
                            break;
                        case ECardTriggerType.ToConsume:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardEntity = BattleCardManager.Instance.GetCardEntity(triggerData.TriggerCardIdx);
                                cardEntity.BattleCardEntityData.CardData.CardDestination = ECardDestination.Consume;
                                
                            }
                            else if (actionUnitEntity is BattleSoliderEntity ToConsume_solider)
                            {
                                BattleCardManager.Instance.AnimationToConsumeCards(ToConsume_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                            }
                            break;
                        case ECardTriggerType.ToStandBy:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardEntity = BattleCardManager.Instance.GetCardEntity(triggerData.TriggerCardIdx);
                                cardEntity.BattleCardEntityData.CardData.CardDestination = ECardDestination.StandBy;
                                
                            }
                            else if (actionUnitEntity is BattleSoliderEntity ToStandBy_solider)
                            {
                                BattleCardManager.Instance.ToStandByCards(ToStandBy_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                            }
                             
                            
                            break;
                        
                        case ECardTriggerType.ConsumeCard:
                            
                            BattleCardManager.Instance.ConsumeCardForms();
                            break;
                        
                        case ECardTriggerType.AddSpecificCard:
                            var newCardID = BattleCardManager.Instance.AddTempNewCard((int)triggerData.Value);
                            BattleCardManager.Instance.NewCardToHand(newCardID);
                            break;
                        case ECardTriggerType.ConsumeSpecificCard:
                            //BattleCardManager.Instance.ConsumeCard((int)triggerData.Value);
                            break;
                        case ECardTriggerType.StandByToPass:
                            BattleCardManager.Instance.RandomStandByToPass();
                            break;
                        case ECardTriggerType.Empty:
                            break;
                        case ECardTriggerType.ConsumeToHand:
                            BattleCardManager.Instance.AnimationConsumeToHand();
                            break;
                        case ECardTriggerType.CardEnergy:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardEntity = CardManager.Instance.GetCard(triggerData.TriggerCardIdx);
                                cardEntity.EnergyDelta +=
                                    (int)(triggerData.Value + triggerData.DeltaValue);

                            }
                            else if (actionUnitEntity is BattleSoliderEntity CardEnergy_solider)
                            {
                                var cardEntity = CardManager.Instance.GetCard(CardEnergy_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                                cardEntity.EnergyDelta +=
                                    (int)(triggerData.Value + triggerData.DeltaValue);
                            }
                            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                            break;
                        case ECardTriggerType.MaxHP:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardEntity = CardManager.Instance.GetCard(triggerData.TriggerCardIdx);
                                cardEntity.MaxHPDelta +=
                                    (int)(triggerData.Value + triggerData.DeltaValue);

                            }
                            else if (actionUnitEntity is BattleSoliderEntity MaxHP_solider)
                            {
                                var cardData = CardManager.Instance.GetCard(MaxHP_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                                cardData.MaxHPDelta +=
                                    (int)(triggerData.Value + triggerData.DeltaValue);
                            }
                            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.Link:
                    effectUnitEntity.BattleUnitData.BattleLinkIDs.Add(triggerData.LinkID);
                    break;
                case ETriggerDataType.RemoveUnit:
                    effectUnitEntity.BattleUnitData.RemoveAllState();
                    effectUnitEntity.ChangeCurHP(triggerValue, true, true, triggerData.ChangeHPInstantly);
                    if (triggerValue < 0)
                    {
                        effectUnitEntity.BattleUnitData.HurtTimes += 1;
                        effectUnitEntity.Hurt();
                    }
                    break;
                case ETriggerDataType.RoundBuff:
                    var battlePlayerData = GamePlayManager.Instance.GamePlayData.BattleData.GetBattlePlayerData(effectUnitEntity.UnitCamp);
                    //battlePlayerData.RoundBuffs.Add(triggerData.BuffID);
                    break;
                case ETriggerDataType.ClearBuff:
                    effectUnitEntity.BattleUnitData.RemoveAllState(triggerData.UnitStateEffectTypes);
                    break;
                case ETriggerDataType.TransferBuff:
                    
                    BattleBuffManager.Instance.TransferBuff(actionUnitEntity.BattleUnitData, effectUnitEntity.BattleUnitData, triggerData);
                    break;
                default:
                    break;
            }

            // if (triggerData.HeroHPDelta)
            // {
            //     // var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnitEntity.UnitCamp);
            //     //
            //     // if (heroEntity != null)
            //     // {
            //     //     heroEntity.BattleUnit.CacheHPDelta += ;
            //     //     triggerData.HeroHPDelta = false;
            //     //
            //     //
            //     // }
            //     
            //     // var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(effectUnitEntity.UnitCamp);
            //     // if (playerData != null && playerData.BattleHero != null)
            //     // {
            //     //     playerData.BattleHero.ChangeHP(triggerData.HeroHPDelta);
            //     //     triggerData.HeroHPDelta = 0;
            //     //
            //     //
            //     // }
            // }

        }
        
        
        public void RoundStartBuffTrigger()
        {
            if(ActionProgress != EActionProgress.RoundStartBuff)
                return;
            
            var buffDataKeys = RoundFightData.RoundStartBuffDatas.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= buffDataKeys.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                

                if (RoundFightData.RoundStartBuffDatas.ContainsKey(buffDataKeys[AcitonUnitIdx]))
                {
                    break;
                }
                else
                {
                    AcitonUnitIdx++;
                }
            }


            if(ActionProgress != EActionProgress.RoundStartBuff)
                return;
            
            var actionData = RoundFightData.RoundStartBuffDatas[buffDataKeys[AcitonUnitIdx]];
            
            var isAttack = false;
            foreach (var trigger in actionData.TriggerDatas)
            {
                foreach (var triggerData in trigger.Value)
                {
                    isAttack = true;
                    TriggerAction(triggerData);
                }
            }

            if (isAttack)
            {

                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            }

            AcitonUnitIdx++;

            GameUtility.DelayExcute(isAttack ? 1f : 0.5f, () =>
            {
                BattleManager.Instance.ContinueAction();
            });

            
        }
        
        public void RoundStartUnitTrigger()
        {
            UnitAttack(RoundFightData.RoundStartUnitDatas, EActionProgress.RoundStartUnit);
        }
        
        public void RoundEndTrigger()
        {
            if(ActionProgress != EActionProgress.RoundEnd)
                return;
            
            foreach (var kv in RoundFightData.RoundEndDatas)
            {
                foreach (var triggerDatas in kv.Value.TriggerDatas.Values)
                {
                    foreach (var triggerData in triggerDatas)
                    {
                        TriggerAction(triggerData);
                    }
                    
                }
                
                
            }

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());

 
            GameUtility.DelayExcute(1f, () =>
            {
                AcitonUnitIdx = 0;
                BattleManager.Instance.NextAction();
                BattleManager.Instance.ContinueAction();
            });
        }


        public void UnitAttack(Dictionary<int, ActionData> unitAttackDatas, EActionProgress actionProgress)
        {
            //var unitKeys = BattleUnitDatas.Keys.ToList();
            
            var unitKeys = unitAttackDatas.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= unitAttackDatas.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                

                if (unitAttackDatas.ContainsKey(unitKeys[AcitonUnitIdx]))
                {
                    break;
                }
                else
                {
                    AcitonUnitIdx++;
                }
            }


            if(ActionProgress != actionProgress)
                return;
            
            var actionData = unitAttackDatas[unitKeys[AcitonUnitIdx]];
            
            var isAttack = false;
            foreach (var trigger in actionData.TriggerDatas)
            {
                foreach (var triggerData in trigger.Value)
                {
                    isAttack = true;
                    BattleBulletManager.Instance.AddTriggerData(triggerData);
                    //TriggerAction(triggerData);
                }
            }

            var time = 0.1f;
            
            var moveTime = 0f;
            var maxMoveTime = 0f;
            
            BattleBulletManager.Instance.AddMoveActionData(unitKeys[AcitonUnitIdx], actionData.MoveData);
            foreach (var kv in actionData.MoveData.MoveUnitDatas)
            {
                var effectUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(kv.Value.UnitIdx);
                moveTime = effectUnitEntity.GetMoveTime(kv.Value.UnitActionState, kv.Value.MoveActionData);
                
                // if (kv.Value.UnitActionState == EUnitActionState.Fly)
                // {
                //     moveTime = BattleUnitManager.Instance.BattleUnitEntities[kv.Value.UnitID].Fly(kv.Value.MoveActionData);
                // }
                // else if (kv.Value.UnitActionState == EUnitActionState.Run)
                // {
                //     moveTime = BattleUnitManager.Instance.BattleUnitEntities[kv.Value.UnitID].Run(kv.Value.MoveActionData);
                // }

                if (moveTime > maxMoveTime)
                {
                    maxMoveTime = moveTime;
                }
            }
            //
            if (isAttack || actionData.MoveData.MoveUnitDatas.Count > 0 || actionProgress == EActionProgress.SoliderActiveAttack)
            {
                time += 1f;
                
                var unit = GetUnitByIdx(unitKeys[AcitonUnitIdx]);
                unit.AttackInRound = true;
                BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Attack(actionData);
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());

            }
            
            

            time += maxMoveTime;

            AcitonUnitIdx++;
            

            GameUtility.DelayExcute(time, () =>
            {
                BattleManager.Instance.ContinueAction();
            });
        }

        

        

        public void SoliderAttack()
        {
            UnitAttack(RoundFightData.SoliderAttackDatas, EActionProgress.SoliderAttack);
            
        }
        
        public void SoliderActiveAttack()
        {
            BattleFightManager.Instance.ActionProgress = EActionProgress.SoliderActiveAttack;
            UnitAttack(RoundFightData.SoliderActiveAttackDatas, EActionProgress.SoliderActiveAttack);
            
        }
        
        public void SoliderAutoAttack()
        {
            BattleFightManager.Instance.ActionProgress = EActionProgress.SoliderAutoAttack;
            UnitAttack(RoundFightData.SoliderActiveAttackDatas, EActionProgress.SoliderAutoAttack);
            
        }
        
        public void ThirdUnitAttack()
        {
            UnitAttack(RoundFightData.ThirdUnitAttackDatas, EActionProgress.ThirdUnitAttack);

        }
        
        public void EnemyMove()
        {
            UnitMove(RoundFightData.EnemyMovePaths, RoundFightData.EnemyMoveDatas, RoundFightData.EnemyAttackDatas,
                EActionProgress.EnemyMove);
            
            
        }
        
        


        private void UnitMove(Dictionary<int, List<int>> unitMovePaths, Dictionary<int, MoveActionData> unitMoveDatas, Dictionary<int, ActionData> unitAttackDatas, EActionProgress actionProgress)
        {
            var unitKeys = unitMovePaths.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= unitKeys.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                
                var unit = BattleUnitManager.Instance.GetUnitByIdx(unitKeys[AcitonUnitIdx]);
                
                if (!unit.BattleUnitData.Exist())
                {
                    AcitonUnitIdx++;
                    continue;
                }
                
                if (unitMovePaths[unitKeys[AcitonUnitIdx]] == null || unitMovePaths[unitKeys[AcitonUnitIdx]].Count <= 0)
                    continue;

                if (unitMoveDatas.ContainsKey(unitKeys[AcitonUnitIdx]))
                {
                    break;
                }
                else
                {
                    AcitonUnitIdx++;
                }
            }
            
            if(ActionProgress != actionProgress)
                return;
            
            var unitID = unitKeys[AcitonUnitIdx];
            var moveActionData = unitMoveDatas[unitKeys[AcitonUnitIdx]];

            var runTime = 0f;

            if (moveActionData.MoveGridPosIdxs.Count > 1)
            {
                var effectUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(unitKeys[AcitonUnitIdx]);
                runTime = effectUnitEntity.GetMoveTime(EUnitActionState.Run, moveActionData);
                effectUnitEntity.Run(moveActionData);
                //runTime = BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Run(moveActionData);
            }

            AcitonUnitIdx++;
            
            GameUtility.DelayExcute(runTime, () =>
            {
                // if (unitAttackDatas.ContainsKey(unitID))
                // {
                //     var attackTime = UnitAttack(unitID, unitAttackDatas[unitID]);
                //     GameUtility.DelayExcute(attackTime, () =>
                //     {
                //         HeroManager.Instance.HeroEntity.UpdateCacheHPDelta();
                //         BattleManager.Instance.ContinueAction();
                //     });
                // }
                // else
                // {
                //     BattleManager.Instance.ContinueAction();
                // }
                BattleManager.Instance.ContinueAction();
                
            });
        }
        
        
        public void EnemyAttack()
        {
            UnitAttack(RoundFightData.EnemyAttackDatas, EActionProgress.EnemyAttack);
            
        }

        
        
        public void UseCardTrigger()
        {
            BattleFightManager.Instance.ActionProgress = EActionProgress.UseCardTrigger;
            
            var unitKeys = BattleUnitDatas.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= unitKeys.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                
                if (RoundFightData.UseCardTriggerDatas.ContainsKey(unitKeys[AcitonUnitIdx]))
                {
                    break;
                }
                else
                {
                    AcitonUnitIdx++;
                }
            }
            
            if(ActionProgress != EActionProgress.UseCardTrigger)
                return;
            
            var actionData = RoundFightData.UseCardTriggerDatas[unitKeys[AcitonUnitIdx]];
            
            var isAttack = false;
            foreach (var trigger in actionData.TriggerDatas)
            {
                foreach (var triggerData in trigger.Value)
                {
                    isAttack = true;
                    //TriggerAction(triggerData);
                    BattleBulletManager.Instance.AddTriggerData(triggerData);
                }
            }

            if (isAttack)
            {
                Log.Debug("AA:" + unitKeys[AcitonUnitIdx]);
                BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Attack(actionData);
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            }
            
            
            AcitonUnitIdx++;

            GameUtility.DelayExcute(isAttack ? 1f : 0.5f, () =>
            {
                UseCardTrigger();
            });
        }

        public List<TriggerData> GetMoveTriggerDatas(MoveActionData moveActionData, int moveIdx)
        {
            if(moveActionData == null)
                return null;
            
            if(moveActionData.TriggerDatas.ContainsKey(moveIdx))
                return moveActionData.TriggerDatas[moveIdx];
            
            return null;
        }

        public void MoveEffectAction(EUnitActionState unitActionState, MoveActionData moveActionData, int moveIdx, int moveUnitID)
        {
            var triggerDatas = GetMoveTriggerDatas(moveActionData, moveIdx);//unitActionState == EUnitActionState.Run ? GetRunTriggerDatas(effectID, moveIdx) : GetFlyTriggerDatas(actionID, effectID, moveIdx));
            if(triggerDatas == null)
                return;
            
            foreach (var triggerData in triggerDatas)
            {
                //
                var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
                Log.Debug("ActionUnitID:" + triggerData.ActionUnitIdx);
                //!(!triggerData.ChangeHPInstantly && HeroManager.Instance.IsHero(triggerData.EffectUnitID))
                if (triggerData.TriggerDataSubType == ETriggerDataSubType.Collision)
                {
                    //effectUnit?.Hurt();
                    triggerData.IsTrigger = true;
                    TriggerAction(triggerData);
                    BattleEffectManager.Instance.ShowCollideEffect(effectUnit.EffectHurtPos.position);


                }
                else if (triggerData.TriggerDataSubType == ETriggerDataSubType.State)
                {
                    triggerData.IsTrigger = true;
                    TriggerAction(triggerData);
                }
                else
                {
                    if (triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx)
                    {
                        actionUnit?.CloseSingleAttack();
                        //effectUnit.Hurt();
                        BattleBulletManager.Instance.AddTriggerData(triggerData); 
                    }
                    else
                    {
                        triggerData.IsTrigger = true;
                            
                        BattleFightManager.Instance.TriggerAction(triggerData.Copy());
                    }
                           
                }
                // if (triggerData.EffectUnitIdx != triggerData.ActionUnitIdx)
                // {
                //     
                //     
                // }
                
                
                
            }
            GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }


        
        public Dictionary<int, MoveUnitData> GetHurtMoveDatas(int actionUnitIdx, int effectGridPosIdx)
        {
            var moveDatas = new Dictionary<int, MoveUnitData>();

            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    if(!(kv2.Value.ActionUnitIdx == actionUnitIdx && kv2.Value.EffectGridPosIdx == effectGridPosIdx))
                        continue;
                    
                    moveDatas.Add(kv2.Key, kv2.Value);
                        
                }
               
            }
            
            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    if(!(kv2.Value.ActionUnitIdx == actionUnitIdx && kv2.Value.EffectGridPosIdx == effectGridPosIdx))
                        continue;
                    
                    moveDatas.Add(kv2.Key, kv2.Value);
                        
                }
  
            }

            return moveDatas;
        }
       
        public Dictionary<int, List<int>> GetAttackHurtFlyPaths(int actionUnitIdx, int effectUnitIdx)
        {
            Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
            var unitFlyDict = new Dictionary<int, List<int>>();
            
            if (RoundFightData.EnemyAttackDatas.ContainsKey(actionUnitIdx))
            {
                moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[actionUnitIdx].MoveData
                    .MoveUnitDatas;
                
                foreach (var kv in moveDataDict)
                {
                    var moveGridPosIdxs = kv.Value.MoveActionData.MoveGridPosIdxs;
                
                    if(effectUnitIdx != kv.Value.MoveActionData.MoveUnitIdx)
                        continue;
 
                    unitFlyDict.Add(kv.Key, moveGridPosIdxs);
                }
            }
            else if (RoundFightData.SoliderActiveAttackDatas.ContainsKey(actionUnitIdx))
            {
                moveDataDict = BattleFightManager.Instance.RoundFightData.SoliderActiveAttackDatas[actionUnitIdx].MoveData
                    .MoveUnitDatas;
                
                foreach (var kv in moveDataDict)
                {
                    // if(effectUnitIdx != kv.Value.MoveActionData.MoveUnitIdx && actionUnitIdx != kv.Value.MoveActionData.MoveUnitIdx)
                    //     continue;
                    
                    var moveGridPosIdxs = kv.Value.MoveActionData.MoveGridPosIdxs;
                    unitFlyDict.Add(kv.Key, moveGridPosIdxs);

                }
            }

            return unitFlyDict;
        }
        

        public bool InObstacle(Dictionary<int, EGridType> obstacleMask, List<int> movePaths)
        {
            if (movePaths[0] == movePaths[movePaths.Count - 1])
                return false;
            
            foreach (var gridPosIdx in movePaths)
            {
                //(gridPosIdx != movePaths[movePaths.Count - 1] && obstacleMask[gridPosIdx] == EGridType.Obstacle)
                if(obstacleMask[gridPosIdx] == EGridType.Obstacle ||
                   (gridPosIdx == movePaths[movePaths.Count - 1] && obstacleMask[gridPosIdx] == EGridType.Unit))
                    return true;
            }
            

            
            return false;

        }
        
        
        public List<int> GetRunPaths(Dictionary<int, EGridType> gridTypes, int startPosIdx, int endPosIdx, List<int> runPaths)
        {
            var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);
            var endCoord = GameUtility.GridPosIdxToCoord(endPosIdx);
            
            runPaths.Clear();
            runPaths.Add(startPosIdx);

            if (gridTypes[endPosIdx] != EGridType.Empty)
                return runPaths;
            
            var deltaX = endCoord.x - startCoord.x;
            var deltaY = endCoord.y - startCoord.y;
            var signX = 0;
            var signY = 0;
            if (deltaX < 0)
            {
                signX = -1;
            }
            else if (deltaX > 0)
            {
                signX = 1;
            }
            
            if (deltaY < 0)
            {
                signY = -1;
            }
            else if (deltaY > 0)
            {
                signY = 1;
            }

            if (deltaX == 0 && deltaY == 0)
                return runPaths;


            var idx = 0;
            var isMoveDirect = false;
            var targetCoord = startCoord;
            while (true)
            {

                targetCoord = targetCoord +
                              new Vector2Int(signX, signY);
                
                if (!GameUtility.InGridRange(targetCoord))
                    break;
                
                var gridPosIdx =
                    GameUtility.GridCoordToPosIdx(targetCoord);
                
                if (gridTypes[gridPosIdx] == EGridType.Obstacle)
                    break;
                
                
                runPaths.Add(gridPosIdx);

                var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
                if (gridProp != null && !MoveDirectPropUseDict[gridProp.Idx].UseInRound)
                {
                    MoveDirectPropUseDict[gridProp.Idx].UseInRound = true;
                    var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.Idx].Direct];
                    signX = newDirectCoord.x;
                    signY = newDirectCoord.y;
                    isMoveDirect = true;

                }
                // var gridProp = (
                //     EGridPropID.MoveDirect, gridPosIdx);
                // if (gridProp != null && gridProp is Data_GridPropMoveDirect moveDirect && !moveDirect.UseInRound)
                // {
                //     moveDirect.UseInRound = true;
                //     var newDirectCoord = Constant.Battle.EPos2CoordMap[moveDirect.Direct];
                //     signX = newDirectCoord.x;
                //     signY = newDirectCoord.y;
                //     isMoveDirect = true;
                //}
                
                if (!isMoveDirect && targetCoord == endCoord)
                {
                    break;
                }

                idx++;
            }
            
            
            // if (deltaX != 0)
            // {
            //     
            //
            // }
            // else if (deltaY != 0)
            // {
            //     var signY = deltaY < 0 ? -1 : 1;
            //     for (int i = 0; i < deltaY * signY; i++)
            //     {
            //         var gridPosIdx =
            //             GameUtility.GridCoordToPosIdx(startCoord + new Vector2Int(0, (i + 1) * signY));
            //         movePosIdxs.Add(gridPosIdx);
            //
            //
            //     }
            // }

            return runPaths;
        }

        public List<int> GetFlyPaths(int startPosIdx, Vector2Int direct, int dis, EUnitActionState unitActionState)
        {
            direct = GameUtility.GetDirect(direct);
            //var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);

            var endPosIdx = GameUtility.GetEndPosIdx(startPosIdx, direct, dis);

            return GetFlyPaths(startPosIdx, endPosIdx, unitActionState);
            
            // var flyPosIdxs = new List<int>();
            // flyPosIdxs.Add(startPosIdx);
            //
            // direct = GameUtility.GetDirect(direct);
            //
            // // var signX = 0;
            // // var signY = 0;
            // // if (direct.x < 0)
            // // {
            // //     signX = -1;
            // // }
            // // else if (direct.x > 0)
            // // {
            // //     signX = 1;
            // // }
            // //
            // // if (direct.y < 0)
            // // {
            // //     signY = -1;
            // // }
            // // else if (direct.y > 0)
            // // {
            // //     signY = 1;
            // // }
            //
            //
            // var idx = 0;
            // var isMoveDirect = false;
            // var targetCoord = startCoord;
            // var lastGridPosIdx = startPosIdx;
            // while (true)
            // {
            //     targetCoord = targetCoord +
            //                   direct;
            //     
            //     if (!GameUtility.InGridRange(targetCoord))
            //         break;
            //     
            //     var gridPosIdx =
            //         GameUtility.GridCoordToPosIdx(targetCoord);
            //     
            //     if (BattleManager.Instance.BattleData.GridTypes[gridPosIdx] == EGridType.Obstacle)
            //     {
            //         break;
            //     }
            //     
            //     flyPosIdxs.Add(gridPosIdx);
            //
            //     var unit = GameUtility.GetUnitByGridPosIdx(gridPosIdx);
            //     // && unit.GetStateCount(EUnitState.UnBePass) > 0
            //     if (unit != null)
            //     {
            //         flyPosIdxs.Add(lastGridPosIdx);
            //         break;
            //     }
            //     
            //     
            //     
            //     var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
            //     if (gridProp != null && !MoveDirectPropUseDict[gridProp.ID].UseInRound)
            //     {
            //         MoveDirectPropUseDict[gridProp.ID].UseInRound = true;
            //         var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.ID].Direct];
            //         direct.x = newDirectCoord.x;
            //         direct.y = newDirectCoord.y;
            //         isMoveDirect = true;
            //
            //     }
            //     idx++;
            //     
            //     lastGridPosIdx = gridPosIdx;
            // }
            //
            // return flyPosIdxs;
        }
        
        public List<int> GetFlyPaths(int startPosIdx, int endPosIdx, EUnitActionState unitActionState)
        {
            var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);
            var endCoord = GameUtility.GridPosIdxToCoord(endPosIdx);
            var direct = endCoord - startCoord;
            
            var flyPosIdxs = new List<int>();

            if (startPosIdx == endPosIdx)
            {
                return flyPosIdxs;
            }
            
            flyPosIdxs.Add(startPosIdx);
            
            
            
            // var signX = 0;
            // var signY = 0;
            // if (direct.x < 0)
            // {
            //     signX = -1;
            // }
            // else if (direct.x > 0)
            // {
            //     signX = 1;
            // }
            //
            // if (direct.y < 0)
            // {
            //     signY = -1;
            // }
            // else if (direct.y > 0)
            // {
            //     signY = 1;
            // }
            
            direct = GameUtility.GetDirect(direct);


            var idx = 0;
            //var isMoveDirect = false;
            var targetCoord = startCoord;
            var lastGridPosIdx = startPosIdx;
            while (true)
            {
                targetCoord = targetCoord +
                              direct;
                
                if (!GameUtility.InGridRange(targetCoord))
                    break;
                
                var gridPosIdx =
                    GameUtility.GridCoordToPosIdx(targetCoord);
                
                flyPosIdxs.Add(gridPosIdx);
                

                var unit = GameUtility.GetUnitByGridPosIdx(gridPosIdx);
                // && unit.GetStateCount(EUnitState.UnBePass) > 0
                if (unitActionState != EUnitActionState.Throw && (unit != null ||
                                                                  RoundFightData.GamePlayData.BattleData.GridTypes[
                                                                      gridPosIdx] == EGridType.Obstacle))
                {
                    flyPosIdxs.Add(lastGridPosIdx);
                    break;
                }
                else if (unitActionState == EUnitActionState.Throw &&
                         RoundFightData.GamePlayData.BattleData.GridTypes[gridPosIdx] == EGridType.Empty)
                {
                    break;
                }
                
                var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
                var isMoveDirect = gridProp != null && !MoveDirectPropUseDict[gridProp.Idx].UseInRound;
                if (isMoveDirect)
                {
                    MoveDirectPropUseDict[gridProp.Idx].UseInRound = true;
                    var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.Idx].Direct];
                    direct.x = newDirectCoord.x;
                    direct.y = newDirectCoord.y;

                }

                if (targetCoord == endCoord && !isMoveDirect)
                {
                    var gridType = RoundFightData.GamePlayData.BattleData.GridTypes[gridPosIdx];
                    if (gridType == EGridType.Obstacle)
                    {
                        flyPosIdxs.Remove(gridPosIdx);
                        break;
                    }
                    
                    if (gridType == EGridType.Unit || gridType == EGridType.TemporaryUnit)
                    {
                        flyPosIdxs.Add(lastGridPosIdx);
                        break;
                    }
                    
                    if(!isMoveDirect)
                        break;

                }
                    
                lastGridPosIdx = gridPosIdx;   
                idx++;
            }

            return flyPosIdxs;
        }
        
        public void RefreshPropMoveDirectUseInRound()
        {
            foreach (var kv in MoveDirectPropUseDict)
            {
                if(RoundFightData.GamePlayData.BattleData.GridPropDatas.ContainsKey(kv.Key))
                
                MoveDirectPropUseDict[kv.Key].UseInRound = (RoundFightData.GamePlayData.BattleData.GridPropDatas[kv.Key] as Data_GridPropMoveDirect).UseInRound;

            }
        }
        
        

        public int ChangeHP(Data_BattleUnit unit, float value, EHPChangeType hpChangeType, bool useDefense = true, bool changeHPInstantly = false)
        {
            return BattleManager.Instance.ChangeHP(unit, (int)value, RoundFightData.GamePlayData, hpChangeType, useDefense, true, changeHPInstantly);

        }
        
        private bool IsEnemy(int unit1Idx, int unit2Idx)
        {
            var unit1 = BattleFightManager.Instance.GetUnitByIdx(unit1Idx);
            var unit2 = BattleFightManager.Instance.GetUnitByIdx(unit2Idx);

            return unit1.UnitCamp != unit2.UnitCamp;
        }
        
        public List<int> GetEffectUnitIdxs(BuffData buffData, int ownUnitIdx, int actionUnitIdx, int effectUnitIdx, int actionUnitGridPosIdx, int actionUnitPreGridPosIdx)
        {
            var realEffectUnitIdxs = new List<int>();
            
            var actionUnit = BattleFightManager.Instance.GetUnitByIdx(actionUnitIdx);
            if (actionUnit != null && actionUnitGridPosIdx == -1)
            {
                actionUnitGridPosIdx = actionUnit.GridPosIdx;
            }
            var ownUnit = BattleFightManager.Instance.GetUnitByIdx(ownUnitIdx);
            var ownUnitGridPosIdx = -1;
            if (ownUnit != null)
            {
                ownUnitGridPosIdx = ownUnit.GridPosIdx;
            }
            
            //var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
            var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionUnitGridPosIdx);
            var actionUnitLastCoord = GameUtility.GridPosIdxToCoord(actionUnitPreGridPosIdx);
            
            var effectUnit = BattleFightManager.Instance.GetUnitByIdx(effectUnitIdx);
            var effectUnitCoord = Vector2Int.zero;
            if (effectUnit != null)
            {
                effectUnitCoord = GameUtility.GridPosIdxToCoord(effectUnit.GridPosIdx);
            }
            
            ;
            
            foreach (var triggerTarget in buffData.TriggerTargets)
            {
                switch (triggerTarget)
                {
                    case ETriggerTarget.Staff:
                        if (buffData.BuffStr.Contains("Tactic"))
                        {
                            foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                            {
                                if(kv.Value.UnitRole != EUnitRole.Staff)
                                    continue;
                                
                                if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != EUnitCamp.Player1)
                                {
                                    realEffectUnitIdxs.Add(kv.Value.Idx);
                                }
                                else if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == EUnitCamp.Player1)
                                {
                                    realEffectUnitIdxs.Add(kv.Value.Idx);
                                }
                            }
                        }
                        break;
                    case ETriggerTarget.Effect:
                        if (effectUnitIdx != -1)
                        {
                            var isEnemy = false;
                            if (actionUnitIdx != -1 && actionUnitIdx != Constant.Battle.UnUnitTriggerIdx)
                            {
                                isEnemy = IsEnemy(actionUnitIdx, effectUnitIdx);
                            }
                            else
                            {
                                isEnemy = true;
                            }
                            
                            
                            if (isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                            {
                                realEffectUnitIdxs.Add(effectUnitIdx);
                            }
                            else if (!isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                            {
                                realEffectUnitIdxs.Add(effectUnitIdx);
                            }
                        }

                        break;
                    case ETriggerTarget.LineExtend:
                        if (effectUnitIdx != -1 && actionUnitIdx  != -1)
                        {
                            var coords = GameUtility.GetRelatedCoords(EActionType.LineExtend, actionUnit.GridPosIdx,
                                effectUnit.GridPosIdx);
                            
                            for (int i = 0; i < coords.Count; i++)
                            {
                                var endPosIdx = GameUtility.GridCoordToPosIdx(coords[i]);
                                if (endPosIdx != actionUnit.GridPosIdx)
                                {
                                    var endPosUnit = GameUtility.GetUnitByGridPosIdx(endPosIdx);
                                    if (endPosUnit != null)
                                    {
                                        var isEndPosEnemy = IsEnemy(actionUnitIdx, endPosUnit.Idx);
                            
                                        if (isEndPosEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                                        {
                                            realEffectUnitIdxs.Add(endPosUnit.Idx);
                                            break;
                                        }
                                        else if (!isEndPosEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                                        {
                                            realEffectUnitIdxs.Add(endPosUnit.Idx);
                                            break;
                                        }

                                    }
                                }
                                
                            }
                            
                            // var isEnemy = IsEnemy(actionUnitIdx, effectUnitIdx);
                            //
                            // if (isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                            // {
                            //     realEffectUnitIdxs.Add(effectUnitIdx);
                            // }
                            // else if (!isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                            // {
                            //     realEffectUnitIdxs.Add(effectUnitIdx);
                            // }
                        }

                        break;
                    case ETriggerTarget.Action:
                        // var Action_isEnemy = false;
                        // if (actionUnitIdx != -1 && actionUnitIdx != Constant.Battle.UnUnitTriggerIdx && effectUnitIdx != -1)
                        // {
                        //     Action_isEnemy = IsEnemy(actionUnitIdx, effectUnitIdx);
                        // }
                        // else
                        // {
                        //     Action_isEnemy = true;
                        // }
                        //
                        // if (Action_isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                        // {
                        //     realEffectUnitIdxs.Add(actionUnitIdx);
                        // }
                        // else if (!Action_isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                        // {
                        //     realEffectUnitIdxs.Add(actionUnitIdx);
                        // }
                
                        realEffectUnitIdxs.Add(actionUnitIdx);
                            

                        break;
                    case ETriggerTarget.Hero:
                        realEffectUnitIdxs.Add(PlayerData.BattleHero.Idx);
                        break;
                    case ETriggerTarget.All:
                        foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                        {
                            if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != actionUnit.UnitCamp)
                            {
                                realEffectUnitIdxs.Add(kv.Value.Idx);
                            }
                            else if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == actionUnit.UnitCamp)
                            {
                                realEffectUnitIdxs.Add(kv.Value.Idx);
                            }
                        }
                        
                        break;
                    case ETriggerTarget.InRange:
                        var range = GameUtility.GetRange(ownUnitGridPosIdx, buffData.TriggerRange,
                            ownUnit != null ? ownUnit.UnitCamp : BattleManager.Instance.CurUnitCamp, buffData.TriggerUnitCamps);
                        foreach (var gridPosIdx in range)
                        {
                            var unit = BattleFightManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
                            if (unit != null)
                            {
                                realEffectUnitIdxs.Add(unit.Idx);
                            }
                        }
                        break;
                    case ETriggerTarget.Vertical:
                        var actionUnitDirect = actionUnitCoord - actionUnitLastCoord;
                        var vertical1 = new Vector2Int(-actionUnitDirect.y, actionUnitDirect.x);
                        var vertical2 = new Vector2Int(actionUnitDirect.y, -actionUnitDirect.x);
                        var vertical1GridPosIdx = GameUtility.GridCoordToPosIdx(actionUnitCoord + vertical1);
                        var vertical2GridPosIdx = GameUtility.GridCoordToPosIdx(actionUnitCoord + vertical2);
                        var vertical1Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(vertical1GridPosIdx);
                        if (vertical1Unit != null)
                        {
                            realEffectUnitIdxs.Add(vertical1Unit.Idx);
                        }
                        var vertical2Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(vertical2GridPosIdx);
                        if (vertical2Unit != null)
                        {
                            realEffectUnitIdxs.Add(vertical2Unit.Idx);
                        }
                        
                        break;
                    case ETriggerTarget.Horizontal:
                        
              
                        var relatedDirect = effectUnitCoord - actionUnitCoord;
                        relatedDirect = GameUtility.GetDirect(relatedDirect);

                        var horizontals = GameUtility.GetRelatedHorizontalCoords(relatedDirect, effectUnitCoord);
                        var horizontal1GridPosIdx = GameUtility.GridCoordToPosIdx(effectUnitCoord + horizontals[0]);
                        var horizontal2GridPosIdx = GameUtility.GridCoordToPosIdx(effectUnitCoord + horizontals[1]);
                        var horizontal1Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(horizontal1GridPosIdx);
                        if (horizontal1Unit != null)
                        {
                            realEffectUnitIdxs.Add(horizontal1Unit.Idx);
                        }
                        var horizontal2Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(horizontal2GridPosIdx);
                        if (horizontal2Unit != null)
                        {
                            realEffectUnitIdxs.Add(horizontal2Unit.Idx);
                        }

                        break;
                    case ETriggerTarget.Select:
                        realEffectUnitIdxs.Add(effectUnitIdx);
                        break;
                    case ETriggerTarget.EffectChain:
                        if (effectUnitIdx != -1 && actionUnitIdx  != -1)
                        {
                            var chains = new List<int>();
                            chains.Add(effectUnit.GridPosIdx);
                            ColllectChain(effectUnit.GridPosIdx, actionUnit.UnitCamp, buffData.TriggerUnitCamps,
                                new List<int>(){ actionUnit.GridPosIdx }, chains);

                            foreach (var chain in chains)
                            {
                                var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(chain);
                                if (unit != null)
                                {
                                    realEffectUnitIdxs.Add(unit.UnitIdx);
                                }
                            }

                            
                            
                            // var isEnemy = IsEnemy(actionUnitIdx, effectUnitIdx);
                            //
                            // if (isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                            // {
                            //     realEffectUnitIdxs.Add(effectUnitIdx);
                            // }
                            // else if (!isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                            // {
                            //     realEffectUnitIdxs.Add(effectUnitIdx);
                            // }
                        }

                        break;
                    default:
                        break;
                }
            }

            //GameUtility.SortHeroIDToLast(realEffectUnitIdxs);
            
            return realEffectUnitIdxs;
        }


        public void ColllectChain(int gridPosIdx, EUnitCamp unitCamp, List<ERelativeCamp> relativeCamps, List<int> exceptGridPosIdxs, List<int> chains)
        {
            var excepts = new List<int>();
            excepts.AddRange(exceptGridPosIdxs);
            excepts.AddRange(chains);
            var chainGridPosIdxs = GameUtility.GetRange(gridPosIdx, EActionType.Direct82Short, unitCamp, relativeCamps,
                true, false, excepts);
            
            
            
            foreach (var linkGridPosIdx in chainGridPosIdxs)
            {
                chains.Add(linkGridPosIdx);
                ColllectChain(linkGridPosIdx, unitCamp, relativeCamps, exceptGridPosIdxs, chains);
            }
            
        }
        
        
        
        public void StartMoveTrigger(Data_BattleUnit unit, List<TriggerData> triggerDatas)
        {

            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, unit, triggerDatas, EBuffTriggerType.StartMove);

        }
        
        public void KillTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var unit = BattleFightManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
  
            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, unit, triggerData, triggerDatas, EBuffTriggerType.Kill);

        }

        public void DeadTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var effectUnit = BattleFightManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            var actionUnit = BattleFightManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
            if(effectUnit == null || effectUnit.Exist())
                return;

            
            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, effectUnit, triggerData, triggerDatas, EBuffTriggerType.Dead);


            TriggerUnitData(effectUnit.Idx, actionUnit == null ? -1 : actionUnit.Idx, effectUnit.GridPosIdx, EBuffTriggerType.Dead, triggerDatas);
            
            var enemyDeadDebuffToOtherEnemy = BattleFightManager.Instance.RoundFightData.GamePlayData.GetUsefulBless(EBlessID.EnemyDeadDebuffToOtherEnemy, BattleManager.Instance.CurUnitCamp);
            
            if (actionUnit != null && effectUnit.UnitCamp != actionUnit.UnitCamp && enemyDeadDebuffToOtherEnemy != null)
            {
                var otherEnemies = new List<Data_BattleUnit>();
                foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                {
                    if (kv.Value.UnitCamp != actionUnit.UnitCamp && kv.Value.Exist())
                    {
                        otherEnemies.Add(kv.Value);
                    }
                }

                if (otherEnemies.Count > 0)
                {
                    var randomEnemyIdx = Random.Next(0, otherEnemies.Count);
                    foreach (var kv in effectUnit.UnitStateData.UnitStates)
                    {
                        if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(kv.Key))
                        {
                            otherEnemies[randomEnemyIdx].ChangeState(kv.Key, kv.Value.Value);
                        }
                    }
                }

            }

            
        }
        
        public void CollideTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var unit = BattleFightManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
  
            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, unit, triggerData, triggerDatas, EBuffTriggerType.Collide);

        }

        public void AddDeBuffTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var effectUnit = BattleFightManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            
            if (effectUnit == null || !effectUnit.Exist())
                return;


            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, effectUnit, triggerData, triggerDatas,
                EBuffTriggerType.AddDeBuff);
        }

        
        
        private Dictionary<int, EGridType> curObstacleMask = new Dictionary<int, EGridType>();

        private void RefreshObstacleMask()
        {
            curObstacleMask.Clear();
            foreach (var kv in RoundFightData.GamePlayData.BattleData.GridTypes)
            {
                if (kv.Value == EGridType.Obstacle)
                {
                    curObstacleMask[kv.Key] = EGridType.Obstacle; 
                }
                else if (kv.Value == EGridType.TemporaryUnit)
                {
                    curObstacleMask[kv.Key] = EGridType.Unit; 
                }
                else
                {
                    curObstacleMask[kv.Key] = EGridType.Empty; 
                }
            }
            
            foreach (var kv in  BattleUnitDatas)
            {
                curObstacleMask[kv.Value.GridPosIdx] =  EGridType.Unit;
            }
            
            // foreach (var kv in  BattleUnitDatas)
            // {
            //     if (kv.Value.UnitCamp == (unitCamp == EUnitCamp.Enemy ? EUnitCamp.Third : EUnitCamp.Enemy) || kv.Value.UnitCamp == EUnitCamp.Player1 || kv.Value.UnitCamp == EUnitCamp.Player2)
            //     {
            //         curObstacleMask[kv.Value.GridPosIdx] =  EGridType.Unit;    
            //     }
            // }
            //
            // foreach (var enemyGridPosIdx in obstacleEnemies)
            // {
            //     curObstacleMask[enemyGridPosIdx] =  EGridType.Unit;
            // }

            var playerData = RoundFightData.GamePlayData.GetPlayerData(EUnitCamp.Player1);
            curObstacleMask[playerData.BattleHero.GridPosIdx] =  EGridType.Unit; 
        }

        
        
        public int GetCollisionHurt(EUnitActionState unitActionState)
        {
            switch (unitActionState)
            {
                case EUnitActionState.Fly:
                    return Constant.Battle.FlyHurt;
                case EUnitActionState.Rush:
                    return Constant.Battle.RushHurt;
                default:
                    return 0; 
            }

        }
        
        public Data_BattleUnit GetUnitByGridPosIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null,
            ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null, int exceptUnitIdx = -1)
        {

            return InternalGetUnitByGridPosIdx(BattleUnitDatas, gridPosIdx, selfUnitCamp, unitCamp, unitRole,
                exceptUnitIdx);
        }
        
        public List<Data_BattleUnit> GetUnitsByCamp(EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null)
        {
            var units = new List<Data_BattleUnit>();
            foreach (var kv in BattleUnitDatas)
            {
                if(!kv.Value.Exist())
                    continue;
                
                if (unitCamp == ERelativeCamp.Us && kv.Value.UnitCamp == selfUnitCamp ||
                    unitCamp == ERelativeCamp.Enemy && kv.Value.UnitCamp != selfUnitCamp || 
                    unitCamp == null)
                {
                    units.Add(kv.Value);
                }
            }
            
            return units;
            
            
            
        }

        public List<Data_BattleUnit> GetUnitByGridPosIdx(int actionUnitGridPosIdx, int effectUnitGridPosIdx, EActionType actionType)
        {
            var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionUnitGridPosIdx);
            var effectUnitCoord = GameUtility.GridPosIdxToCoord(effectUnitGridPosIdx);

            var pointList = new List<List<Vector2Int>>();
            if (Constant.Battle.DynamicRelatedUnitFlyRanges.Contains(actionType))
            {
                pointList.Add(new List<Vector2Int>(){ new Vector2Int(0, 0)} );
                pointList.Add(GameUtility.GetRelatedCoords(actionType, actionUnitGridPosIdx, effectUnitGridPosIdx));
            }
            else
            {
                pointList = Constant.Battle.ActionTypePoints[actionType];
            }

            var units = new List<Data_BattleUnit>();

            var idx = 0;
            foreach (var points in pointList)
            {
                idx++;
                if(idx == 1)
                    continue;
                
                foreach (var point in points)
                {
                    var targetCoord = effectUnitCoord + point;
                    var targetGridPosIdx = GameUtility.GridCoordToPosIdx(targetCoord);
                    var unit = GetUnitByGridPosIdx(targetGridPosIdx);
                    if (unit != null)
                    {
                        units.Add(unit);
                    }
                    
                }
            }

            return units;
        }

        private Data_BattleUnit InternalGetUnitByGridPosIdx(Dictionary<int, Data_BattleUnit> battleUnitDatas,
            int gridPosIdx, EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null,
            int exceptUnitID = -1)
        {
            if (BattleUnitDatasByGridPosIdx.ContainsKey(gridPosIdx))
            {
                var unit = BattleUnitDatasByGridPosIdx[gridPosIdx];
                if (unitCamp == ERelativeCamp.Us && selfUnitCamp != unit.UnitCamp)
                    return null;

                if (unitCamp == ERelativeCamp.Enemy && selfUnitCamp == unit.UnitCamp)
                    return null;

                if (unitRole != null && unit.UnitRole != unitRole)
                {
                    return null;
                }

                if (unit.Idx == exceptUnitID)
                {
                    return null;
                }

                return unit;
            }
            else
            {
                return null;
            }
            
            // foreach (var kv in battleUnitDatas)
            // {
            //
            //     if (unitCamp == ERelativeCamp.Us && selfUnitCamp != kv.Value.UnitCamp)
            //         continue;
            //
            //     if (unitCamp == ERelativeCamp.Enemy && selfUnitCamp == kv.Value.UnitCamp)
            //         continue;
            //
            //     if (unitRole != null && kv.Value.UnitRole != unitRole)
            //     {
            //         continue;
            //     }
            //
            //     if (kv.Value.ID == exceptUnitID)
            //     {
            //         continue;
            //     }
            //
            //
            //     if (kv.Value.GridPosIdx == gridPosIdx)
            //     {
            //         return kv.Value;
            //     }
            // }
            //
            //
            //
            // return null;
        }

        public Data_BattleUnit GetUnitByIdx(int id)
        {
            if (BattleUnitDatas.ContainsKey(id))
            {
                return BattleUnitDatas[id];
            }

            return null;

            // foreach (var kv in BattleUnitDatas)
            // {
            //     // if (unitCamp != null && kv.Value.UnitCamp != unitCamp)
            //     //     continue;
            //
            //     // if (attackType != null && kv.Value is Data_BattleSolider solider)
            //     // {
            //     //     var drBuff = CardManager.Instance.GetBuffTable(solider.CardID);
            //     //     var soliderAttackType = drBuff.TriggerRange;
            //     //     if (soliderAttackType != attackType)
            //     //     {
            //     //         continue;
            //     //     }
            //     // }
            //     // else if (attackType != null && kv.Value is Data_BattleMonster enemy)
            //     // {
            //     //     var drEnemy = GameEntry.DataTable.GetEnemy(enemy.EnemyTypeID);
            //     //     var drBuff = GameEntry.DataTable.GetBuff(drEnemy.OwnBuffs[0]);
            //     //     var enemyAttackType = drBuff.TriggerRange;
            //     //     if (enemyAttackType != attackType)
            //     //     {
            //     //         continue;
            //     //     }
            //     // }
            //
            //     if (kv.Value.ID == id)
            //     {
            //         return kv.Value;
            //     }
            // }
            //
            //
            // return null;
        }

        public Data_BattleUnit GetUnitByGridPosIdxMoreCamps(int gridPosIdx, EUnitCamp? selfUnitCamp = null,
            List<ERelativeCamp> unitCamps = null)
        {
            if (unitCamps == null)
                return null;
            
            foreach (var unitCamp in unitCamps)
            {
                var unit = GetUnitByGridPosIdx(gridPosIdx, selfUnitCamp, unitCamp);
                if (unit != null)
                {
                    return unit;
                }
            }

            return null;
        }

    }
}