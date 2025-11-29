using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace RoundHero
{

    public partial class BattleFightManager : Singleton<BattleFightManager>
    {
        private void CachePreData()
        {
            RoundFightData.Clear();
            RoundFightData.HPDeltaDict = new Dictionary<EUnitCamp, List<HPDeltaData>>()
            {
                [EUnitCamp.Player1] = new List<HPDeltaData>(),
                [EUnitCamp.Player2] = new List<HPDeltaData>(),
            };
            //Log.Debug("CachePreData1:" + BattleUnitManager.Instance.BattleUnitDatas.Count + "-" + BattleUnitManager.Instance.BattleUnitEntities.Count);


            RoundFightData.GamePlayData = GamePlayManager.Instance.GamePlayData.Copy();
            RoundFightData.TempTriggerData = BattleManager.Instance.TempTriggerData.Copy();

            PlayerData = RoundFightData.GamePlayData.GetPlayerData(PlayerManager.Instance.PlayerData.UnitCamp);

            foreach (var kv in ActionUnitIDs)
            {
                kv.Value.Clear();
            }

            BattleUnitDatas = RoundFightData.GamePlayData.BattleData.BattleUnitDatas;

            RefreshUnitGridPosIdx();

            foreach (var kv in RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if (kv.Value is Data_BattleHero hero)
                {
                    PlayerData.BattleHero = hero;
                }
            }


            MoveDirectPropUseDict.Clear();
            foreach (var kv in RoundFightData.GamePlayData.BattleData.GridPropDatas)
            {
                if (kv.Value is Data_GridPropMoveDirect moveDirect)
                {
                    MoveDirectPropUseDict.Add(moveDirect.Idx, moveDirect.Copy());

                }

            }


            if (BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.NewUnit)
            {
                BattleCardManager.Instance.CacheUseCardData(
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                    null, BattleManager.Instance.TempTriggerData.TargetGridPosIdx,
                    BattleManager.Instance.TempTriggerData.UnitData.Idx);

                // RoundFightData.GamePlayData.BattleData.GridTypes[RoundFightData.TempTriggerData.UnitData.GridPosIdx] =
                //     EGridType.TemporaryUnit;
                //
                // var newUnitID = RoundFightData.TempTriggerData.UnitData.Idx;
                //
                // if (RoundFightData.GamePlayData.BattleData.BattleUnitDatas.ContainsKey(newUnitID))
                // {
                //     RoundFightData.GamePlayData.BattleData.BattleUnitDatas.Remove(newUnitID);
                // }
                //
                // RoundFightData.GamePlayData.BattleData.BattleUnitDatas.Add(newUnitID,
                //     RoundFightData.TempTriggerData.UnitData);
                //
                // var soliderData = RoundFightData.TempTriggerData.UnitData as Data_BattleSolider;
                // if (soliderData != null)
                // {
                //     FuneManager.Instance.CacheUnitUseData(newUnitID, newUnitID, soliderData.CardIdx,
                //         BattleManager.Instance.CurUnitCamp,
                //         RoundFightData.TempTriggerData.UnitData.GridPosIdx);
                // }
            }
            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.MoveUnit)
            {
                RoundFightData.GamePlayData.BattleData.GridTypes[
                    BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx] = EGridType.Empty;
                RoundFightData.GamePlayData.BattleData.GridTypes[
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx] = EGridType.TemporaryUnit;
                
                var effectUnit = GetUnitByGridPosIdx(BattleManager.Instance.TempTriggerData.TargetGridPosIdx);
                BattleCardManager.Instance.CacheUseCardData(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                    effectUnit, BattleManager.Instance.TempTriggerData.TargetGridPosIdx,
                    Constant.Battle.UnUnitTriggerIdx, true);
                
            }
            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.SelectHurtUnit)
            {
                RoundFightData.GamePlayData.BattleData.GridTypes[
                    BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx] = EGridType.Empty;
                RoundFightData.GamePlayData.BattleData.GridTypes[
                    BattleManager.Instance.TempTriggerData.TargetGridPosIdx] = EGridType.TemporaryUnit;
            }
            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.UseBuff)
            {
                var effectUnit = GetUnitByGridPosIdx(BattleManager.Instance.TempTriggerData.TargetGridPosIdx);
                // RoundFightData.GamePlayData.BattleData.BattleUnitDatas.ContainsKey(BattleManager.Instance
                //     .TempTriggerData.CardEffectUnitIdx)
                //     ? RoundFightData.GamePlayData.BattleData.BattleUnitDatas[
                //         BattleManager.Instance.TempTriggerData.CardEffectUnitIdx]
                //     : null;

                if (BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType == TriggerBuffType.Card)
                {

                    var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                    //if (!string.IsNullOrEmpty(buffStr) && buffStr != EBuffID.Spec_AttackUs.ToString() && buffStr != EBuffID.Spec_MoveUs.ToString())

                    if ((!string.IsNullOrEmpty(buffStr) && buffStr != EBuffID.Spec_AttackUs.ToString() &&
                         buffStr != EBuffID.Spec_MoveUs.ToString() &&
                         buffStr != EBuffID.Spec_MoveEnemy.ToString()) ||
                        string.IsNullOrEmpty(buffStr))
                    {
                        BattleCardManager.Instance.CacheUseCardData(
                            BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                            effectUnit, BattleManager.Instance.TempTriggerData.TargetGridPosIdx,
                            Constant.Battle.UnUnitTriggerIdx);
                    }


                }
                // else if (BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType ==
                //          TriggerBuffType.EnergyBuff)
                // {
                //     BattleCardManager.Instance.CacheTacticCardData(
                //         BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                //         BattleManager.Instance.CurUnitCamp, effectUnit);
                // }

            }

            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.AutoAtk ||
                     BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.ActiveAtk)
            {
                var effectUnit = GetUnitByGridPosIdx(BattleManager.Instance.TempTriggerData.TargetGridPosIdx);
                BattleCardManager.Instance.CacheUseCardData(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                    effectUnit, BattleManager.Instance.TempTriggerData.TargetGridPosIdx,
                    Constant.Battle.UnUnitTriggerIdx, true);
            }
            // else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.MoveUnit)
            // {
            //     
            // }

            if (BattleCardManager.Instance.SelectPassCardIdx != -1)
            {
                BattleCardManager.Instance.CachePassCard();
            }

            // if (BattleCardManager.Instance.PointerCardIdx != -1)
            // {
            //     // BattleCardManager.Instance.CacheTacticCardData(BattleCardManager.Instance.,
            //     //     BattleManager.Instance.CurUnitCamp, null);
            // }

            // if (BattleManager.Instance.TempTriggerData.TriggerType == ETempUnitType.NewUnit)
            // {
            //     var newUnitID = RoundFightData.TempTriggerData.UnitData.Idx;
            //

        }

        public void CachePreRoundFightData()
        {
            //Log.Debug("CacheRoundFightData");

            //BattleAreaManager.Instance.RefreshObstacles();

            CachePreData();
            CachePreRoundStartDatas();
            //CacheLinks();

            //CacheUseCardTriggerDatas();
            //CacheSoliderActiveAttackDatas();
            //CacheSoliderAutoAttackDatas();

            //CacheRoundStartDatas();

            //CacheSoliderMoveDatas();
            //CacheSoliderAttackDatas();
            CacheRoundHandCards(true);
            if (!(TutorialManager.Instance.IsTutorial() && BattleManager.Instance.BattleData.Round == 0))
            {
                CalculateEnemyPaths();
            }
            
            CacheEnemyAttackDatas();
            //CacheEnemyMoveDatas();
            //

            // CalculateThirdUnitPaths();
            // CacheThirdUnitMoveDatas();
            // CacheThirdUnitAttackDatas();

            //CacheRoundEndDatas();


        }

        public void CacheRoundFightData()
        {
            //Log.Debug("CacheRoundFightData");

            //BattleAreaManager.Instance.RefreshObstacles();


            CachePreData();

            CacheLinks();

            CacheUseCardTriggerDatas();

            CacheSoliderActiveAttackDatas();
            CacheSoliderAutoAttackDatas();

            CacheSoliderMoveDatas();
            CacheEnemyMoveDatas();
            
            CacheRoundPassCards();

            CacheRoundStartDatas();


            CacheSoliderAttackDatas();

            CacheEnemyAttackDatas();

            CacheRoundEndDatas();


            CacheRoundHandCards(false);
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }



        public void CacheUseCardTriggerDatas()
        {
            //BattleCardManager.Instance.PointerCardIdx
            if (BattleCardManager.Instance.SelectCardIdx == -1)
                return;

            //CacheUseCardTriggerAttack();
            CacheUseCardTrigger();
        }

        public int CacheConsumeCardEnergy(int cardIdx, List<TriggerData> triggerDatas)
        {
            // if (triggerDatas.Count <= 0)
            //     return 0;

            var _triggerDatas = new List<TriggerData>();
            foreach (var triggerData in triggerDatas)
            {
                if (triggerData.TriggerCardIdx == cardIdx)
                {
                    _triggerDatas.Add(triggerData);
                }

            }
            //

            if (cardIdx == -1)
                return 0;

            var unitIdx = BattleManager.Instance.TempTriggerData.UnitData != null
                ? BattleManager.Instance.TempTriggerData.UnitData.Idx
                : -1;

            var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardIdx, unitIdx);
            cardEnergy = BattleCardManager.Instance.GetCardEnergyDynamicDelta(cardIdx, cardEnergy, triggerDatas,
                RoundFightData.GamePlayData);

            var cardHpDeltaData = new CardHPDeltaData()
            {
                CardIdx = cardIdx,
                //HPDeltaOwnerType = EHPDeltaOwnerType.Card,
                HPDeltaType = EHPDeltaType.UseCard,
                HPDelta = -cardEnergy,
            };


            AddHPDetlaData(PlayerManager.Instance.PlayerData.UnitCamp, cardHpDeltaData);
            //RoundFightData.HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Add(cardHpDeltaData);
            BattleFightManager.Instance.ChangeHP(BattleFightManager.Instance.PlayerData.BattleHero, -cardEnergy,
                EHPChangeType.CardConsume, true, true);

            return cardEnergy;
        }
        
        private void CacheUseCardTrigger()
        {
            foreach (var kv in BattleUnitDatas)
            {
                if (!kv.Value.Exist())
                    continue;

                //(kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
                if (kv.Value.GetAllStateCount(EUnitState.UnAtk) > 0 &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;

                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);

                if (triggerBuffDatas != null)
                {
                    var actionData = new ActionData();
                    actionData.ActionUnitIdx = kv.Value.Idx;

                    var emptyTriggerData = new TriggerData();
                    var triggerDatas = new List<TriggerData>();
                    BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData,
                        kv.Value, emptyTriggerData, triggerDatas, EBuffTriggerType.UseCard);

                    foreach (var triggerData in triggerDatas)
                    {
                        actionData.AddTriggerData(kv.Value.Idx, triggerData, kv.Value);
                    }

                    if (triggerDatas.Count > 0)
                    {
                        RoundFightData.UseCardTriggerDatas.Add(kv.Value.Idx, actionData);
                    }
                    CalculateHeroHPDelta(actionData);
                }


            }
        }

        private void CacheRoundStartBuffDatas()
        {
            foreach (var kv in BattleUnitDatas)
            {
                var actionData = new ActionData();
                actionData.ActionUnitIdx = kv.Key;
                actionData.ActionDataType = EActionDataType.UnitState;
                var triggerDatas = new List<TriggerData>();

                var hurtRoundStartCount = kv.Value.GetAllStateCount(EUnitState.HurtRoundStart);
                if (hurtRoundStartCount > 0)
                {
                    if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    {
                        var subHurtRoundStartData = BattleFightManager.Instance.Unit_State(triggerDatas, kv.Value.Idx,
                            kv.Value.Idx, kv.Value.Idx, EUnitState.HurtRoundStart, -1,
                            ETriggerDataType.State);
                        subHurtRoundStartData.ActionUnitGridPosIdx = subHurtRoundStartData.EffectUnitGridPosIdx = kv.Value.GridPosIdx;
                        BattleBuffManager.Instance.CacheTriggerData(subHurtRoundStartData, triggerDatas);
                        
                        var triggerData = BattleFightManager.Instance.BattleRoleAttribute(kv.Key, kv.Key, kv.Key,
                            EUnitAttribute.HP, -1, ETriggerDataSubType.State);
                        triggerData.UnitStateDetail.UnitState = EUnitState.HurtRoundStart;
                        triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = kv.Value.GridPosIdx;
                        if (BattleCoreManager.Instance.IsCoreIdx(triggerData.EffectUnitIdx))
                        {
                            triggerData.ChangeHPInstantly = false;
                        }

                        BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);

                        // triggerDatas.Add(triggerData);
                        // SimulateTriggerData(triggerData, triggerDatas);
                    }

                    //kv.Value.RemoveState(EUnitState.HurtRoundStart);
                    



                }

                // foreach (var funeID in kv.Value.FuneIDs)
                // {
                //     var triggerData = FuneManager.Instance.RoundTrigger(funeID, kv.Value.ID, kv.Value.ID, triggerDatas);
                //
                // }

                if (triggerDatas.Count > 0)
                {
                    RoundFightData.RoundStartBuffDatas.Add(kv.Key, actionData);
                    actionData.TriggerDataDict.Add(kv.Key, new TriggerCollection()
                    {
                        ActionUnitIdx = kv.Key,
                        EffectTagIdx = kv.Key,
                        TriggerDatas = triggerDatas
                    });
                }

                CalculateHeroHPDelta(actionData);
                
            }
        }
        
        private void CachePreRoundStartDatas()
        {
            foreach (var kv in BattleUnitDatas)
            {
                var actionData = new ActionData();
                actionData.ActionUnitIdx = kv.Key;
                var triggerDatas = new List<TriggerData>();


                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);

                if (triggerBuffDatas == null)
                    continue;

                //var attackWithoutHero = kv.Value.BuffCount(EBuffID.AttackWithoutHero) > 0;
                var idx = -1;
                foreach (var triggerBuffData in triggerBuffDatas)
                {
                    idx++;
                    if (triggerBuffData.BuffData.BuffTriggerType != EBuffTriggerType.PreRoundStart)
                        continue;

                    // || attackWithoutHero
                    var buffData = triggerBuffData.BuffData;
                    var range = GameUtility.GetRange(kv.Value.GridPosIdx, buffData.TriggerRange, kv.Value.UnitCamp,
                        buffData.TriggerUnitCamps, true);

                    foreach (var rangeGridPosIdx in range)
                    {
                        var unit = GetUnitByGridPosIdx(rangeGridPosIdx);
                        if (unit == null)
                            continue;

                        List<float> values = new List<float>();
  
                        values = triggerBuffData.ValueList;

                        BattleBuffManager.Instance.PreRoundStartTrigger(triggerBuffData.BuffData, values,
                            kv.Value.Idx,
                            kv.Value.Idx,
                            unit.Idx, triggerDatas);


                    }

                }

                if (triggerDatas.Count > 0)
                {
                    RoundFightData.PreRoundStartDatas.Add(kv.Key, actionData);
                    actionData.TriggerDataDict.Add(kv.Key, new TriggerCollection()
                    {
                        ActionUnitIdx = kv.Key,
                        EffectTagIdx = kv.Key,
                        TriggerDatas = triggerDatas
                    });
                }

                CalculateHeroHPDelta(actionData);
            }
        }

        private void CacheRoundStartUnitDatas()
        {
            foreach (var kv in BattleUnitDatas)
            {
                var actionData = new ActionData();
                actionData.ActionUnitIdx = kv.Key;
                var triggerDatas = new List<TriggerData>();


                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);

                if (triggerBuffDatas == null)
                    continue;

                //var attackWithoutHero = kv.Value.BuffCount(EBuffID.AttackWithoutHero) > 0;
                var idx = -1;
                foreach (var triggerBuffData in triggerBuffDatas)
                {
                    idx++;
                    if (triggerBuffData.BuffData.BuffTriggerType != EBuffTriggerType.RoundStart)
                        continue;

                    // || attackWithoutHero
                    var buffData = triggerBuffData.BuffData;
                    var range = GameUtility.GetRange(kv.Value.GridPosIdx, buffData.TriggerRange, kv.Value.UnitCamp,
                        buffData.TriggerUnitCamps, true);

                    foreach (var rangeGridPosIdx in range)
                    {
                        var unit = GetUnitByGridPosIdx(rangeGridPosIdx);
                        if (unit == null)
                            continue;

                        List<float> values = new List<float>();
                        
                        values = triggerBuffData.ValueList;

                        BattleBuffManager.Instance.RoundStartTrigger(triggerBuffData.BuffData, values,
                            kv.Value.Idx,
                            kv.Value.Idx,
                            unit.Idx, triggerDatas);


                    }

                }

                if (triggerDatas.Count > 0)
                {
                    RoundFightData.RoundStartUnitDatas.Add(kv.Key, actionData);
                    actionData.TriggerDataDict.Add(kv.Key, new TriggerCollection()
                    {
                        ActionUnitIdx = kv.Key,
                        EffectTagIdx = kv.Key,
                        TriggerDatas = triggerDatas
                    });
                }

                CalculateHeroHPDelta(actionData);
            }
        }


        private void CacheRoundStartDatas()
        {
            BattleCurseManager.Instance.CacheRoundStartDatas();
            BlessManager.Instance.CacheRoundStartDatas();

            CacheRoundStartBuffDatas();
            CacheRoundStartUnitDatas();

        }


        private void CacheRoundEndDatas()
        {
            var actionData = BattleCurseManager.Instance.CacheAttackMostUnit_Attack();
            //CalculateHeroHPDelta(actionData);
        }

        private void CacheAttackData(EUnitCamp unitCamp, List<BuffValue> triggerBuffDatas, int gridPosIdx,
            ActionData actionData, int attackUnitIdx, EBuffTriggerType buffTriggerType)
        {
            var attackUnit = GetUnitByIdx(attackUnitIdx);
            //var attackWithoutHero = attackUnit.BuffCount(EBuffID.AttackWithoutHero) > 0;
            var isTrigger = false;
            foreach (var triggerBuffData in triggerBuffDatas)
            {
                if (triggerBuffData.BuffData.BuffTriggerType != buffTriggerType)
                    continue;
                
                List<int> range;

                if (buffTriggerType == EBuffTriggerType.SelectUnit && unitCamp != EUnitCamp.Enemy)
                {
                    range = new List<int>();
                    // foreach (var buffValue in triggerBuffDatas)
                    // {
                    //     
                    // }
                    range.Add(triggerBuffData.TargetGridPosIdx);
                }
                else
                {
                    range = GameUtility.GetRange(gridPosIdx, triggerBuffData.BuffData.TriggerRange, unitCamp,
                        triggerBuffData.BuffData.TriggerUnitCamps, true);

                }

                // if (unitCamp == EUnitCamp.Enemy)
                // {
                //     //治疗型敌人会有问题
                //     if (triggerBuffData.BuffData.TriggerUnitCamps.Count > 1)
                //     {
                //         var isNotFirstUnitCamp = true;
                //         foreach (var _gridPosIdx in range)
                //         {
                //             var unit = GetUnitByGridPosIdx(_gridPosIdx);
                //             if (unit != null && GameUtility.GetRelativeCamp(EUnitCamp.Enemy, unit.UnitCamp)  ==
                //                 triggerBuffData.BuffData.TriggerUnitCamps[0])
                //             {
                //                 isNotFirstUnitCamp = false;
                //             }
                //
                //         }
                //
                //         if (isNotFirstUnitCamp)
                //         {
                //             range.Clear();
                //         }
                //     }
                // }

                // || attackWithoutHero

                var rangeContainFirstCamp = false;
                var range2 = new List<int>();
                if (unitCamp == EUnitCamp.Enemy && range.Count > 0)
                {

                    // for (int i = 0; i < range.Count; i++)
                    // {
                    //     var _gridPosIdx = range[i];
                    //     var unit = GetUnitByGridPosIdx(_gridPosIdx);
                    //     if (unit != null && unit.UnitRole == EUnitRole.Core)
                    //     {
                    //         range2.Add(_gridPosIdx);
                    //         range.Remove(_gridPosIdx);
                    //     }
                    //     
                    // }
                    
                    for (int i = 0; i < range.Count; i++)
                    {
                        var _gridPosIdx = range[i];
                        var unit = GetUnitByGridPosIdx(_gridPosIdx);
                        if (unit != null &&  GameUtility.GetRelativeCamp(unitCamp, unit.UnitCamp) == triggerBuffData.BuffData.TriggerUnitCamps[0])
                        {
                            range2.Add(_gridPosIdx);
                            range.Remove(_gridPosIdx);
                        }
                    }

                    for (int i = 0; i < range.Count; i++)
                    {
                        var _gridPosIdx = range[i];
                        if (range2.Contains(_gridPosIdx))
                            continue;
                        
                        range2.Add(_gridPosIdx);

                    }

                    // range.Sort((gridPosIdx1, gridPosIdx2) =>{
                    //     var unit1 = GetUnitByGridPosIdx(gridPosIdx1);
                    //     var unit2 = GetUnitByGridPosIdx(gridPosIdx2);
                    //
                    //     if (unit1 == null)
                    //         return 0;
                    //     
                    //     if (unit2 == null)
                    //         return 0;
                    //     
                    //     if (unit2.UnitRole == EUnitRole.Hero)
                    //         return 1;
                    //
                    //     return 0;
                    // });

                    // foreach (var rangeGridPosIdx in range)
                    // {
                    //     var unit = GetUnitByGridPosIdx(rangeGridPosIdx);
                    //     if(unit == null)
                    //         continue;
                    //     var relativeCamp = GameUtility.GetRelativeCamp(attackUnit.UnitCamp, unit.UnitCamp);
                    //     if (relativeCamp == triggerBuffData.BuffData.TriggerUnitCamps[0])
                    //     {
                    //         rangeContainFirstCamp = true;
                    //     }
                    // }


                }
                else
                {
                    range2 = range;
                }


                var isSubCurHP = false;





                // if (unitCamp == EUnitCamp.Player1 || unitCamp == EUnitCamp.Player2 || rangeContainFirstCamp)
                // {
                var triggerRangeStr = triggerBuffData.BuffData.TriggerRange.ToString();
                var directs = new List<ERelativePos>();
                foreach (var rangeGridPosIdx in range2)
                {
                    if (triggerRangeStr.Contains("Parabola") || triggerRangeStr.Contains("Long"))
                    {
                        var direct = GameUtility.GetRelativePos(gridPosIdx, rangeGridPosIdx);
                        if (direct != null)
                        {
                            if (directs.Contains((ERelativePos)direct))
                            {
                                continue;
                            }

                            directs.Add((ERelativePos)direct);
                        }

                    }

                    var effectUnit = GetUnitByGridPosIdx(rangeGridPosIdx);
                    if (effectUnit == null)
                        continue;

                    if (!effectUnit.Exist())
                        continue;

                    if (!triggerBuffData.BuffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy) &&
                        effectUnit.UnitCamp != attackUnit.UnitCamp)
                    {
                        continue;
                    }

                    if (!triggerBuffData.BuffData.TriggerUnitCamps.Contains(ERelativeCamp.Us) &&
                        effectUnit.UnitCamp == attackUnit.UnitCamp)
                    {
                        continue;
                    }

                    var triggerDatas = new List<TriggerData>();
                    if (!actionData.TriggerDataDict.ContainsKey(effectUnit.Idx))
                    {
                        actionData.TriggerDataDict.Add(effectUnit.Idx, new TriggerCollection()
                        {
                            ActionUnitIdx = attackUnitIdx,
                            EffectTagIdx = effectUnit.Idx,
                            TriggerDatas = triggerDatas
                        });
                    }
                    else
                    {
                        triggerDatas = actionData.TriggerDataDict[effectUnit.Idx].TriggerDatas;

                    }

                    var _triggerDatas = BattleBuffManager.Instance.BuffTrigger(buffTriggerType,
                        triggerBuffData.BuffData,
                        triggerBuffData.ValueList, attackUnitIdx,
                        attackUnitIdx,
                        effectUnit.Idx, triggerDatas);

                    

                    foreach (var triggerData in _triggerDatas)
                    {
                        // triggerData.BuffValue = triggerBuffData.Copy();
                        // if (GameUtility.IsSubCurHPTrigger(triggerData))
                        // {
                        //     //isSubCurHP = true;
                        //     BattleBuffManager.Instance.AttackTrigger(triggerDatas[0], triggerDatas);
                        //     BattleUnitStateManager.Instance.CheckUnitState(attackUnitIdx, triggerDatas);
                        // }

                        CacheUnitActiveMoveDatas(attackUnitIdx, rangeGridPosIdx, triggerBuffData.BuffData, actionData,
                            triggerData);
                    }
                    
                    

                    isTrigger = true;
                    if (unitCamp == EUnitCamp.Enemy &&
                        triggerBuffData.BuffData.BuffTriggerType == EBuffTriggerType.SelectUnit)
                    {
                        var attackUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(attackUnitIdx);
                        if (attackUnitEntity != null)
                        {
                            attackUnitEntity.TargetPosIdx = rangeGridPosIdx;
                        }

                        break;
                    }

                    // && triggerBuffData.BuffData.FlyRange
                    // if(unitCamp == EUnitCamp.Enemy)
                    //     break;
                    
                }
                //}


                // if (isSubCurHP)
                // {
                //     var triggerDatas = actionData.TriggerDatas.Values.ToList()[actionData.TriggerDatas.Count - 1];
                //     BattleBuffManager.Instance.AttackTrigger(triggerDatas[0], triggerDatas);
                //     BattleUnitStateManager.Instance.CheckUnitState(attackUnitID, triggerDatas);
                // }
            }

            for (int j = actionData.TriggerDataDict.Values.Count -1; j >= 0; j--)
            {
                var keys = actionData.TriggerDataDict.Keys.ToList();
                var triggerDatas = actionData.TriggerDataDict.Values.ToList()[j].TriggerDatas;
                for (int i = triggerDatas.Count -1; i >= 0; i--)
                {
                    var triggerData = triggerDatas[i];
                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        var _attackUnit = GameUtility.GetUnitDataByIdx(triggerData.ActionUnitIdx);
                        if (_attackUnit != null)
                        {
                            SubUnitState(_attackUnit, EUnitState.SubDmg, actionData.TriggerDataDict[keys[j]].TriggerDatas);
                            SubUnitState(_attackUnit, EUnitState.AddDmg, actionData.TriggerDataDict[keys[j]].TriggerDatas);
                        }
                        
                    }
                }
            }

            for (int l = actionData.TriggerDataDict.Values.Count -1 ; l >= 0; l--)
            {
                var triggerCollection = actionData.TriggerDataDict.Values.ToList()[l];
                for (int k = triggerCollection.MoveData.MoveUnitDatas.Values.Count -1; k >= 0; k--)
                {

                    var moveUnitData = triggerCollection.MoveData.MoveUnitDatas.Values.ToList()[k];
                    for (int i = moveUnitData.MoveActionData.TriggerDataDict.Count -1; i >= 0; i--)
                    {
                        var keys = moveUnitData.MoveActionData.TriggerDataDict.Keys.ToList();
                        var moveTriggerCollection = moveUnitData.MoveActionData.TriggerDataDict.Values.ToList()[i];
                        for (int j = moveTriggerCollection.TriggerDatas.Count -1; j >= 0; j--)
                        {
                            var triggerData = moveTriggerCollection.TriggerDatas[j];
                            if (GameUtility.IsSubCurHPTrigger(triggerData))
                            {
                                var _attackUnit = GameUtility.GetUnitDataByIdx(triggerData.ActionUnitIdx);
                                if (_attackUnit != null)
                                {
                                    SubUnitState(_attackUnit, EUnitState.SubDmg, moveUnitData.MoveActionData.TriggerDataDict[keys[i]].TriggerDatas);
                                    SubUnitState(_attackUnit, EUnitState.AddDmg, moveUnitData.MoveActionData.TriggerDataDict[keys[i]].TriggerDatas);
                                }
                        
                            }
                        }
                    }
                }
            }
            
            

            // if (isTrigger)
            // {
            //     SubUnitState(attackUnit, EUnitState.SubDmg, actionData.TriggerDatas);
            //     SubUnitState(attackUnit, EUnitState.AddDmg, actionData.TriggerDatas);
            // }

            foreach (var kv3 in actionData.TriggerDataDict)
            {
                foreach (var kv in kv3.Value.MoveData.MoveUnitDatas)
                {
                    // kv.Value.MoveActionData.InterrelatedActionUnitIdx = attackUnitIdx;
                    // kv.Value.MoveActionData.InterrelatedEffectUnitIdx = kv3.Key;
                
                    foreach (var kv2 in kv.Value.MoveActionData.TriggerDataDict)
                    {
                        foreach (var data in kv2.Value.TriggerDatas)
                        {
                            data.InterrelatedActionUnitIdx = attackUnitIdx;
                            data.InterrelatedEffectUnitIdx = kv3.Key;
                        }
                    }
                }
            }

            
        }


        public void CacheUnitActiveMoveDatas(int actionUnitIdx, int effectGridPosIdx, BuffData buffData,
            ActionData actionData, TriggerData triggerData, int actionGridPosIdx = 0)
        {
            if (effectGridPosIdx == -1)
                return;

            var actionUnitCamp = EUnitCamp.Player1;
            int _actionGridPosIdx;
            var actionUnit = GetUnitByIdx(actionUnitIdx);
            if (actionUnit == null)
            {
                actionUnitCamp = EUnitCamp.Player1;
                _actionGridPosIdx = actionGridPosIdx;
            }
            else
            {
                actionUnitCamp = actionUnit.UnitCamp;
                _actionGridPosIdx = actionUnit.GridPosIdx;
            }
                

            
            var effectUnit = GetUnitByGridPosIdx(effectGridPosIdx);
            
            
            if (effectUnit == null && !Constant.Battle.RelatedUnitFlyRanges.Contains(buffData.FlyRange))
                return;

            if (effectUnit != null)
            {
                var relativeCamp = GameUtility.GetRelativeCamp(actionUnitCamp, effectUnit.UnitCamp);
                if (!buffData.TriggerUnitCamps.Contains(relativeCamp))
                    return;
            }
            
            
            if (!actionData.TriggerDataDict.ContainsKey(effectUnit.Idx))
            {
                actionData.TriggerDataDict.Add(effectUnit.Idx, new TriggerCollection());
            }

            var actionTriggerCollection = actionData.TriggerDataDict[effectUnit.Idx];

            actionTriggerCollection.ActionUnitIdx = actionUnitIdx;
            actionTriggerCollection.EffectTagIdx = effectUnit.Idx;

            var actionUnitCoord = GameUtility.GridPosIdxToCoord(_actionGridPosIdx);


            var flyDirect = Vector2Int.zero;
            List<int> flyPaths;
            List<int> newFlyPaths = new List<int>();
            var moveActionDatas = new Dictionary<int, MoveActionData>();

            //var buffData = BattleBuffManager.Instance.GetBuffData(buffID);


            // if (effectUnit == null &&
            //     (buffData.FlyType != EFlyType.SelfPass))
            //     return;

            var effectUnitCoord = GameUtility.GridPosIdxToCoord(effectGridPosIdx);
            var moveUnitIdx = -1;
            var dis = 1;
            if (actionUnit != null)
            {
                if (actionUnit.FuneCount(EBuffID.Spec_AddFlyRange) > 0)
                {
                    dis = 99;
                }
            }

            if (buffData.FlyType == EFlyType.Exchange && actionUnit != null)
            {
                if(actionUnit.Idx == effectUnit.Idx)
                    return;
                
                // if(actionTriggerCollection.MoveData.MoveUnitDatas.ContainsKey(actionUnit.Idx))
                //     return;
                
                actionTriggerCollection.MoveData.MoveUnitDatas.Add(actionUnit.Idx, new MoveUnitData()
                {
                    ActionUnitIdx = actionUnitIdx,
                    UnitIdx = actionUnit.Idx,
                    MoveActionData = new MoveActionData()
                    {
                        MoveUnitIdx = actionUnit.Idx,
                        MoveGridPosIdxs = new List<int>()
                        {
                            actionUnit.GridPosIdx,
                            effectUnit.GridPosIdx,
                        }
                    },
                    UnitActionState = EUnitActionState.Fly,
                });
                
                // flyDirect = effectUnitCoord - actionUnitCoord;
                // CacheUnitFlyMoveDatas(actionUnit.Idx, flyDirect, dis, moveActionDatas, actionData, triggerData,
                //     actionUnit.GridPosIdx, EUnitActionState.Fly);
                
                

                actionTriggerCollection.MoveData.MoveUnitDatas.Add(effectUnit.Idx, new MoveUnitData()
                {
                    ActionUnitIdx = actionUnitIdx,
                    UnitIdx = effectUnit.Idx,
                    MoveActionData = new MoveActionData()
                    {
                        MoveUnitIdx = effectUnit.Idx,
                        MoveGridPosIdxs = new List<int>()
                        {
                            effectUnit.GridPosIdx,
                            actionUnit.GridPosIdx,
                        }
                    },
                    UnitActionState = EUnitActionState.Fly,
                });
                // flyDirect = actionUnitCoord - effectUnitCoord;
                // CacheUnitFlyMoveDatas(effectUnit.Idx, flyDirect, dis, moveActionDatas, actionData, triggerData,
                //     effectGridPosIdx, EUnitActionState.Fly);
                
                var actionUnitGridPosIdx = actionUnit.GridPosIdx;
                actionUnit.GridPosIdx = effectUnit.GridPosIdx;
                effectUnit.GridPosIdx = actionUnitGridPosIdx;
                
            }
            else if (Constant.Battle.RelatedUnitFlyRanges.Contains(buffData.FlyRange) ||
                     Constant.Battle.DynamicRelatedUnitFlyRanges.Contains(buffData.FlyRange))
            {
                var relatedUnits = GetUnitByGridPosIdx(_actionGridPosIdx, effectGridPosIdx, buffData.FlyRange);
                foreach (var relatedUnit in relatedUnits)
                {
                    var relatedUnitCoord = GameUtility.GridPosIdxToCoord(relatedUnit.GridPosIdx);
                    flyDirect = buffData.FlyType == EFlyType.Back
                        ? relatedUnitCoord - effectUnitCoord
                        : effectUnitCoord - relatedUnitCoord;
                    flyPaths = GetFlyPaths(relatedUnit.GridPosIdx, flyDirect, dis, EUnitActionState.Fly);
                    CacheUnitMoveDatas(relatedUnit.Idx, flyPaths, moveActionDatas, EUnitActionState.Fly);

                    

                    if (!actionTriggerCollection.MoveData.MoveUnitDatas.ContainsKey(relatedUnit.Idx) &&
                        moveActionDatas.ContainsKey(relatedUnit.Idx))
                    {
                        actionTriggerCollection.MoveData.MoveUnitDatas.Add(relatedUnit.Idx, new MoveUnitData()
                        {
                            ActionUnitIdx = triggerData.ActionUnitIdx,
                            EffectGridPosIdx = effectGridPosIdx,
                            UnitIdx = relatedUnit.Idx,
                            MoveActionData = moveActionDatas[relatedUnit.Idx],
                            UnitActionState = EUnitActionState.Fly,
                        });

                    }
                }
            }
            else if (buffData.FlyRange == EActionType.LineExtend)
            {

                flyDirect = effectUnitCoord - actionUnitCoord;

                flyPaths = GetFlyPaths(effectUnit.GridPosIdx, flyDirect, dis, EUnitActionState.Fly);
                CacheUnitMoveDatas(effectUnit.Idx, flyPaths, moveActionDatas, EUnitActionState.Fly);

                if (!actionTriggerCollection.MoveData.MoveUnitDatas.ContainsKey(effectUnit.Idx) &&
                    moveActionDatas.ContainsKey(effectUnit.Idx))
                {
                    actionTriggerCollection.MoveData.MoveUnitDatas.Add(effectUnit.Idx, new MoveUnitData()
                    {
                        ActionUnitIdx = triggerData.ActionUnitIdx,
                        EffectGridPosIdx = effectGridPosIdx,
                        UnitIdx = effectUnit.Idx,
                        MoveActionData = moveActionDatas[effectUnit.Idx],
                        UnitActionState = EUnitActionState.Fly,
                    });

                }

                var relatedUnits =
                    GameUtility.GetRelatedCoords(buffData.FlyRange, _actionGridPosIdx, effectGridPosIdx);
                if (relatedUnits.Count > 0)
                {
                    var relatedUnit = GetUnitByGridPosIdx(GameUtility.GridCoordToPosIdx(relatedUnits[0]));
                    if (relatedUnit != null)
                    {
                        var relatedUnitCoord = GameUtility.GridPosIdxToCoord(relatedUnit.GridPosIdx);
                        flyDirect = relatedUnitCoord - actionUnitCoord;

                        flyPaths = GetFlyPaths(relatedUnit.GridPosIdx, flyDirect, dis, EUnitActionState.Fly);
                        CacheUnitMoveDatas(relatedUnit.Idx, flyPaths, moveActionDatas, EUnitActionState.Fly);

                        if (!actionTriggerCollection.MoveData.MoveUnitDatas.ContainsKey(relatedUnit.Idx) &&
                            moveActionDatas.ContainsKey(relatedUnit.Idx))
                        {
                            actionTriggerCollection.MoveData.MoveUnitDatas.Add(relatedUnit.Idx, new MoveUnitData()
                            {
                                ActionUnitIdx = triggerData.ActionUnitIdx,
                                EffectGridPosIdx = effectGridPosIdx,
                                UnitIdx = relatedUnit.Idx,
                                MoveActionData = moveActionDatas[relatedUnit.Idx],
                                UnitActionState = EUnitActionState.Fly,
                            });

                        }
                    }

                }



            }
            else
            {
                var unitActionState = EUnitActionState.Fly;
                if (buffData.FlyRange == EActionType.Self && actionUnit != null)
                {
                    if (buffData.FlyType == EFlyType.Back)
                    {
                        flyDirect = actionUnitCoord - effectUnitCoord;
                    }
                    else if (buffData.FlyType == EFlyType.Close)
                    {
                        dis = 99;
                        flyDirect = effectUnitCoord - actionUnitCoord;
                        unitActionState = EUnitActionState.Rush;
                    }

                    moveUnitIdx = actionUnit.Idx;

                }
                else if (buffData.FlyRange == EActionType.Other)
                {
                    if (buffData.FlyType == EFlyType.Back)
                    {
                        flyDirect = effectUnitCoord - actionUnitCoord;
                    }
                    else if (buffData.FlyType == EFlyType.Close)
                    {
                        dis = 99;
                        flyDirect = actionUnitCoord - effectUnitCoord;
                    }

                    moveUnitIdx = effectUnit.Idx;
                }
                else if (buffData.FlyType == EFlyType.SelfPass && actionUnit != null)
                {
                    dis = 99;
                    flyDirect = effectUnitCoord - actionUnitCoord;
                    moveUnitIdx = actionUnit.Idx;
                    unitActionState = EUnitActionState.Throw;
                }
                else if (buffData.FlyType == EFlyType.BackToSelf)
                {
                    dis = 99;
                    flyDirect = actionUnitCoord - effectUnitCoord;
                    moveUnitIdx = effectUnit.Idx;
                    unitActionState = EUnitActionState.Throw;
                }

                CacheUnitFlyMoveDatas(moveUnitIdx, flyDirect, dis, moveActionDatas, actionData, triggerData,
                    effectGridPosIdx, unitActionState);

                // if (buffData.FlyType == EFlyType.Rush)
                // {
                //     CacheUnitFlyMoveDatas(actionUnit.Idx, effectUnitCoord - actionUnitCoord, 99, moveActionDatas, actionData, triggerData,
                //         effectGridPosIdx, EUnitActionState.Rush);
                //     
                //     // CacheUnitFlyMoveDatas(effectUnit.Idx, effectUnitCoord - actionUnitCoord, 1, moveActionDatas, actionData, triggerData,
                //     //     effectGridPosIdx, EUnitActionState.Fly);
                // }




            }

            
            //CalculateUnitPaths(EUnitCamp.Third, RoundFightData.ThirdUnitMovePaths);
        }


        private void CacheUnitFlyMoveDatas(int moveUnitIdx, Vector2Int flyDirect, int dis,
            Dictionary<int, MoveActionData> moveActionDatas, ActionData actionData, TriggerData triggerData,
            int effectGridPosIdx, EUnitActionState unitActionState)
        {
            
            TriggerCollection actionTriggerCollection;
            if (!actionData.TriggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
            {
                actionData.TriggerDataDict.Add(triggerData.EffectUnitIdx, new TriggerCollection()
                {
                    ActionUnitIdx = triggerData.ActionUnitIdx,
                    EffectTagIdx = triggerData.EffectUnitIdx,
                });
            }

            actionTriggerCollection = actionData.TriggerDataDict[triggerData.EffectUnitIdx];
            
            var moveUnit = GetUnitByIdx(moveUnitIdx);
            if (moveUnit != null)
            {
                var flyPaths = GetFlyPaths(moveUnit.GridPosIdx, flyDirect, dis, unitActionState);

                CacheUnitMoveDatas(moveUnitIdx, flyPaths, moveActionDatas, unitActionState);

                if (moveActionDatas.Count > 0)
                {
                    actionTriggerCollection.MoveData.MoveUnitDatas.Add(moveUnitIdx, new MoveUnitData()
                    {
                        ActionUnitIdx = triggerData.ActionUnitIdx,
                        EffectGridPosIdx = effectGridPosIdx,
                        UnitIdx = moveUnitIdx,
                        MoveActionData = moveActionDatas[moveUnitIdx],
                        UnitActionState = unitActionState,
                    });

                }
            }
        }

        private void CacheSoliderAttackDatas()
        {
            CacheLinks();
            foreach (var kv in BattleUnitDatas)
            {
                var soliderData = kv.Value as Data_BattleSolider;
                if (soliderData == null)
                    continue;

                if (!kv.Value.Exist())
                    continue;

                //kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
                if (kv.Value.GetAllStateCount(EUnitState.UnAtk) > 0 &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;

                var actionData = new ActionData();
                actionData.ActionUnitIdx = soliderData.Idx;
                RoundFightData.SoliderAttackDatas.Add(soliderData.Idx, actionData);

                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);
                CacheAttackData(BattleManager.Instance.CurUnitCamp, triggerBuffDatas, kv.Value.GridPosIdx, actionData,
                    soliderData.Idx, EBuffTriggerType.ActionEnd);
                CalculateHeroHPDelta(actionData);
            }

        }

        private void CacheEnemyAttackData(Data_BattleMonster enemyData)
        {
            if (enemyData == null)
                return;

            if (!enemyData.Exist())
                return;

            if ((enemyData.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return;

            ActionData actionData;
            if (RoundFightData.EnemyAttackDatas.ContainsKey(enemyData.Idx))
            {
                actionData = RoundFightData.EnemyAttackDatas[enemyData.Idx];
            }
            else
            {
                actionData = new ActionData();
                actionData.ActionUnitIdx = enemyData.Idx;
                RoundFightData.EnemyAttackDatas.Add(enemyData.Idx, actionData);
            }


            var drEnemy = GameEntry.DataTable.GetEnemy(enemyData.MonsterID);
            var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);

            var attackRange = GameUtility.GetRange(enemyData.GridPosIdx, buffData.TriggerRange, EUnitCamp.Enemy,
                buffData.TriggerUnitCamps, true);

            // if (!attackRange.Contains(RoundFightData.GamePlayData.PlayerData.BattleHero.GridPosIdx))
            // {
            //     return;
            // }

            if (attackRange.Count > 0)
            {
                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, enemyData,
                    out List<BuffValue> triggerBuffDatas);
                CacheAttackData(EUnitCamp.Enemy, triggerBuffDatas, enemyData.GridPosIdx, actionData, enemyData.Idx,
                    EBuffTriggerType.ActionEnd);
                CacheAttackData(EUnitCamp.Enemy, triggerBuffDatas, enemyData.GridPosIdx, actionData, enemyData.Idx,
                    EBuffTriggerType.SelectUnit);

                

            }
            // else
            // {
            //     BattleUnitManager.Instance.GetSecondaryBuffValue(RoundFightData.GamePlayData, enemyData,
            //         out List<BuffValue> secondaryTriggerBuffDatas);
            //     CacheAttackData(EUnitCamp.Enemy, secondaryTriggerBuffDatas, enemyData.GridPosIdx, actionData, enemyData.Idx, EBuffTriggerType.ActionEnd);
            // }



            CalculateHeroHPDelta(actionData);
        }

        
        public void CacheEnemyAttackDatas()
        {

            var enemies = new List<Data_BattleUnit>();
            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != EUnitCamp.Enemy)
                    continue;

                if (!kv.Value.Exist())
                    continue;
                
                if (!(kv.Value as Data_BattleMonster).IsRoundStart)
                    continue;

                enemies.Add(kv.Value);
            }

            enemies.Sort((e1, e2) => { return e1.GridPosIdx - e2.GridPosIdx; });

            foreach (var enemy in enemies)
            {
                CacheEnemyAttackData(enemy as Data_BattleMonster);
            }

        }


        private void CacheSoliderActiveAttackDatas()
        {
            var unitData = RoundFightData.TempTriggerData.UnitData;

            if (unitData == null)
                return;

            // if (effectUnitData == null)
            //     return;

            RoundFightData.SoliderActiveAttackDatas.Clear();
            if (!unitData.Exist())
                return;

            if (RoundFightData.TempTriggerData.TriggerType != ETempTriggerType.ActiveAtk)
                return;

            // if (unitData.GetStateCount(EUnitState.ActiveAtk) <= 0)
            //     return;

            //soliderData.GetAllStateCount(EUnitState.UnAction) > 0 ||
            if ((unitData.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return;

            var actionData = new ActionData();
            actionData.ActionUnitIdx = unitData.Idx;
            RoundFightData.SoliderActiveAttackDatas.Add(unitData.Idx, actionData);

            BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, unitData,
                out List<BuffValue> triggerBuffDatas, RoundFightData.TempTriggerData.TargetGridPosIdx);
            // CacheUnitActiveAttackData(BattleManager.Instance.CurUnitCamp, triggerBuffDatas, unitData.GridPosIdx,
            //     actionData,
            //     unitData.Idx, RoundFightData.TempTriggerData.TargetGridPosIdx);
            CacheAttackData(BattleManager.Instance.CurUnitCamp, triggerBuffDatas, unitData.GridPosIdx,
                actionData,
                unitData.Idx, EBuffTriggerType.SelectUnit);

            CalculateHeroHPDelta(actionData);
        }

        private void CacheSoliderAutoAttackDatas()
        {
            var unitData = RoundFightData.TempTriggerData.UnitData;

            if (unitData == null)
                return;

            if (!GameUtility.CheckUnitCamp(new List<ERelativeCamp>()
                {
                    ERelativeCamp.Us
                }, BattleManager.Instance.CurUnitCamp, unitData.UnitCamp))
            {
                return;
            }

            if (RoundFightData.TempTriggerData.TriggerType != ETempTriggerType.AutoAtk)
                return;

            // if (unitData.GetStateCount(EUnitState.AutoAtk) <= 0)
            //     return;

            if (!unitData.Exist())
                return;

            if ((unitData.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return;

            var actionData = new ActionData();
            actionData.ActionUnitIdx = unitData.Idx;
            RoundFightData.SoliderActiveAttackDatas.Add(unitData.Idx, actionData);

            BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, unitData,
                out List<BuffValue> triggerBuffDatas);
            // CacheUnitAutoAttackDatas(BattleManager.Instance.CurUnitCamp, triggerBuffDatas, unitData.GridPosIdx, actionData,
            //     unitData.Idx);

            CacheAttackData(BattleManager.Instance.CurUnitCamp, triggerBuffDatas, unitData.GridPosIdx, actionData,
                unitData.Idx, EBuffTriggerType.AutoAttack);
            CalculateHeroHPDelta(actionData);
        }

        public List<int> CacheUnitMoveDatas(int unitIdx, List<int> movePaths,
            Dictionary<int, MoveActionData> unitMoveDatas, EUnitActionState unitActionState)
        {
            var passUnit = GetUnitByIdx(unitIdx);
            if (passUnit == null)
                return null;
            if (!passUnit.Exist())
                return null;

            // var battlePlayerData = RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance
            //     .CurUnitCamp);
            //passUnit.GetAllStateCount(EUnitState.UnAction) > 0 ||
            if ((passUnit.GetAllStateCount(EUnitState.UnMove) > 0) &&
                !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return null;

            var moveActionData = new MoveActionData();
            moveActionData.MoveUnitIdx = passUnit.Idx;
            unitMoveDatas.Add(passUnit.Idx, moveActionData);

            // var tempFirstEnemyActionSoliders = new List<int>();
            // var hpRefreshTriggerTypeSoliders = new List<int>();


            var minFullPaths = new List<int>();

            if (movePaths.Count > 0)
            {
                passUnit.GridPosIdx = movePaths[0];
            }



            for (int i = 0; i < movePaths.Count; i++)
            {
                var preGridPosIdx = passUnit.GridPosIdx;
                var gridPosIdx = movePaths[i];
                var nextGridPosIdx = -1;
                if (i + 1 < movePaths.Count)
                {
                    nextGridPosIdx = movePaths[i + 1];
                }

                passUnit.GridPosIdx = gridPosIdx;
                CacheLinks();

                var triggerDatas = new List<TriggerData>();



                // if (moveUnitCamp == EUnitCamp.Enemy)
                // {
                //     TriggerPassAuxiliary(gridPosIdx, unitID, triggerDatas);
                // }
                var bePassUnit = GetUnitByGridPosIdx(gridPosIdx, null, null, null, unitIdx);

                if (unitActionState != EUnitActionState.Throw)
                {
                    List<MoveUnitStateData> preMoveTriggerDatas = null;

                    if (nextGridPosIdx != preGridPosIdx)
                    {
                        preMoveTriggerDatas = MoveTrigger(i, passUnit, bePassUnit, triggerDatas);
                    }


                    var hurtEachMoveCount = passUnit.GetAllStateCount(EUnitState.HurtEachMove);
                    if (hurtEachMoveCount > 0 && i > 0)
                    {
                        if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                        {
                            var hurtEachMoveTriggerData = BattleFightManager.Instance.BattleRoleAttribute(passUnit.Idx,
                                passUnit.Idx,
                                passUnit.Idx,
                                EUnitAttribute.HP, -hurtEachMoveCount, ETriggerDataSubType.Unit);
                            hurtEachMoveTriggerData.UnitStateDetail.UnitState = EUnitState.HurtEachMove;
                            SimulateTriggerData(hurtEachMoveTriggerData, triggerDatas);
                            triggerDatas.Add(hurtEachMoveTriggerData);
                        }

                        passUnit.RemoveState(EUnitState.HurtEachMove);
                    }

                    if (bePassUnit != null && nextGridPosIdx != preGridPosIdx)
                    {
                        BattleUnitStateManager.Instance.HurtRoundStartMoveTrigger(passUnit.Idx, bePassUnit.Idx,
                            triggerDatas);

                        // BattleBuffManager.Instance.BuffsTrigger(unit, triggerDatas, ETriggerType.Pass);
                        // BattleBuffManager.Instance.BuffsTrigger(bePassUnit, triggerDatas, ETriggerType.BePass);

                        BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, passUnit,
                            out List<BuffValue> passUnitTriggerBuffDatas);

                        if (passUnitTriggerBuffDatas != null)
                        {
                            foreach (var triggerBuffData in passUnitTriggerBuffDatas)
                            {
                                BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.Pass,
                                    triggerBuffData.BuffData, triggerBuffData.ValueList, unitIdx, unitIdx,
                                    bePassUnit.Idx,
                                    triggerDatas, gridPosIdx, preGridPosIdx);
                            }
                        }

                        BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, bePassUnit,
                            out List<BuffValue> bePassUnitTriggerBuffDatas);

                        if (bePassUnitTriggerBuffDatas != null)
                        {
                            var idx = 0;
                            foreach (var triggerBuffData in bePassUnitTriggerBuffDatas)
                            {
                                BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.BePass,
                                    triggerBuffData.BuffData, triggerBuffData.ValueList, bePassUnit.Idx, bePassUnit.Idx,
                                    passUnit.Idx,
                                    triggerDatas, gridPosIdx, preGridPosIdx);
                                idx++;
                            }
                        }
                    }

                    var gridProp = BattleGridPropManager.Instance.GetGridProp(gridPosIdx);
                    if (gridProp != null)
                    {
                        var drGridProp = GameEntry.DataTable.GetGridProp(gridProp.GridPropID);
                        var idx = 0;

                        foreach (var buffIDStr in drGridProp.GridPropIDs)
                        {
                            var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                            BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.BePass,
                                buffData, BattleGridPropManager.Instance.GetValues(gridProp.GridPropID, idx),
                                Constant.Battle.UnUnitTriggerIdx, Constant.Battle.UnUnitTriggerIdx, unitIdx,
                                triggerDatas, gridPosIdx, preGridPosIdx, -1, -1, ETriggerDataSubType.Prop);
                            idx++;
                        }
                    }

                    if (nextGridPosIdx != preGridPosIdx)
                    {
                        MoveTrigger(i, passUnit, bePassUnit, triggerDatas, preMoveTriggerDatas);
                    }


                    TriggerUnitData(unitIdx, bePassUnit == null ? passUnit.Idx : bePassUnit.Idx, gridPosIdx,
                        EBuffTriggerType.Move, triggerDatas);

                }

                //var isPassTrigger = false;


                //CacheUnitRangeDatas(passUnit.Idx, preGridPosIdx, gridPosIdx, triggerDatas);

                if (nextGridPosIdx == preGridPosIdx)
                {
                    var collideHurt = GetCollisionHurt(unitActionState);
                    var bePassUnitIdx = -1;
                    if (bePassUnit != null)
                    {
                        var firstCollideUnHurt = GamePlayManager.Instance.GamePlayData.GetUsefulBless(
                            EBlessID.FirstCollideUnHurt,
                            bePassUnit.UnitCamp);
                        var collideUnDead = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.CollideUnDead,
                            bePassUnit.UnitCamp);

                        if (!(firstCollideUnHurt != null && bePassUnit.CollideCount == 0) &&
                            !(collideUnDead != null && bePassUnit.CurHP <= Mathf.Abs(collideHurt)))
                        {
                            bePassUnitIdx = bePassUnit.Idx;
                            var collisionTriggerData = BattleFightManager.Instance.BattleRoleAttribute(unitIdx, unitIdx,
                                bePassUnit.Idx, EUnitAttribute.HP, collideHurt, ETriggerDataSubType.Collision);
                            collisionTriggerData.ActionUnitGridPosIdx = gridPosIdx;
                            collisionTriggerData.EffectUnitGridPosIdx = gridPosIdx;

                            if (BattleCoreManager.Instance.IsCoreIdx(bePassUnit.Idx))
                            {
                                collisionTriggerData.ChangeHPInstantly = false;
                            }

                            CollideTrigger(collisionTriggerData, triggerDatas);
                            BattleBuffManager.Instance.CacheTriggerData(collisionTriggerData, triggerDatas);
                        }

                        BattleUnitStateManager.Instance.HurtRoundStartMoveTrigger(passUnit.Idx, bePassUnit.Idx,
                            triggerDatas);


                        // var bePassUnitHeroEntity = HeroManager.Instance.GetHeroEntity(bePassUnit.UnitCamp);
                        // if (bePassUnitHeroEntity != null && bePassUnitHeroEntity.BattleHeroEntityData.BattleHeroData.Idx == bePassUnit.Idx)
                        // {
                        //     collisionTriggerData.ChangeHPInstantly = false;
                        // }

                    }

                    var firstCollideUnHurt2 = GamePlayManager.Instance.GamePlayData.GetUsefulBless(
                        EBlessID.FirstCollideUnHurt,
                        passUnit.UnitCamp);
                    var collideUnDead2 = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.CollideUnDead,
                        passUnit.UnitCamp);
                    if (!(firstCollideUnHurt2 != null && passUnit.CollideCount == 0) &&
                        !(collideUnDead2 != null && passUnit.CurHP <= Mathf.Abs(collideHurt)))
                    {
                        var collisionTriggerData2 = BattleFightManager.Instance.BattleRoleAttribute(bePassUnitIdx,
                            bePassUnitIdx,
                            unitIdx, EUnitAttribute.HP, collideHurt, ETriggerDataSubType.Collision);
                        collisionTriggerData2.ActionUnitGridPosIdx = gridPosIdx;
                        collisionTriggerData2.EffectUnitGridPosIdx = gridPosIdx;

                        // var passUnitHeroEntity = HeroManager.Instance.GetHeroEntity(passUnit.UnitCamp);
                        // if (passUnitHeroEntity!= null && passUnitHeroEntity.BattleHeroEntityData.BattleHeroData.Idx == passUnit.Idx)
                        // {
                        //     collisionTriggerData2.ChangeHPInstantly = false;
                        // }

                        if (BattleCoreManager.Instance.IsCoreIdx(passUnit.Idx))
                        {
                            collisionTriggerData2.ChangeHPInstantly = false;
                        }

                        BattleBuffManager.Instance.CacheTriggerData(collisionTriggerData2, triggerDatas);
                    }




                }

                if (i == 1)
                {
                    StartMoveTrigger(passUnit, triggerDatas);
                }

                if (triggerDatas.Count > 0)
                {
                    moveActionData.TriggerDataDict.Add(i, new TriggerCollection()
                    {
                        ActionUnitIdx = passUnit != null ? passUnit.Idx : -1,
                        EffectTagIdx = gridPosIdx,//bePassUnit != null ? bePassUnit.Idx : -1,
                        TriggerDatas = triggerDatas
                    });
                }

                minFullPaths.Add(gridPosIdx);

                if (!passUnit.Exist() && nextGridPosIdx != preGridPosIdx)
                {
                    //i为终点，表现上，敌人行动终点和我方单位重合，所以，让敌人多走一格，绕过我方单位
                    // if (i == movePaths.Count - 1)
                    // {
                    //     minFullPaths.Add(movePaths[i + 1]);
                    // }
                    break;
                }

                // if ((passUnit.GetAllStateCount(EUnitState.UnAction) > 0 ||
                //      passUnit.GetAllStateCount(EUnitState.UnMove) > 0) &&
                //     !GameUtility.ContainRoundState(RoundFightData.GamePlayData, ECardID.DeBuffUnEffect))
                //     break;



            }

            CalculateHeroHPDelta(moveActionData);
            
            moveActionData.MoveGridPosIdxs = new List<int>(minFullPaths);
            // if (tempFirstEnemyActionSoliders.Count > 0)
            // {
            //     firstUnitActionSoliders.AddRange(tempFirstEnemyActionSoliders);
            // }

            // if (minFullPaths.Count > 0)
            // {
            //     passUnit.GridPosIdx = minFullPaths[minFullPaths.Count - 1];
            //     RoundFightData.GamePlayData.BattleData.GridTypes[minFullPaths[0]] = EGridType.Empty;
            //     RoundFightData.GamePlayData.BattleData.GridTypes[minFullPaths[minFullPaths.Count - 1]] = EGridType.Unit;
            //
            //     RefreshUnitGridPosIdx();
            //     if (passUnit.UnitCamp == EUnitCamp.Enemy && unitActionState == EUnitActionState.Run)
            //     {
            //         if (!RoundFightData.EnemyAttackDatas.ContainsKey(passUnit.Idx))
            //         {
            //             CacheEnemyAttackData(passUnit as Data_BattleMonster);
            //         }
            //
            //     }
            // }

            // foreach (var kv in unitMoveDatas)
            // {
            //     CalculateHeroHPDelta(kv.Value);
            // }



            return minFullPaths;
        }


        private void CacheSoliderMoveDatas()
        {
            if (RoundFightData.TempTriggerData.UnitData != null &&
                RoundFightData.TempTriggerData.UnitData.UnitCamp == PlayerManager.Instance.PlayerData.UnitCamp &&
                RoundFightData.TempTriggerData.TriggerType == ETempTriggerType.MoveUnit)
            {
                //var firstUnitActionSoliders = new List<int>();
                RoundFightData.TempTriggerData.TempUnitMovePaths = CacheUnitMoveDatas(
                    RoundFightData.TempTriggerData.UnitData.Idx, RoundFightData.TempTriggerData.TempUnitMovePaths,
                    RoundFightData.SoliderMoveDatas, EUnitActionState.Run);
            }

        }

        private void CacheEnemyMoveDatas()
        {
            if (RoundFightData.TempTriggerData.UnitData != null &&
                RoundFightData.TempTriggerData.UnitData.UnitCamp == EUnitCamp.Enemy &&
                RoundFightData.TempTriggerData.TriggerType == ETempTriggerType.MoveUnit)
            {
                //var firstUnitActionSoliders = new List<int>();
                RoundFightData.TempTriggerData.TempUnitMovePaths = CacheUnitMoveDatas(
                    RoundFightData.TempTriggerData.UnitData.Idx, RoundFightData.TempTriggerData.TempUnitMovePaths,
                    RoundFightData.EnemyMoveDatas, EUnitActionState.Run);
            }

        }

 
        public void SimulateTriggerData(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var effectUnit = GetUnitByIdx(triggerData.EffectUnitIdx);
            var actionUnit = GetUnitByIdx(triggerData.ActionUnitIdx);

            var triggerValue = triggerData.Value + triggerData.DeltaValue;

            switch (triggerData.TriggerDataType)
            {
                case ETriggerDataType.HeroAtrb:
                    var dataBattleHero = effectUnit as Data_BattleHero;
                    switch (triggerData.HeroAttribute)
                    {
                        case EHeroAttribute.HP:
                            //triggerData.ChangeHPInstantly = false;
                            CurHPTriggerData(triggerData, triggerDatas);

                            break;

                        // case EHeroAttribute.CurEnergy:
                        //     if (dataBattleHero != null)
                        //     {
                        //         dataBattleHero.Attribute.SetAttribute(EHeroAttribute.CurEnergy,
                        //             dataBattleHero.Attribute.GetAttribute(EHeroAttribute.CurEnergy) +
                        //             triggerValue);
                        //     }
                        //
                        //     break;
                        case EHeroAttribute.CurHeart:
                            break;
                        case EHeroAttribute.MaxHeart:
                            break;
                        case EHeroAttribute.MaxHP:
                            break;
                        case EHeroAttribute.Coin:
                            if (dataBattleHero != null)
                            {
                                dataBattleHero.Attribute.SetAttribute(EHeroAttribute.Coin,
                                    dataBattleHero.Attribute.GetAttribute(EHeroAttribute.Coin) + triggerValue);
                            }

                            break;
                        case EHeroAttribute.Damage:
                            if (dataBattleHero != null)
                            {
                                dataBattleHero.Attribute.SetAttribute(EHeroAttribute.Damage,
                                    dataBattleHero.Attribute.GetAttribute(EHeroAttribute.Damage) + triggerValue);
                            }

                            break;
                        default:
                            break;
                    }

                    break;
                case ETriggerDataType.Atrb:
                    switch (triggerData.BattleUnitAttribute)
                    {
                        case EUnitAttribute.HP:
                            CurHPTriggerData(triggerData, triggerDatas);

                            var coreHurtAcquireCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(
                                EBlessID.CoreHurtAcquireCard,
                                PlayerManager.Instance.PlayerData.UnitCamp);

                            if (coreHurtAcquireCard != null && triggerValue < 0 &&
                                BattleCoreManager.Instance.IsCoreIdx(triggerData.EffectUnitIdx))
                            {
                                var drCoreHurtAcquireCard = GameEntry.DataTable.GetBless(EBlessID.CoreHurtAcquireCard);
                                BattleCardManager.Instance.CacheAcquireCards(triggerData, triggerDatas,
                                    int.Parse(drCoreHurtAcquireCard.GetValues(0)[0]));
                                // triggerData.AcquireCardCirculation.HandCards = BattleCardManager.Instance.CacheAcquireHandCards(
                                //     RoundFightData.GamePlayData, int.Parse(drCoreHurtAcquireCard.GetValues(0)[0]));

                                // var battlePlayerData = RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(RoundFightData.GamePlayData.PlayerData.UnitCamp);
                                //
                                // triggerData.AcquireCardCirculation.HandCards = new List<int>(battlePlayerData.HandCards);
                                // triggerData.AcquireCardCirculation.StandByCards = new List<int>(battlePlayerData.StandByCards);
                                // triggerData.AcquireCardCirculation.PassCards = new List<int>(battlePlayerData.PassCards);
                                // triggerData.AcquireCardCirculation.ConsumeCards = new List<int>(battlePlayerData.ConsumeCards);
                            }

                            break;
                        case EUnitAttribute.MaxHP:
                            if (effectUnit != null)
                            {
                                effectUnit.BaseMaxHP += (int)triggerValue;
                            }
                            
                            CurHPTriggerData(triggerData, triggerDatas);
                            break;
                        case EUnitAttribute.BaseDamage:
                            effectUnit.BaseDamage += (int)triggerValue;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ETriggerDataType.State:
                    effectUnit.ChangeState(triggerData.UnitStateDetail.UnitState, triggerData.UnitStateDetail.Value);
                    if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff]
                            .Contains(triggerData.UnitStateDetail.UnitState) && triggerData.UnitStateDetail.Value > 0)
                    {
                        AddDeBuffTrigger(triggerData, triggerDatas);
                    }

                    break;
                case ETriggerDataType.RoundState:
                    effectUnit.ChangeRoundState(triggerData.UnitStateDetail.UnitState,
                        triggerData.UnitStateDetail.Value);
                    break;
                case ETriggerDataType.Card:
                    switch (triggerData.CardTriggerType)
                    {

                        case ECardTriggerType.AcquireCard:
                            BattleCardManager.Instance.CacheAcquireCards(triggerData, triggerDatas,
                                (int)triggerValue);
                            break;

                        case ECardTriggerType.ToConsume:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardData = GetCard(triggerData.TriggerCardIdx);
                                cardData.CardDestination = ECardDestination.Consume;

                            }

                            break;
                        case ECardTriggerType.ToStandBy:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardData = GetCard(triggerData.TriggerCardIdx);
                                cardData.CardDestination = ECardDestination.StandBy;

                            }

                            break;
                        /*case ECardTriggerType.CardEnergy:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardData = GetCard(triggerData.TriggerCardIdx);
                                cardData.EnergyDelta = (int)triggerValue;

                            }

                            break;*/

                    }

                    break;
                case ETriggerDataType.RemoveUnit:
                    effectUnit.RemoveAllState();
                    CurHPTriggerData(triggerData, triggerDatas);
                    break;
                case ETriggerDataType.RoundBuff:
                    // var battlePlayerData =
                    //     RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(effectUnit.UnitCamp);
                    //battlePlayerData.RoundBuffs.Add(triggerData.BuffID);
                    break;
                case ETriggerDataType.ClearBuff:
                    effectUnit.RemoveAllState(triggerData.UnitStateEffectTypes);
                    break;
                case ETriggerDataType.TransferBuff:
                    BattleBuffManager.Instance.TransferBuff(actionUnit, effectUnit, triggerData);

                    break;
                default:
                    break;
            }

        }


        private void CacheLinks()
        {
            // foreach (var kv in BattleUnitDatas)
            // {
            //     kv.Value.LinkIDs.Clear();
            // }
            // foreach (var kv in BattleUnitDatas)
            // {
            //     kv.Value.Links.Clear();
            // }
            // UnitActionRange.Clear();
            //
            // if(BattleCurseManager.Instance.IsLinkUnEffect())
            //     return;
            //
            // CacheUnitLinkIDs();
            // CacheUnitLinks();
            // CacheUnitActionRange();

        }



        
        
        public void CacheRoundPassCards()
        {
            RoundFightData.RoundPassCardCirculation.TriggerDatas.Clear();

            var battlePlayerData =
                RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(RoundFightData.GamePlayData.PlayerData
                    .UnitCamp);

            ToPassCard(battlePlayerData);

            RoundFightData.RoundPassCardCirculation.PassCards = new List<int>(battlePlayerData.PassCards);
            RoundFightData.RoundPassCardCirculation.HandCards = new List<int>(battlePlayerData.HandCards);
            RoundFightData.RoundPassCardCirculation.ConsumeCards = new List<int>(battlePlayerData.ConsumeCards);
            RoundFightData.RoundPassCardCirculation.StandByCards = new List<int>(battlePlayerData.StandByCards);
        }

        public void CacheRoundHandCards(bool isFirstRound)
        {
            RoundFightData.RoundAcquireCardCirculation.TriggerDatas.Clear();

            var cardCount = RoundAcquireCardCount(RoundFightData.GamePlayData);

            // var unuseCount =
            //     BattleManager.Instance.GetUnUseCardCount();

            if (BattleCardManager.Instance.IsShuffleCard(cardCount, RoundFightData.GamePlayData))
            {
                var addHP = BlessManager.Instance.ShuffleCardAddCurHP(RoundFightData.GamePlayData);
                if (addHP > 0)
                {
                    var hpDeltaData = new BlessHPDeltaData()
                    {
                        HPDelta = addHP,
                        HPDeltaType = EHPDeltaType.Bless,

                    };
                    AddHPDetlaData(PlayerManager.Instance.PlayerData.UnitCamp, hpDeltaData);

                    //RoundFightData.HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Add(hpDeltaData);
                    var blessData = BattleFightManager.Instance.RoundFightData.GamePlayData.GetUsefulBless(
                        EBlessID.ShuffleCardAddCurHP, PlayerManager.Instance.PlayerData.UnitCamp);
                    ;

                    var triggerDatas = new List<TriggerData>();
                    var triggerData =
                        BattleFightManager.Instance.Unit_HeroAttribute(Constant.Battle.UnUnitTriggerIdx,
                            Constant.Battle.UnUnitTriggerIdx, BattleFightManager.Instance.PlayerData.BattleHero.Idx,
                            EHeroAttribute.HP, addHP);
                    triggerData.TriggerDataSubType = ETriggerDataSubType.Bless;
                    triggerData.TriggerBlessIdx = blessData.BlessIdx;
                    BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);

                    RoundFightData.RoundAcquireCardCirculation.TriggerDatas.Add(Constant.Battle.UnUnitTriggerIdx,
                        triggerDatas);

                    // var playerData =
                    //     RoundFightData.GamePlayData.GetPlayerData(PlayerManager.Instance.PlayerData.UnitCamp);
                    // playerData.BattleHero.ChangeHP(hpDeltaData.HPDelta);
                }
            }

            RoundFightData.RoundAcquireCardCirculation.HandCards =
                BattleCardManager.Instance.CacheAcquireHandCards(RoundFightData.GamePlayData, cardCount,
                    isFirstRound);

            var battlePlayerData =
                RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(RoundFightData.GamePlayData.PlayerData
                    .UnitCamp);

            RoundFightData.RoundAcquireCardCirculation.PassCards = new List<int>(battlePlayerData.PassCards);
            RoundFightData.RoundAcquireCardCirculation.HandCards = new List<int>(battlePlayerData.HandCards);
            RoundFightData.RoundAcquireCardCirculation.ConsumeCards = new List<int>(battlePlayerData.ConsumeCards);
            RoundFightData.RoundAcquireCardCirculation.StandByCards = new List<int>(battlePlayerData.StandByCards);
        }



        private List<int> ToPassCard(Data_BattlePlayer battlePlayerData)
        {


            var passCards = new List<int>();
            for (int i = battlePlayerData.HandCards.Count - 1; i >= 0; i--)
            {
                var card = BattleManager.Instance.GetCard(battlePlayerData.HandCards[i]);
                if (card.FuneCount(EBuffID.Spec_UnPass) > 0)
                    continue;

                passCards.Add(battlePlayerData.HandCards[i]);
                battlePlayerData.HandCards.RemoveAt(i);
                //BattlePlayerData.PassCards.Add(BattlePlayerData.HandCards[i]);
            }

            battlePlayerData.PassCards.AddRange(passCards);


            return passCards;
        }

        private int RoundAcquireCardCount(Data_GamePlay gamePlayData)
        {
            var cardCount = BattleCardManager.Instance.GetEachHardCardCount(gamePlayData);
            var eachRoundAcquireCardCount =
                gamePlayData.BlessCount(EBlessID.EachRoundAcquireCard,
                    BattleManager.Instance.CurUnitCamp);
            if (eachRoundAcquireCardCount > 0)
            {
                var drEachRoundAcquireCard = GameEntry.DataTable.GetBless(EBlessID.EachRoundAcquireCard);

                cardCount += (int)BattleBuffManager.Instance.GetBuffValue(drEachRoundAcquireCard.Values0[0]) *
                             eachRoundAcquireCardCount;
            }

            return cardCount;
        }

        public void AddHPDetlaData(EUnitCamp unitCamp, HPDeltaData hpDeltaData)
        {
            // var coreHurtAcquireCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.CoreHurtAcquireCard,
            //     PlayerManager.Instance.PlayerData.UnitCamp);

            RoundFightData.HPDeltaDict[unitCamp].Add(hpDeltaData);
            // if (hpDeltaData is not CardHPDeltaData && coreHurtAcquireCard != null && hpDeltaData.HPDelta < 0)
            // {
            //     
            //     
            // }
        }

        public Data_Card GetCard(int cardIdx)
        {
            if (RoundFightData.GamePlayData.PlayerData.CardDatas.ContainsKey(cardIdx))
                return RoundFightData.GamePlayData.PlayerData.CardDatas[cardIdx];
          
            return null;
        }


        public bool IsSoliderAutoAttackData(int unitidx)
        {
            if (!BattleFightManager.Instance.RoundFightData.SoliderActiveAttackDatas.ContainsKey(unitidx))
                return false;

            return BattleFightManager.Instance.RoundFightData.SoliderActiveAttackDatas[unitidx].TriggerDataDict.Count > 0;

        }

        public void CacheHurtTrigger(ActionData actionData)
        {
            foreach (var kv in actionData.TriggerDataDict)
            {
                InternalCacheHurtTrigger(kv.Value.TriggerDatas);
            }
 
            foreach (var kv in actionData.TriggerDataDict)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    foreach (var kv3 in kv2.Value.MoveActionData.TriggerDataDict)
                    {
                        InternalCacheHurtTrigger(kv3.Value.TriggerDatas);
                    }
                    
                }
            }
            

        }
        
        public void CacheHurtTrigger(Dictionary<int, TriggerCollection> triggerCollections)
        {
            foreach (var kv in triggerCollections)
            {
                InternalCacheHurtTrigger(kv.Value.TriggerDatas);
            }


        }

        private void InternalCacheHurtTrigger(List<TriggerData> triggerDatas)
        {
            for (int i = triggerDatas.Count - 1; i >= 0; i--)
            {
                var triggerData = triggerDatas[i];
                if (GameUtility.IsSubCurHPTrigger(triggerData))
                {
                    var _effectUnit = GameUtility.GetUnitDataByIdx(triggerData.EffectUnitIdx);
                    if (_effectUnit != null )
                    {
                        // BattleCurseManager.Instance.CacheUnitDeadRecoverLessHPUnit(effectUnitOldHP, effectUnitData.CurHP,
                        //     triggerDatas);
                        BattleBuffManager.Instance.HurtTrigger(triggerData, triggerDatas);
                        if (!_effectUnit.Exist())
                        {
                            BattleFightManager.Instance.DeadTrigger(triggerData, triggerDatas);
                            BattleFightManager.Instance.KillTrigger(triggerData, triggerDatas);
                        }
                                

                    }
                }
            }

        }

    }
}