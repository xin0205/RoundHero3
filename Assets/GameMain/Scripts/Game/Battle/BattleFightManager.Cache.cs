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

            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.AutoAtk)
            {
                var effectUnit = GetUnitByGridPosIdx(BattleManager.Instance.TempTriggerData.TargetGridPosIdx);
                BattleCardManager.Instance.CacheUseCardData(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                    effectUnit, BattleManager.Instance.TempTriggerData.TargetGridPosIdx,
                    Constant.Battle.UnUnitTriggerIdx);
            }
            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempTriggerType.MoveUnit)
            {
                var effectUnit = GetUnitByGridPosIdx(BattleManager.Instance.TempTriggerData.TargetGridPosIdx);
                BattleCardManager.Instance.CacheUseCardData(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                    effectUnit, BattleManager.Instance.TempTriggerData.TargetGridPosIdx,
                    Constant.Battle.UnUnitTriggerIdx);
            }

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

        public void CacheRoundFightData()
        {
            //Log.Debug("CacheRoundFightData");

            //BattleAreaManager.Instance.RefreshObstacles();

            CachePreData();

            //CacheLinks();

            //CacheUseCardTriggerDatas();
            //CacheSoliderActiveAttackDatas();
            //CacheSoliderAutoAttackDatas();

            //CacheRoundStartDatas();

            //CacheSoliderMoveDatas();
            //CacheSoliderAttackDatas();
            CacheRoundHandCards(true);
            CalculateEnemyPaths();
            //CacheEnemyMoveDatas();
            //

            // CalculateThirdUnitPaths();
            // CacheThirdUnitMoveDatas();
            // CacheThirdUnitAttackDatas();

            //CacheRoundEndDatas();


        }

        public void CacheRoundFightData2()
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

        private void CacheUseCardTriggerAttack()
        {
            // foreach (var kv in BattleUnitDatas)
            // {
            //     if (kv.Value.CurHP <= 0)
            //         continue;
            //
            //     //kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
            //     if (kv.Value.GetAllStateCount(EUnitState.UnAttack) > 0 &&
            //         !GameUtility.ContainRoundState(RoundFightData.GamePlayData, ECardID.RoundDeBuffUnEffect))
            //         continue;
            //
            //     if (kv.Value.BuffCount(EBuffID.UseCardTriggerAttack) <= 0)
            //         continue;
            //
            //
            //     BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
            //         out List<BuffValue> triggerBuffDatas);
            //
            //     if (triggerBuffDatas != null)
            //     {
            //         var actionData = new ActionData();
            //         actionData.ActionUnitID = kv.Value.ID;
            //         RoundFightData.UseCardTriggerDatas.Add(kv.Value.ID, actionData);
            //
            //         CacheAttackData(kv.Value.UnitCamp, triggerBuffDatas, kv.Value.GridPosIdx, actionData,
            //             kv.Value.ID, EBuffTriggerType.UseCard);
            //     }
            //
            //
            // }
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
                            ETriggerDataType.RoleState);
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
                    actionData.TriggerDatas.Add(kv.Key, triggerDatas);
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
                        // if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_HurtSubDamageSameAttack)
                        // {
                        //     if ((RoundFightData.GamePlayData.BattleData.Round + 1) % triggerBuffData.ValueList[0] ==
                        //         triggerBuffData.ValueList[1])
                        //     {
                        //         values = new List<float>()
                        //             {Math.Abs(GetDamage(unit.ID))};
                        //     }
                        //     else
                        //     {
                        //         continue;
                        //     }
                        // }
                        // else if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_AddDamageSameLastHurt ||
                        //          triggerBuffData.DrBuff.BuffID == EBuffID.Round_HurtSubDamageSameLastHurt)
                        // {
                        //     
                        //     if (unit.LastCurHPDelta < 0)
                        //     {
                        //         values = new List<float>()
                        //             {-unit.LastCurHPDelta};
                        //
                        //     }
                        //     else
                        //     {
                        //         continue;
                        //     }
                        // }
                        // else if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_AddDamageSameEnemyCount ||
                        //          triggerBuffData.DrBuff.BuffID == EBuffID.Round_HurtSubDamageSameEnemyCount)
                        // {
                        //
                        //     var unitCount = GameUtility.GetUnitCount(RoundFightData.GamePlayData, unit.UnitCamp,
                        //         ERelativeCamp.Enemy);
                        //     if (unitCount >= 0)
                        //     {
                        //         values = new List<float>()
                        //             {unitCount};
                        //
                        //     }
                        //     else
                        //     {
                        //         continue;
                        //     }
                        // }
                        // else
                        // {
                        //     values = triggerBuffData.ValueList;
                        //
                        // }
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
                    actionData.TriggerDatas.Add(kv.Key, triggerDatas);
                }

                CalculateHeroHPDelta(actionData);
            }
        }

        // private void CacheUnitAutoAttackDatas(EUnitCamp unitCamp, List<BuffValue> triggerBuffDatas, int gridPosIdx,
        //     ActionData actionData, int attackUnitID)
        // {
        //     foreach (var triggerBuffData in triggerBuffDatas)
        //     {
        //         if (triggerBuffData.BuffData.BuffTriggerType != EBuffTriggerType.AutoAttack)
        //             continue;
        //
        //
        //         var range = GameUtility.GetRange(gridPosIdx, triggerBuffData.BuffData.TriggerRange, unitCamp,
        //             triggerBuffData.BuffData.TriggerUnitCamps, true);
        //
        //         foreach (var rangeGridPosIdx in range)
        //         {
        //             var unit = GetUnitByGridPosIdx(rangeGridPosIdx);
        //             if (unit == null)
        //                 continue;
        //
        //             List<float> values = triggerBuffData.ValueList;
        //             
        //             actionData.AddEmptyTriggerDataList(unit.Idx);
        //             var triggerDatas = actionData.TriggerDatas[unit.Idx];
        //             
        //             BattleBuffManager.Instance.AutoAttackTrigger(triggerBuffData.BuffData, values,
        //                 attackUnitID,
        //                 attackUnitID,
        //                 unit.Idx, triggerDatas);
        //
        //             CacheUnitActiveMoveDatas(attackUnitID, rangeGridPosIdx, triggerBuffData.BuffData, actionData);
        //         }
        //
        //     }

        //}

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
            CalculateHeroHPDelta(actionData);
        }

        private void CacheAttackData(EUnitCamp unitCamp, List<BuffValue> triggerBuffDatas, int gridPosIdx,
            ActionData actionData, int attackUnitIdx, EBuffTriggerType buffTriggerType)
        {
            var attackUnit = GetUnitByIdx(attackUnitIdx);
            //var attackWithoutHero = attackUnit.BuffCount(EBuffID.AttackWithoutHero) > 0;

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

                // || attackWithoutHero

                var rangeContainFirstCamp = false;
                var range2 = new List<int>();
                if (unitCamp == EUnitCamp.Enemy && range.Count > 0)
                {

                    for (int i = range.Count - 1; i >= 0; i--)
                    {
                        var _gridPosIdx = range[i];
                        var unit = GetUnitByGridPosIdx(_gridPosIdx);
                        if (unit != null && unit.UnitRole == EUnitRole.Hero)
                        {
                            range2.Add(_gridPosIdx);
                            range.Remove(_gridPosIdx);
                        }
                    }

                    for (int i = 0; i < range.Count; i++)
                    {
                        var _gridPosIdx = range[i];
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
                    if (!actionData.TriggerDatas.ContainsKey(effectUnit.Idx))
                    {
                        actionData.TriggerDatas.Add(effectUnit.Idx, triggerDatas);
                    }
                    else
                    {
                        triggerDatas = actionData.TriggerDatas[effectUnit.Idx];

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
        }

        // private void CacheUnitActiveAttackData(EUnitCamp unitCamp, List<BuffValue> triggerBuffDatas, int gridPosIdx,
        //     ActionData actionData, int attackUnitID, int effectGridPosIdx)
        // {
        //     foreach (var triggerBuffData in triggerBuffDatas)
        //     {
        //         var isSubCurHP = false;
        //         
        //         var effectUnit = GetUnitByGridPosIdx(effectGridPosIdx);
        //         
        //         if (effectUnit != null)
        //         {
        //             if (effectUnit.CurHP <= 0)
        //                 continue;
        //             
        //             actionData.AddEmptyTriggerDataList(effectUnit.Idx);
        //             var triggerDatas = actionData.TriggerDatas[effectUnit.Idx];
        //         
        //             var triggerData = BattleBuffManager.Instance.BuffTrigger(triggerBuffData.BuffData.BuffTriggerType, triggerBuffData.BuffData,
        //                 triggerBuffData.ValueList, attackUnitID,
        //                 attackUnitID,
        //                 effectUnit.Idx, null, triggerDatas);
        //             
        //             triggerData.BuffValue = triggerBuffData.Copy();
        //
        //             if (GameUtility.IsSubCurHPTrigger(triggerData))
        //             {
        //                 isSubCurHP = true;
        //             }
        //         
        //             if (isSubCurHP)
        //             {
        //                 BattleBuffManager.Instance.AttackTrigger(triggerDatas[0], triggerDatas);
        //                 BattleUnitStateManager.Instance.CheckUnitState(attackUnitID, triggerDatas);
        //             }
        //             
        //         }
        //
        //         CacheUnitActiveMoveDatas(attackUnitID, effectGridPosIdx, triggerBuffData.BuffData, actionData);
        //
        //     }
        // }

        private void CacheUnitActiveMoveDatas(int actionUnitIdx, int effectGridPosIdx, BuffData buffData,
            ActionData actionData, TriggerData triggerData)
        {
            if (effectGridPosIdx == -1)
                return;

            var actionUnit = GetUnitByIdx(actionUnitIdx);
            var effectUnit = GetUnitByGridPosIdx(effectGridPosIdx);
            if (effectUnit == null && !Constant.Battle.RelatedUnitFlyRanges.Contains(buffData.FlyRange))
                return;

            if (effectUnit != null)
            {
                var relativeCamp = GameUtility.GetRelativeCamp(actionUnit.UnitCamp, effectUnit.UnitCamp);
                if (!buffData.TriggerUnitCamps.Contains(relativeCamp))
                    return;
            }

            var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionUnit.GridPosIdx);


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

            if (buffData.FlyType == EFlyType.Exchange)
            {
                if(actionUnit.Idx == effectUnit.Idx)
                    return;
                
                if(actionData.MoveData.MoveUnitDatas.ContainsKey(actionUnit.Idx))
                    return;
                
                actionData.MoveData.MoveUnitDatas.Add(actionUnit.Idx, new MoveUnitData()
                {
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

                actionData.MoveData.MoveUnitDatas.Add(effectUnit.Idx, new MoveUnitData()
                {
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
            }
            else if (Constant.Battle.RelatedUnitFlyRanges.Contains(buffData.FlyRange) ||
                     Constant.Battle.DynamicRelatedUnitFlyRanges.Contains(buffData.FlyRange))
            {
                var relatedUnits = GetUnitByGridPosIdx(actionUnit.GridPosIdx, effectGridPosIdx, buffData.FlyRange);
                foreach (var relatedUnit in relatedUnits)
                {
                    var relatedUnitCoord = GameUtility.GridPosIdxToCoord(relatedUnit.GridPosIdx);
                    flyDirect = buffData.FlyType == EFlyType.Back
                        ? relatedUnitCoord - effectUnitCoord
                        : effectUnitCoord - relatedUnitCoord;
                    flyPaths = GetFlyPaths(relatedUnit.GridPosIdx, flyDirect, dis, EUnitActionState.Fly);
                    CacheUnitMoveDatas(relatedUnit.Idx, flyPaths, moveActionDatas, EUnitActionState.Fly);


                    if (!actionData.MoveData.MoveUnitDatas.ContainsKey(relatedUnit.Idx) &&
                        moveActionDatas.ContainsKey(relatedUnit.Idx))
                    {
                        actionData.MoveData.MoveUnitDatas.Add(relatedUnit.Idx, new MoveUnitData()
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

                if (!actionData.MoveData.MoveUnitDatas.ContainsKey(effectUnit.Idx) &&
                    moveActionDatas.ContainsKey(effectUnit.Idx))
                {
                    actionData.MoveData.MoveUnitDatas.Add(effectUnit.Idx, new MoveUnitData()
                    {
                        ActionUnitIdx = triggerData.ActionUnitIdx,
                        EffectGridPosIdx = effectGridPosIdx,
                        UnitIdx = effectUnit.Idx,
                        MoveActionData = moveActionDatas[effectUnit.Idx],
                        UnitActionState = EUnitActionState.Fly,
                    });

                }

                var relatedUnits =
                    GameUtility.GetRelatedCoords(buffData.FlyRange, actionUnit.GridPosIdx, effectGridPosIdx);
                if (relatedUnits.Count > 0)
                {
                    var relatedUnit = GetUnitByGridPosIdx(GameUtility.GridCoordToPosIdx(relatedUnits[0]));
                    if (relatedUnit != null)
                    {
                        var relatedUnitCoord = GameUtility.GridPosIdxToCoord(relatedUnit.GridPosIdx);
                        flyDirect = relatedUnitCoord - actionUnitCoord;

                        flyPaths = GetFlyPaths(relatedUnit.GridPosIdx, flyDirect, dis, EUnitActionState.Fly);
                        CacheUnitMoveDatas(relatedUnit.Idx, flyPaths, moveActionDatas, EUnitActionState.Fly);

                        if (!actionData.MoveData.MoveUnitDatas.ContainsKey(relatedUnit.Idx) &&
                            moveActionDatas.ContainsKey(relatedUnit.Idx))
                        {
                            actionData.MoveData.MoveUnitDatas.Add(relatedUnit.Idx, new MoveUnitData()
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
                if (buffData.FlyRange == EActionType.Self)
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
                else if (buffData.FlyType == EFlyType.SelfPass)
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
            var moveUnit = GetUnitByIdx(moveUnitIdx);
            if (moveUnit != null)
            {
                var flyPaths = GetFlyPaths(moveUnit.GridPosIdx, flyDirect, dis, unitActionState);

                CacheUnitMoveDatas(moveUnitIdx, flyPaths, moveActionDatas, unitActionState);

                if (moveActionDatas.Count > 0)
                {
                    actionData.MoveData.MoveUnitDatas.Add(moveUnitIdx, new MoveUnitData()
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

                enemies.Add(kv.Value);
            }

            enemies.Sort((e1, e2) => { return e1.GridPosIdx - e2.GridPosIdx; });

            foreach (var enemy in enemies)
            {

                CacheEnemyAttackData(enemy as Data_BattleMonster);
            }

        }

        // private void CacheEnemyAttackDatas()
        // {
        //
        //     foreach (var kv in BattleUnitDatas)
        //     {
        //         if (kv.Value.UnitCamp != EUnitCamp.Enemy)
        //             continue;
        //
        //         if (kv.Value.CurHP <= 0)
        //             continue;
        //
        //
        //         if (!RoundFightData.EnemyMovePaths.ContainsKey(kv.Key) ||
        //             RoundFightData.EnemyMovePaths[kv.Key] == null || RoundFightData.EnemyMovePaths[kv.Key].Count <= 0)
        //             continue;
        //
        //         kv.Value.GridPosIdx =
        //             RoundFightData.EnemyMovePaths[kv.Key][RoundFightData.EnemyMovePaths[kv.Key].Count - 1];
        //
        //     }
        //
        //     CacheLinks();
        //
        //     foreach (var kv in BattleUnitDatas)
        //     {
        //         if (kv.Value.UnitCamp != EUnitCamp.Enemy)
        //             continue;
        //
        //         var enemyData = kv.Value as Data_BattleMonster;
        //         if (enemyData == null)
        //             continue;
        //
        //         if (kv.Value.CurHP <= 0)
        //             continue;
        //
        //         //kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
        //         if ((kv.Value.GetAllStateCount(EUnitState.UnAtk) > 0) &&
        //             !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
        //             continue;
        //
        //
        //         var actionData = new ActionData();
        //         actionData.ActionUnitID = enemyData.Idx;
        //
        //         RoundFightData.EnemyAttackDatas.Add(enemyData.Idx, actionData);
        //         var drEnemy = GameEntry.DataTable.GetEnemy(enemyData.MonsterID);
        //         var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
        //         
        //         var attackRange = GameUtility.GetRange(enemyData.GridPosIdx, buffData.TriggerRange, EUnitCamp.Enemy,
        //             buffData.TriggerUnitCamps, true);
        //
        //         if (attackRange.Count > 0)
        //         {
        //             BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
        //                 out List<BuffValue> triggerBuffDatas);
        //             CacheAttackData(EUnitCamp.Enemy, triggerBuffDatas, kv.Value.GridPosIdx, actionData, enemyData.Idx, EBuffTriggerType.ActionEnd);
        //         }
        //         else
        //         {
        //             BattleUnitManager.Instance.GetSecondaryBuffValue(RoundFightData.GamePlayData, kv.Value,
        //                 out List<BuffValue> secondaryTriggerBuffDatas);
        //             CacheAttackData(EUnitCamp.Enemy, secondaryTriggerBuffDatas, kv.Value.GridPosIdx, actionData, enemyData.Idx, EBuffTriggerType.ActionEnd);
        //         }
        //
        //         
        //         
        //
        //         CalculateHeroHPDelta(actionData);
        //
        //     }
        // }


        private void CacheThirdUnitAttackDatas()
        {
            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != EUnitCamp.Third)
                    continue;

                if (!kv.Value.Exist())
                    continue;


                if (!RoundFightData.ThirdUnitMovePaths.ContainsKey(kv.Key) ||
                    RoundFightData.ThirdUnitMovePaths[kv.Key] == null ||
                    RoundFightData.ThirdUnitMovePaths[kv.Key].Count <= 0)
                    continue;

                kv.Value.GridPosIdx =
                    RoundFightData.ThirdUnitMovePaths[kv.Key][RoundFightData.ThirdUnitMovePaths[kv.Key].Count - 1];

            }

            CacheLinks();

            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != EUnitCamp.Third)
                    continue;

                var enemyData = kv.Value as Data_BattleMonster;
                if (enemyData == null)
                    continue;

                if (!kv.Value.Exist())
                    continue;

                //kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
                if ((kv.Value.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;

                var actionData = new ActionData();
                actionData.ActionUnitIdx = enemyData.Idx;

                RoundFightData.ThirdUnitAttackDatas.Add(enemyData.Idx, actionData);

                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);
                CacheAttackData(EUnitCamp.Third, triggerBuffDatas, kv.Value.GridPosIdx, actionData, enemyData.Idx,
                    EBuffTriggerType.ActionEnd);
                CalculateHeroHPDelta(actionData);
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

            var battlePlayerData = RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance
                .CurUnitCamp);
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
                    moveActionData.TriggerDatas.Add(i, triggerDatas);
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

            if (minFullPaths.Count > 0)
            {
                passUnit.GridPosIdx = minFullPaths[minFullPaths.Count - 1];
                RoundFightData.GamePlayData.BattleData.GridTypes[minFullPaths[0]] = EGridType.Empty;
                RoundFightData.GamePlayData.BattleData.GridTypes[minFullPaths[minFullPaths.Count - 1]] = EGridType.Unit;

                RefreshUnitGridPosIdx();
                if (passUnit.UnitCamp == EUnitCamp.Enemy && unitActionState == EUnitActionState.Run)
                {
                    if (!RoundFightData.EnemyAttackDatas.ContainsKey(passUnit.Idx))
                    {
                        CacheEnemyAttackData(passUnit as Data_BattleMonster);
                    }

                }
            }

            // foreach (var kv in unitMoveDatas)
            // {
            //     CalculateHeroHPDelta(kv.Value);
            // }



            return minFullPaths;
        }

        // public void CacheUnitRangeDatas(int actionUnitID, int preGridPosIdx, int curGridPosIdx, List<TriggerData> triggerDatas)
        // {
        //     var unitDisplacementDatas =
        //         new List<BattleBuffManager.UnitDisplacementData>();
        //     unitDisplacementDatas.Add(new BattleBuffManager.UnitDisplacementData(actionUnitID, preGridPosIdx, curGridPosIdx));
        //     var unitDisplacementResults = BattleBuffManager.Instance.CacheUnitRangeTrigger(RoundFightData.GamePlayData, unitDisplacementDatas);
        //
        //     foreach (var unitDisplacementResult in unitDisplacementResults)
        //     {
        //         var buffTriggerType = unitDisplacementResult.BuffTriggerType;
        //         var unit = GameUtility.GetUnitDataByIdx(unitDisplacementResult.TriggerUnitID);
        //
        //         BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, unit,
        //             out List<BuffValue> triggerBuffDatas);
        //
        //         if (triggerBuffDatas != null)
        //         {
        //             foreach (var triggerBuffData in triggerBuffDatas)
        //             {
        //                 BattleBuffManager.Instance.BuffTrigger(buffTriggerType,
        //                     triggerBuffData.BuffData, triggerBuffData.ValueList,
        //                     unitDisplacementResult.TriggerUnitID, unitDisplacementResult.TriggerUnitID,
        //                     unitDisplacementResult.BeTriggerUnitID,
        //                     triggerDatas);
        //             }
        //
        //         }
        //
        //     }
        // }

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

        

        private void CacheThirdUnitMoveDatas()
        {
            CacheUnitMoveDatas(RoundFightData.ThirdUnitMovePaths, RoundFightData.ThirdUnitMoveDatas);

        }

        private void CacheUnitMoveDatas(Dictionary<int, List<int>> movePaths, Dictionary<int, MoveActionData> moveDatas)
        {
            var keys = movePaths.Keys.ToList();
            //var firstUnitActionSoliders = new List<int>();

            for (int i = 0; i < movePaths.Count; i++)
            {
                movePaths[keys[i]] = CacheUnitMoveDatas(keys[i],
                    movePaths[keys[i]], moveDatas, EUnitActionState.Run);
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
                case ETriggerDataType.RoleAttribute:
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
                case ETriggerDataType.RoleState:
                    effectUnit.ChangeState(triggerData.UnitStateDetail.UnitState, triggerData.UnitStateDetail.Value);
                    if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff]
                            .Contains(triggerData.UnitStateDetail.UnitState) && triggerData.UnitStateDetail.Value > 0)
                    {
                        AddDeBuffTrigger(triggerData, triggerDatas);
                    }

                    break;
                case ETriggerDataType.RoundRoleState:
                    effectUnit.ChangeRoundState(triggerData.UnitStateDetail.UnitState,
                        triggerData.UnitStateDetail.Value);
                    break;
                case ETriggerDataType.Card:
                    switch (triggerData.CardTriggerType)
                    {

                        case ECardTriggerType.AcquireCard:
                            BattleCardManager.Instance.CacheAcquireCards(triggerData, triggerDatas,
                                (int)(triggerData.Value + triggerData.DeltaValue));
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



        public void CalculateEnemyPaths()
        {
            CalculateUnitPaths(EUnitCamp.Enemy, RoundFightData.EnemyMovePaths);
        }

        public void CalculateHeroHPDelta(MoveActionData moveActionData)
        {

            //moveActionData.MoveUnitIdx, 
            CalculateHeroHPDelta(moveActionData.TriggerDatas, true);
        }

        public void CalculateHeroHPDelta(ActionData actionData)
        {
            // foreach (var kv in actionData.MoveData.MoveUnitDatas)
            // {
            //     CalculateHeroHPDelta(kv.Value.MoveActionData);
            // }

            //actionData.ActionUnitID, 
            CalculateHeroHPDelta(actionData.TriggerDatas);
        }


        public void CalculateHeroHPDelta(Dictionary<int, List<TriggerData>> triggerDatas,
            bool isMoveTriggerData = false)
        {
            // foreach (var kv in triggerDatas)
            // {
            //     for (int i = 0; i < kv.Value.Count; i++)
            //     {
            //         var triggerData = kv.Value[i];
            //         var effectUnit = GetUnitByID(triggerData.EffectUnitID);
            //         if (effectUnit == null)
            //             continue;
            //                 
            //         // if (!triggerDatas.ContainsKey(triggerData.EffectUnitID))
            //         // {
            //         //     triggerDatas.Add(triggerData.EffectUnitID, new List<TriggerData>());
            //         // }
            //         
            //         
            //         if (effectUnit.AddHeroHP > 0)
            //         {
            //             //var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(effectUnit.UnitCamp);
            //             // var addHeroHPTriggerData = new TriggerData()
            //             // {
            //             //     ActionUnitID = triggerData.ActionUnitID,
            //             //     EffectUnitID = playerData.BattleHero.ID,
            //             //     TriggerDataType = ETriggerDataType.RoleAttribute,
            //             //     BattleUnitAttribute = EUnitAttribute.HP,
            //             //     Value = effectUnit.AddHeroHP,
            //             //     ChangeHPInstantly = true,
            //             // };
            //             //
            //             // BattleBuffManager.Instance.CacheTriggerData(addHeroHPTriggerData, triggerDatas[kv.Key]);
            //             //effectUnit.AddHeroHP = 0;
            //         }
            //     }
            // }


            var hpDeltaList = new List<HPDeltaData>();

            foreach (var kv in triggerDatas)
            {

                foreach (var triggerData in kv.Value)
                {

                    if (triggerData.EffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx)
                    {
                        var hpDeltaData = HeroManager.Instance.AddHPDelta(triggerData);
                        if (hpDeltaData != null)
                        {
                            AddHPDetlaData(PlayerManager.Instance.PlayerData.UnitCamp, hpDeltaData);
                            //RoundFightData.HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Add(hpDeltaData);
                            hpDeltaList.Add(hpDeltaData);
                        }

                    }
                    else
                    {

                        var effectUnit = GameUtility.GetUnitDataByIdx(triggerData.EffectUnitIdx);
                        if (effectUnit == null)
                            continue;

                        var triggerValue = triggerData.Value + triggerData.DeltaValue;

                        //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                        if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
                            continue;

                        if (effectUnit.UnitCamp == EUnitCamp.Enemy)
                        {
                            //triggerData.ChangeHPInstantly = true;
                            continue;
                        }


                        var value = effectUnit.AddHeroHP;
                        if (RoundFightData.GamePlayData.BlessCount(EBlessID.AddCurHPByAttackDamage,
                                BattleManager.Instance.CurUnitCamp) > 0)
                        {
                            value = Math.Abs((int)triggerValue);
                        }

                        effectUnit.AddHeroHP = 0;

                        if (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) &&
                            !effectUnit.Exist())
                            continue;

                        if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && !GameUtility.ContainRoundState(
                                GamePlayManager.Instance.GamePlayData,
                                EBuffID.Spec_CurseUnEffect))
                            continue;

                        if (effectUnit.UnitRole != EUnitRole.Hero)
                        {
                            triggerData.HeroHPDelta = true;
                        }


                        // var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
                        // var isHeroUnit = playerData != null && playerData.BattleHero != null &&
                        //                  playerData.BattleHero.Idx == unit.Idx;
                        var units = GameUtility.GetUnitsByCamp(effectUnit.UnitCamp);

                        var isCoreUnit = units.Exists((battleUnit =>
                            battleUnit.Idx == effectUnit.Idx && battleUnit is Data_BattleCore));
                        //hpDeltaDict[effectUnit.UnitCamp].HPDelta += (int) (isCoreUnit ? triggerValue : Math.Abs(value));
                        //hpDeltaDict[effectUnit.UnitCamp].Key = isMoveTriggerData ? kv.Key : playerData.BattleHero.Idx;

                        var hpDeltaData = HeroManager.Instance.AddHPDelta(triggerData);
                        hpDeltaData.HPDelta = (int)(isCoreUnit ? triggerValue : Math.Abs(value));
                        //RoundFightData.HPDeltaDict[PlayerManager.Instance.PlayerData.UnitCamp].Add(hpDeltaData);
                        AddHPDetlaData(PlayerManager.Instance.PlayerData.UnitCamp, hpDeltaData);
                        hpDeltaList.Add(hpDeltaData);
                    }


                }



            }

            var playerData = RoundFightData.GamePlayData.GetPlayerData(PlayerManager.Instance.PlayerData.UnitCamp);
            foreach (var hpDeltaData in hpDeltaList)
            {

                // && kv2.Value.HPDelta != 0
                if (playerData != null && playerData.BattleHero != null)
                {
                    playerData.BattleHero.ChangeHP(hpDeltaData.HPDelta);

                }
            }

            // var hpDeltaDict = new Dictionary<EUnitCamp, HPDeltaData>()
            // {
            //     [EUnitCamp.Player1] = new HPDeltaData(),
            //     [EUnitCamp.Player2] = new HPDeltaData(),
            // };
            //
            //
            // foreach (var kv in triggerDatas)
            // {
            //     for (int i = kv.Value.Count - 1; i >= 0; i--)
            //     {
            //         var triggerData = kv.Value[i];
            //         
            //         var effectUnit = GameUtility.GetUnitDataByID(triggerData.EffectUnitID);
            //         if(effectUnit == null)
            //             continue;
            //
            //         var triggerValue = triggerData.Value + triggerData.DeltaValue;
            //         var value = effectUnit.AddHeroHP;
            //         
            //         if(BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) && effectUnit.CurHP <= 0)
            //             continue;
            //
            //         if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && ! GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData,
            //                 EBuffID.Spec_CurseUnEffect))
            //             continue;
            //         
            //         effectUnit.AddHeroHP = 0;
            //         //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
            //         if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
            //             continue;
            //
            //         var unit = GameUtility.GetUnitDataByID(triggerData.EffectUnitID, true);
            //         if(unit == null)
            //             continue;
            //         
            //         if (unit.UnitCamp == EUnitCamp.Enemy)
            //         {
            //             triggerData.ChangeHPInstantly = true;
            //             continue;
            //         }
            //
            //         if (unit.UnitRole != EUnitRole.Hero)
            //         {
            //             triggerData.ChangeHPInstantly = true;
            //         }
            //
            //         var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
            //         var isHeroUnit = playerData != null && playerData.BattleHero != null &&
            //                          playerData.BattleHero.ID == unit.ID;
            //         
            //         hpDeltaDict[unit.UnitCamp].Value += (int) (isHeroUnit ? triggerValue : Math.Abs(value));
            //         hpDeltaDict[unit.UnitCamp].Key = isMoveTriggerData ? kv.Key : playerData.BattleHero.ID;
            //
            //         if (unit.UnitRole == EUnitRole.Hero && !triggerData.ChangeHPInstantly)
            //         {
            //             kv.Value.RemoveAt(i);
            //         }
            //     }
            //     
            //     // foreach (var triggerData in kv.Value)
            //     // {
            //     //     var effectUnit = GameUtility.GetUnitDataByID(triggerData.EffectUnitID);
            //     //     if(effectUnit == null)
            //     //         continue;
            //     //
            //     //     var triggerValue = triggerData.Value + triggerData.DeltaValue;
            //     //     var value = effectUnit.AddHeroHP;
            //     //     
            //     //     if(BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) && effectUnit.CurHP <= 0)
            //     //         continue;
            //     //
            //     //     if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && ! GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData,
            //     //             EBuffID.Spec_CurseUnEffect))
            //     //         continue;
            //     //     
            //     //     effectUnit.AddHeroHP = 0;
            //     //     //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
            //     //     if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
            //     //         continue;
            //     //
            //     //     var unit = GameUtility.GetUnitDataByID(triggerData.EffectUnitID, true);
            //     //     if(unit == null)
            //     //         continue;
            //     //     
            //     //     if (unit.UnitCamp == EUnitCamp.Enemy)
            //     //     {
            //     //         triggerData.ChangeHPInstantly = true;
            //     //         continue;
            //     //     }
            //     //
            //     //     if (unit.UnitRole != EUnitRole.Hero)
            //     //     {
            //     //         triggerData.ChangeHPInstantly = true;
            //     //     }
            //     //
            //     //     var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
            //     //     var isHeroUnit = playerData != null && playerData.BattleHero != null &&
            //     //                      playerData.BattleHero.ID == unit.ID;
            //     //     
            //     //     hpDeltaDict[unit.UnitCamp].Value += (int) (isHeroUnit ? triggerValue : Math.Abs(value));
            //     //     hpDeltaDict[unit.UnitCamp].Key = isMoveTriggerData ? kv.Key : playerData.BattleHero.ID;
            //     // }
            //     
            //
            //     
            // }
            //
            // foreach (var kv2 in hpDeltaDict)
            // {
            //     var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(kv2.Key);
            //     if (playerData != null && playerData.BattleHero != null && kv2.Value.Value != 0)
            //     {
            //         var triggerData = new TriggerData()
            //         {
            //             EffectUnitID = playerData.BattleHero.ID,
            //             TriggerDataType = ETriggerDataType.RoleAttribute,
            //             BattleUnitAttribute = EUnitAttribute.HP,
            //             Value = kv2.Value.Value,
            //             ChangeHPInstantly = true,
            //         };
            //         triggerData.ActionUnitID = triggerData.Value < 0 ? actionUnitID : -1;
            //         triggerData.OwnUnitID = triggerData.Value < 0 ? actionUnitID : -1;
            //         
            //         if(!triggerDatas.ContainsKey(kv2.Value.Key))
            //         {
            //             triggerDatas.Add(kv2.Value.Key, new List<TriggerData>());
            //         }
            //
            //         BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas[kv2.Value.Key]);
            //
            //     }
            // }
        }

        public void CalculateUnitPaths(EUnitCamp unitCamp, List<int> actionUnitIdxs, List<int> obstacleEnemies,
            Dictionary<int, List<int>> movePaths)
        {

            RefreshObstacleMask();
            //var gridObstacles = GetGridObstacles();

            //var heroCoord = GameUtility.GridPosIdxToCoord(playerData.BattleHero.GridPosIdx);
            //var unitPaths = new Dictionary<int, Dictionary<int, PathState>>();

            var cacheBuffDatas = new Dictionary<int, BuffData>();
            foreach (var unitkey in actionUnitIdxs)
            {
                var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[unitkey] as Data_BattleMonster;
                var drEnemy = GameEntry.DataTable.GetEnemy(battleUnit.MonsterID);
                var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
                cacheBuffDatas.Add(unitkey, buffData);
            }

            var oriGridPosIdxs = new Dictionary<int, int>();
            foreach (var key in actionUnitIdxs)
            {
                var battleUnitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
                oriGridPosIdxs.Add(key, battleUnitData.GridPosIdx);
            }

            var retGetRange = new List<int>(50);
            var retGetRange2 = new List<int>(50);

            actionUnitIdxs.Sort((actionUnitIdx1, actionUnitIdx2) =>
            {
                var unit1 = GetUnitByIdx(actionUnitIdx1);
                var unit2 = GetUnitByIdx(actionUnitIdx2);

                return unit1.GridPosIdx - unit2.GridPosIdx;

            });


            foreach (var actionUnitIdx in actionUnitIdxs)
            {
                var buffData = cacheBuffDatas[actionUnitIdx];
                var battleUnitData =
                    RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitIdx] as Data_BattleMonster;

                if (!battleUnitData.Exist())
                    continue;

                var enemyData = GameEntry.DataTable.GetEnemy(battleUnitData.MonsterID);

                retGetRange.Clear();
                retGetRange2.Clear();

                retGetRange.Add(battleUnitData.GridPosIdx);
                var intersectDict = GameUtility.GetActionGridPosIdxs(battleUnitData.UnitCamp, battleUnitData.GridPosIdx,
                    enemyData.MoveType,
                    buffData.TriggerRange, buffData.TriggerUnitCamps, buffData.BuffTriggerType, ref retGetRange,
                    ref retGetRange2, true);

                var isFindPath = false;


                foreach (var kv in intersectDict)
                {
                    var intersectList = kv.Value;
                    for (int i = 0; i < intersectList.Count; i++)
                    {
                        var intersectGridPosIdx = intersectList[i];

                        if (curObstacleMask[intersectGridPosIdx] == EGridType.Obstacle)
                        {
                            continue;
                        }

                        var runPaths = new List<int>(16);

                        var realPaths =
                            BattleFightManager.Instance.GetRunPaths(curObstacleMask, battleUnitData.GridPosIdx,
                                intersectGridPosIdx, runPaths);


                        var realTargetPosIdx = realPaths[realPaths.Count - 1];

                        // if(realTargetPosIdx == battleUnitData.GridPosIdx && realTargetPosIdx != intersectGridPosIdx)
                        //     continue;

                        if (realTargetPosIdx != intersectGridPosIdx)
                            continue;

                        if (InObstacle(curObstacleMask, realPaths))
                        {
                            continue;
                        }



                        // if(realTargetPosIdx == battleUnitData.GridPosIdx)
                        //     continue;

                        battleUnitData.GridPosIdx = realTargetPosIdx;
                        RefreshUnitGridPosIdx();

                        // var actionGridPosIdx = GameUtility.GetActionGridPosIdx(curObstacleMask, realTargetPosIdx, buffData.TriggerRange, true);
                        //  if (actionGridPosIdx == -1)
                        // {
                        //     battleUnitData.GridPosIdx = oriGridPosIdxs[actionUnitIdx];
                        //     RefreshUnitGridPosIdx();
                        //
                        //     continue;
                        // }

                        RoundFightData.GamePlayData.BattleData.GridTypes[oriGridPosIdxs[actionUnitIdx]] =
                            EGridType.Empty;
                        RoundFightData.GamePlayData.BattleData.GridTypes[realTargetPosIdx] = EGridType.Unit;
                        RefreshObstacleMask();

                        // curObstacleMask[oriGridPosIdxs[actionUnitIdx]] = EGridType.Empty;
                        // curObstacleMask[realTargetPosIdx] = EGridType.Unit;
                        // battleUnitData.GridPosIdx = realTargetPosIdx;
                        // RefreshUnitGridPosIdx();

                        movePaths.Add(actionUnitIdx, realPaths);
                        isFindPath = true;
                        break;
                    }



                    if (isFindPath)
                        break;
                }

                if (!isFindPath)
                {

                    foreach (var kv in intersectDict)
                    {
                        var unit = GetUnitByIdx(kv.Key);

                        SearchPath(curObstacleMask, actionUnitIdx, battleUnitData.GridPosIdx,
                            unit.GridPosIdx, movePaths,
                            enemyData.MoveType.ToString().Contains("Direct8"));
                        if (movePaths.ContainsKey(battleUnitData.Idx) && movePaths[battleUnitData.Idx].Count > 0)
                        {
                            RoundFightData.GamePlayData.BattleData.GridTypes[battleUnitData.GridPosIdx] =
                                EGridType.Empty;
                            RoundFightData.GamePlayData.BattleData.GridTypes[
                                    movePaths[battleUnitData.Idx][movePaths[battleUnitData.Idx].Count - 1]] =
                                EGridType.Unit;
                            RefreshObstacleMask();
                            // curObstacleMask[battleUnitData.GridPosIdx] = EGridType.Empty;
                            // curObstacleMask[movePaths[battleUnitData.Idx][movePaths[battleUnitData.Idx].Count - 1]] =
                            //     EGridType.Unit;
                            break;
                        }
                    }
                }



                // if (!isFindPath)
                // {
                //
                //     foreach (var kv in intersectDict)
                //     {
                //         var unit = GetUnitByIdx(kv.Key);
                //
                //         SearchPath(curObstacleMask, actionUnitIdx, battleUnitData.GridPosIdx,
                //             unit.GridPosIdx, movePaths,
                //             enemyData.MoveType.ToString().Contains("Direct8"));
                //         if (movePaths.ContainsKey(battleUnitData.Idx) && movePaths[battleUnitData.Idx].Count > 0)
                //         {
                //             curObstacleMask[battleUnitData.GridPosIdx] = EGridType.Empty;
                //             curObstacleMask[movePaths[battleUnitData.Idx][movePaths[battleUnitData.Idx].Count - 1]] =
                //                 EGridType.Unit;
                //             break;
                //         }
                //     }
                //
                //
                // }
                if (!movePaths.ContainsKey(actionUnitIdx))
                {
                    movePaths.Add(actionUnitIdx, new List<int>() { battleUnitData.GridPosIdx });
                }

                movePaths[actionUnitIdx] = CacheUnitMoveDatas(actionUnitIdx,
                    movePaths[actionUnitIdx],
                    unitCamp == EUnitCamp.Enemy ? RoundFightData.EnemyMoveDatas : RoundFightData.ThirdUnitMoveDatas,
                    EUnitActionState.Run);

                RefreshObstacleMask();
            }

            RefreshPropMoveDirectUseInRound();

            foreach (var key in actionUnitIdxs)
            {
                var battleEnemy = BattleUnitDatas[key];
                battleEnemy.GridPosIdx = oriGridPosIdxs[key];
            }

            RefreshUnitGridPosIdx();



        }

        public void CalculateUnitPaths(EUnitCamp unitCamp, Dictionary<int, List<int>> movePaths)
        {
            RefreshPropMoveDirectUseInRound();

            movePaths.Clear();

            var unitIdxs = new List<int>();

            foreach (var kv in RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                var battleUnitData = kv.Value;

                if (battleUnitData.UnitCamp != unitCamp)
                    continue;


                if (battleUnitData.GetAllStateCount(EUnitState.UnMove) > 0 &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;

                // var battleEnemyData = battleUnitData as Data_BattleMonster;
                // var drEnemy = GameEntry.DataTable.GetEnemy(battleEnemyData.MonsterID);
                // var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
                //
                // var attackRange = GameUtility.GetRange(battleEnemyData.GridPosIdx, buffData.TriggerRange, unitCamp,
                //     buffData.TriggerUnitCamps, true);
                //
                // if (attackRange.Contains(BattleHeroManager.Instance.BattleHeroData.GridPosIdx))
                // {
                //     movePaths.Add(battleUnitData.ID, new List<int>()
                //     {
                //         battleUnitData.GridPosIdx
                //     });
                //     continue;
                // }

                unitIdxs.Add(battleUnitData.Idx);
            }

            // unitIDs.Sort((unit1ID, unit2ID) =>
            // {
            //     var unit1 = BattleUnitManager.Instance.GetUnitByID(unit1ID).BattleUnit;
            //     var unit2 = BattleUnitManager.Instance.GetUnitByID(unit2ID).BattleUnit;
            //     
            //     
            //     if (unit1.UnitCamp != EUnitCamp.Enemy || unit2.UnitCamp != EUnitCamp.Enemy)
            //     {
            //         return 0;
            //     }
            //
            //     var drEnemy1 = GameEntry.DataTable.GetEnemy(unit1.ID);
            //     var drEnemy2 = GameEntry.DataTable.GetEnemy(unit2.ID);
            //     
            //     if (drEnemy2.AttackType == EAttackType.Lock && drEnemy1.AttackType == EAttackType.Dynamic)
            //         return -1;
            //
            //
            //     return 1;
            //
            // });


            List<int> obstacleEnemies = new List<int>();
            foreach (var kv in RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != unitCamp)
                    continue;

                obstacleEnemies.Add(kv.Value.GridPosIdx);
            }

            // for (int i = unitIDs.Count - 1; i >= 0; i--)
            // {
            //     if(i >= Constant.Enemy.EnemyActionCount)
            //         unitIDs.RemoveAt(i);
            // }

            if (unitIdxs.Count > 0)
            {
                CalculateUnitPaths(unitCamp, unitIdxs, obstacleEnemies, movePaths);

            }

        }

        private void SearchPath(Dictionary<int, EGridType> gridTypes, int actionUnitIdx, int startGridPosIdx,
            int endGridPosIdx, Dictionary<int, List<int>> movePaths, bool isQblique)
        {
            var paths = GameUtility.GetPaths(gridTypes, startGridPosIdx, endGridPosIdx, isQblique);
            if (paths.Count > 0)
            {
                movePaths.Add(actionUnitIdx, paths);
            }

        }

        public Dictionary<int, List<TriggerData>> GetInDirectAttackDatas(int unitIdx)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();


            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {

                    foreach (var triggerDatas in kv2.Value.MoveActionData.TriggerDatas)
                    {
                        foreach (var triggerData in triggerDatas.Value)
                        {
                            // if (triggerData.ActualValue == 0)
                            // {
                            //     continue;
                            // }

                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }

                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    foreach (var triggerDatas in kv2.Value.MoveActionData.TriggerDatas)
                    {
                        foreach (var triggerData in triggerDatas.Value)
                        {
                            // if (triggerData.ActualValue == 0)
                            // {
                            //     continue;
                            // }

                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }


            return triggerDataDict;
        }

        public Dictionary<int, List<TriggerData>> GetDirectAttackDatas(int unitIdx)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();

            // if (RoundFightData.EnemyMoveDatas.ContainsKey(unitIdx))
            // {
            //     var triggerDataList = RoundFightData.EnemyMoveDatas[unitIdx].TriggerDatas.Values.ToList();
            // }
            foreach (var kv in RoundFightData.RoundStartBuffDatas)
            {
                foreach (var datas in kv.Value.TriggerDatas.Values.ToList())
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.OwnUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }
            
            
            foreach (var kv in RoundFightData.EnemyMoveDatas)
            {
                foreach (var datas in kv.Value.TriggerDatas.Values.ToList())
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.OwnUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }


            // if (RoundFightData.SoliderMoveDatas.ContainsKey(unitIdx))
            // {
            //     var triggerDataList = RoundFightData.SoliderMoveDatas[unitIdx].TriggerDatas.Values.ToList();
            //     
            //     
            // }
            foreach (var kv in RoundFightData.SoliderMoveDatas)
            {
                foreach (var datas in kv.Value.TriggerDatas.Values.ToList())
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.OwnUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }
            
            foreach (var kv in RoundFightData.EnemyMoveDatas)
            {
                foreach (var datas in kv.Value.TriggerDatas.Values.ToList())
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.OwnUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }

            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDatas.Values)
                {
                    foreach (var triggerData in kv2)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.OwnUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDatas.Values)
                {
                    foreach (var triggerData in kv2)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.OwnUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderAttackDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDatas.Values)
                {
                    foreach (var triggerData in kv2)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.OwnUnitIdx == unitIdx)
                        {
                            if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                        }
                    }
                }
            }



            foreach (var kv in RoundFightData.BuffData_Use.TriggerDatas)
            {
                foreach (var triggerData in kv.Value)
                {
                    // if (triggerData.ActualValue == 0)
                    // {
                    //     continue;
                    // }

                    if (triggerData.OwnUnitIdx == unitIdx)
                    {
                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }
                }
            }

            return triggerDataDict;
        }


        public Dictionary<int, List<TriggerData>> GetHurtInDirectAttackDatas(int effectUnitIdx, int actionUnitIdx = -1)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();

            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {

                var triggerDataList = kv.Value.MoveData.MoveUnitDatas.Values.ToList();
                foreach (var moveUnitData in triggerDataList)
                {
                    foreach (var kv2 in moveUnitData.MoveActionData.TriggerDatas)
                    {
                        foreach (var triggerData in kv2.Value)
                        {
                            // if (triggerData.ActualValue == 0)
                            // {
                            //     continue;
                            // }

                            if (triggerData.EffectUnitIdx != effectUnitIdx)
                                continue;

                            if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 &&
                                triggerData.ActionUnitIdx != actionUnitIdx)
                                continue;

                            if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.ActionUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.ActionUnitIdx].Add(triggerData);
                        }
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {

                var triggerDataList = kv.Value.MoveData.MoveUnitDatas.Values.ToList();
                foreach (var moveUnitData in triggerDataList)
                {
                    foreach (var kv2 in moveUnitData.MoveActionData.TriggerDatas)
                    {
                        foreach (var triggerData in kv2.Value)
                        {
                            // if (triggerData.ActualValue == 0)
                            // {
                            //     continue;
                            // }

                            if (triggerData.EffectUnitIdx != effectUnitIdx)
                                continue;

                            if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 &&
                                triggerData.ActionUnitIdx != actionUnitIdx)
                                continue;

                            if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
                            {
                                triggerDataDict.Add(triggerData.ActionUnitIdx, new List<TriggerData>());
                            }

                            triggerDataDict[triggerData.ActionUnitIdx].Add(triggerData);
                        }
                    }
                }

            }


            return triggerDataDict;
        }

        public Dictionary<int, List<TriggerData>> GetHurtDirectAttackDatas(int effectUnitIdx, int actionUnitIdx = -1)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();

            foreach (var kv in RoundFightData.RoundStartBuffDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDatas)
                {
                    foreach (var triggerData in kv2.Value)
                    {
                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                        {
                            continue;
                        }

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        {
                            continue;
                        }

                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }

                }
            }

            foreach (var kv in RoundFightData.BuffData_Use.TriggerDatas)
            {

                foreach (var triggerData in kv.Value)
                {
                    // if (triggerData.ActualValue == 0)
                    // {
                    //     continue;
                    // }

                    if (triggerData.EffectUnitIdx != effectUnitIdx)
                    {
                        continue;
                    }

                    if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                    {
                        continue;
                    }

                    if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                    {
                        triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                    }

                    triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                }
            }

            foreach (var kv in RoundFightData.EnemyMoveDatas)
            {
                var triggerDataList = kv.Value.TriggerDatas.Values.ToList();
                foreach (var datas in triggerDataList)
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                        {
                            continue;
                        }

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        {
                            continue;
                        }

                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }
                }
            }

            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {
                var triggerDataList = kv.Value.TriggerDatas.Values.ToList();
                foreach (var datas in triggerDataList)
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                        {
                            continue;
                        }

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        {
                            continue;
                        }

                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {
                var triggerDataList = kv.Value.TriggerDatas.Values.ToList();
                foreach (var datas in triggerDataList)
                {
                    foreach (var triggerData in datas)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                        {
                            continue;
                        }

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        {
                            continue;
                        }

                        if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                    }
                }
            }

            foreach (var kv in RoundFightData.SoliderMoveDatas)
            {

                foreach (var kv2 in kv.Value.TriggerDatas)
                {
                    foreach (var triggerData in kv2.Value)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                            continue;

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 &&
                            triggerData.ActionUnitIdx != actionUnitIdx)
                            continue;

                        if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.ActionUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.ActionUnitIdx].Add(triggerData);
                    }
                }
            }
            
            foreach (var kv in RoundFightData.EnemyMoveDatas)
            {

                foreach (var kv2 in kv.Value.TriggerDatas)
                {
                    foreach (var triggerData in kv2.Value)
                    {
                        // if (triggerData.ActualValue == 0)
                        // {
                        //     continue;
                        // }

                        if (triggerData.EffectUnitIdx != effectUnitIdx)
                            continue;

                        if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 &&
                            triggerData.ActionUnitIdx != actionUnitIdx)
                            continue;

                        if (!triggerDataDict.ContainsKey(triggerData.ActionUnitIdx))
                        {
                            triggerDataDict.Add(triggerData.ActionUnitIdx, new List<TriggerData>());
                        }

                        triggerDataDict[triggerData.ActionUnitIdx].Add(triggerData);
                    }
                }
            }

            // foreach (var kv in RoundFightData.BuffData_Use.TriggerDatas)
            // {
            //
            //     foreach (var triggerData in kv.Value)
            //     {
            //         if (triggerData.EffectUnitIdx != effectUnitIdx)
            //         {
            //             continue;
            //         }
            //             
            //         if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
            //         {
            //             continue;
            //         }
            //             
            //         if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
            //         {
            //             triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
            //         }
            //         triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
            //     }
            // }


            return triggerDataDict;
        }

        public Dictionary<int, List<TriggerData>> GetTacticHurtAttackDatas(int effectUnitIdx)
        {
            var triggerDataDict = new Dictionary<int, List<TriggerData>>();


            foreach (var kv in RoundFightData.BuffData_Use.TriggerDatas)
            {

                foreach (var triggerData in kv.Value)
                {
                    // if (triggerData.ActualValue == 0)
                    // {
                    //     continue;
                    // }

                    if (triggerData.EffectUnitIdx != effectUnitIdx)
                    {
                        continue;
                    }

                    // if (actionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                    // {
                    //     continue;
                    // }

                    if (!triggerDataDict.ContainsKey(triggerData.EffectUnitIdx))
                    {
                        triggerDataDict.Add(triggerData.EffectUnitIdx, new List<TriggerData>());
                    }

                    triggerDataDict[triggerData.EffectUnitIdx].Add(triggerData);
                }
            }


            return triggerDataDict;
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


    }
}